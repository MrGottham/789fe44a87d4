using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Domstolene.JFS.CommonLibrary.IoC.Interfaces;
using DsiNext.DeliveryEngine.Domain.Metadata;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.IoC;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Log;
using DsiNext.DeliveryEngine.Repositories.DataManipulators;
using DsiNext.DeliveryEngine.Repositories.Interfaces;
using DsiNext.DeliveryEngine.Repositories.Interfaces.DataManipulators;
using DsiNext.DeliveryEngine.Repositories.Interfaces.Enums;
using DsiNext.DeliveryEngine.Resources;

namespace DsiNext.DeliveryEngine.Infrastructure.IoC
{
    /// <summary>
    /// Configuration provider to data manipulators for data repositories.
    /// </summary>
    public class DataManipulatorsConfigurationProvider : IDataValidatorsConfigurationProvider
    {
        #region IConfigurationProvider Members

        /// <summary>
        ///  Adding configuration to the container for Inversion of Control.
        /// </summary>
        /// <param name="container">Container for Inversion of Control.</param>
        public void AddConfiguration(IWindsorContainer container)
        {
            var dataManipulatorsFileName = ConfigurationManager.AppSettings["DataManipulatorsFileName"];
            if (dataManipulatorsFileName == null)
            {
                throw new DeliveryEngineSystemException(Resource.GetExceptionMessage(ExceptionMessage.ApplicationSettingMissing, "DataManipulatorsFileName"));
            }

            dataManipulatorsFileName = Environment.ExpandEnvironmentVariables(dataManipulatorsFileName);
            if (string.IsNullOrEmpty(dataManipulatorsFileName))
            {
                container.Register(Component.For<IDataManipulators>().ImplementedBy<DataManipulators>().LifeStyle.Transient);
                return;
            }
            if (!File.Exists(dataManipulatorsFileName))
            {
                throw new DeliveryEngineSystemException(Resource.GetExceptionMessage(ExceptionMessage.FileNotFound, dataManipulatorsFileName));
            }

            var assembly = typeof (DataManipulators).Assembly;
            using (var resourceStream = assembly.GetManifestResourceStream(string.Format("{0}.Schemas.DataManipulators.xsd", assembly.GetName().Name)))
            {
                if (resourceStream == null)
                {
                    throw new DeliveryEngineSystemException(Resource.GetExceptionMessage(ExceptionMessage.ResourceNotFound, string.Format("{0}.Schemas.DataManipulators.xsd", assembly.GetName().Name)));
                }
                var schema = XmlSchema.Read(resourceStream, ValidationEventHandler);
                var readerSettings = new XmlReaderSettings
                    {
                        IgnoreComments = true,
                        IgnoreProcessingInstructions = true,
                        IgnoreWhitespace = true,
                        ValidationFlags = XmlSchemaValidationFlags.ReportValidationWarnings,
                        ValidationType = ValidationType.Schema
                    };
                readerSettings.Schemas.Add(schema);
                readerSettings.ValidationEventHandler += ValidationEventHandler;
                var reader = XmlReader.Create(dataManipulatorsFileName, readerSettings);
                try
                {
                    var document = new XmlDocument();
                    document.Load(reader);
                    var namespaceManager = new XmlNamespaceManager(document.NameTable);
                    namespaceManager.AddNamespace("ns", schema.TargetNamespace);
                    var dataSetterNodeList = document.SelectNodes("/ns:DataManipulators/ns:DataSetter", namespaceManager);
                    if (dataSetterNodeList != null && dataSetterNodeList.Count > 0)
                    {
                        var dataSetterNo = 0;
                        foreach (XmlNode dataSetterNode in dataSetterNodeList)
                        {
                            var criteriaConfigurations = GenerateCriteriaConfigurations(dataSetterNode.SelectNodes("ns:EqualCriteria | ns:PoolCriteria | ns:IntervalCriteria", namespaceManager));
                            // ReSharper disable PossibleNullReferenceException
                            var dataSetter = new DataSetter(dataSetterNode.Attributes["table"].Value, dataSetterNode.SelectSingleNode("ns:Field", namespaceManager).Attributes["name"].Value, dataSetterNode.SelectSingleNode("ns:Field", namespaceManager).Attributes["value"].Value, criteriaConfigurations);
                            // ReSharper restore PossibleNullReferenceException
                            container.Register(Component.For<IDataSetter>().Named(string.Format("{0}[{1}]", typeof(IDataSetter).Name, dataSetterNo++)).Instance(dataSetter).LifeStyle.Transient);
                        }
                    }
                    var regularExpressionReplacerNodeList = document.SelectNodes("/ns:DataManipulators/ns:RegularExpressionReplacer", namespaceManager);
                    if (regularExpressionReplacerNodeList != null && regularExpressionReplacerNodeList.Count > 0)
                    {
                        var regularExpressionReplacerNo = 0;
                        foreach (XmlNode regularExpressionReplacerNode in regularExpressionReplacerNodeList)
                        {
                            var fieldNode = regularExpressionReplacerNode.SelectSingleNode("ns:Field", namespaceManager);
                            // ReSharper disable PossibleNullReferenceException
                            var regularExpression = new Regex(fieldNode.Attributes["regularExpression"].Value, RegexOptions.Compiled);
                            var newFieldValue = fieldNode.Attributes["value"].Value;
                            var regularExpressionReplacer = new RegularExpressionReplacer(regularExpressionReplacerNode.Attributes["table"].Value, regularExpression, (RegularExpressionApplyOn) Enum.Parse(typeof (RegularExpressionApplyOn), fieldNode.Attributes["applyOn"].Value), fieldNode.Attributes["name"].Value, Equals(newFieldValue, "{null}") ? null : newFieldValue);
                            // ReSharper restore PossibleNullReferenceException
                            container.Register(Component.For<IRegularExpressionReplacer>().Named(string.Format("{0}[{1}]", typeof (IRegularExpressionReplacer).Name, regularExpressionReplacerNo++)).Instance(regularExpressionReplacer).LifeStyle.Transient);
                        }
                    }
                    var rowDuplicatorNodeList = document.SelectNodes("/ns:DataManipulators/ns:RowDuplicator", namespaceManager);
                    if (rowDuplicatorNodeList != null && rowDuplicatorNodeList.Count > 0)
                    {
                        var rowDuplicatorNo = 0;
                        foreach (XmlNode rowDuplicatorNode in rowDuplicatorNodeList)
                        {
                            var criteriaConfigurations = GenerateCriteriaConfigurations(rowDuplicatorNode.SelectNodes("ns:EqualCriteria | ns:PoolCriteria | ns:IntervalCriteria", namespaceManager));
                            var fieldUpdates = new Collection<Tuple<string, object>>();
                            var fieldNodeList = rowDuplicatorNode.SelectNodes("ns:Field", namespaceManager);
                            if (fieldNodeList != null && fieldNodeList.Count > 0)
                            {
                                foreach (XmlNode fieldNode in fieldNodeList)
                                {
                                    // ReSharper disable PossibleNullReferenceException
                                    fieldUpdates.Add(new Tuple<string, object>(fieldNode.Attributes["name"].Value, fieldNode.Attributes["value"].Value));
                                    // ReSharper restore PossibleNullReferenceException
                                }
                            }
                            // ReSharper disable PossibleNullReferenceException
                            var rowDuplicator = new RowDuplicator(rowDuplicatorNode.Attributes["table"].Value, fieldUpdates, criteriaConfigurations);
                            // ReSharper restore PossibleNullReferenceException
                            container.Register(Component.For<IRowDuplicator>().Named(string.Format("{0}[{1}]", typeof (IRowDuplicator).Name, rowDuplicatorNo++)).Instance(rowDuplicator).LifeStyle.Transient);
                        }
                    }
                    var missingForeignKeyHandlerNodeList = document.SelectNodes("ns:DataManipulators/ns:MissingForeignKeyHandler", namespaceManager);
                    if (missingForeignKeyHandlerNodeList != null && missingForeignKeyHandlerNodeList.Count > 0)
                    {
                        var missingForeignKeyHandlerNo = 0;
                        foreach (XmlNode missingForeignKeyHandlerNode in missingForeignKeyHandlerNodeList)
                        {
                            var workerNode = missingForeignKeyHandlerNode.SelectSingleNode("ns:PrimaryKeyAdder | ns:ForeignKeyCleaner | ns:ForeignKeyDeleter", namespaceManager);
                            if (workerNode == null)
                            {
                                continue;
                            }
                            var foreignKeyFields = new List<string>();
                            var foreignKeyFieldNodeList = workerNode.SelectNodes("ns:ForeignKey", namespaceManager);
                            if (foreignKeyFieldNodeList != null && foreignKeyFieldNodeList.Count > 0)
                            {
                                // ReSharper disable PossibleNullReferenceException
                                foreignKeyFields.AddRange(from XmlNode foreignKeyFieldNode in foreignKeyFieldNodeList select foreignKeyFieldNode.Attributes["field"].Value);
                                // ReSharper restore PossibleNullReferenceException
                            }
                            IMissingForeignKeyWorker worker = null;
                            switch (workerNode.LocalName)
                            {
                                case "PrimaryKeyAdder":
                                    var setFieldValues = new Dictionary<string, object>();
                                    var setFieldValueNodeList = workerNode.SelectNodes("ns:SetValue", namespaceManager);
                                    if (setFieldValueNodeList != null && setFieldValueNodeList.Count > 0)
                                    {
                                        foreach (XmlNode setFieldValueNode in setFieldValueNodeList)
                                        {
                                            // ReSharper disable PossibleNullReferenceException
                                            setFieldValues.Add(setFieldValueNode.Attributes["field"].Value, setFieldValueNode.Attributes["value"].Value);
                                            // ReSharper restore PossibleNullReferenceException
                                        }
                                    }
                                    try
                                    {
                                        // ReSharper disable PossibleNullReferenceException
                                        worker = new PrimaryKeyAdder(workerNode.Attributes["targetTable"].Value, foreignKeyFields, setFieldValues, container.Resolve<IMetadataRepository>(), container.Resolve<IInformationLogger>());
                                        // ReSharper restore PossibleNullReferenceException
                                    }
                                    catch (DeliveryEngineSystemException)
                                    {
                                        continue;
                                    }
                                    break;

                                case "ForeignKeyCleaner":
                                    try
                                    {
                                        // ReSharper disable PossibleNullReferenceException
                                        worker = new ForeignKeyCleaner(workerNode.Attributes["targetTable"].Value, foreignKeyFields, container.Resolve<IMetadataRepository>(), container.Resolve<IInformationLogger>());
                                        // ReSharper restore PossibleNullReferenceException
                                    }
                                    catch (DeliveryEngineSystemException)
                                    {
                                        continue;
                                    }
                                    break;

                                case "ForeignKeyDeleter":
                                    try
                                    {
                                        // ReSharper disable PossibleNullReferenceException
                                        worker = new ForeignKeyDeleter(workerNode.Attributes["targetTable"].Value, foreignKeyFields, container.Resolve<IMetadataRepository>(), container.Resolve<IInformationLogger>());
                                        // ReSharper restore PossibleNullReferenceException
                                    }
                                    catch (DeliveryEngineSystemException)
                                    {
                                        continue;
                                    }
                                    break;
                            }
                            // ReSharper disable PossibleNullReferenceException
                            var missingForeignKeHandler = new MissingForeignKeyHandler(missingForeignKeyHandlerNode.Attributes["table"].Value, worker, container.Resolve<IContainer>());
                            // ReSharper restore PossibleNullReferenceException
                            container.Register(Component.For<IMissingForeignKeyHandler>().Named(string.Format("{0}[{1}]", typeof (IMissingForeignKeyHandler).Name, missingForeignKeyHandlerNo++)).Instance(missingForeignKeHandler).LifeStyle.Transient);
                        }
                    }
                }
                finally
                {
                    reader.Close();
                }
                resourceStream.Close();
            }
            
            container.Register(Component.For<IDataManipulators>().ImplementedBy<DataManipulators>().LifeStyle.Transient);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Generates the configuration for criterias to the data manipulator.
        /// </summary>
        /// <param name="criteriaNodes">XML node list of criterias.</param>
        /// <returns>Configuration for criterias to the data manipulator.</returns>
        private static IEnumerable<Tuple<Type, string, object>> GenerateCriteriaConfigurations(XmlNodeList criteriaNodes)
        {
            var criteriaConfigurations = new Collection<Tuple<Type, string, object>>();
            if (criteriaNodes == null || criteriaNodes.Count == 0)
            {
                return criteriaConfigurations;
            }
            foreach (XmlNode criteriaNode in criteriaNodes)
            {
                switch (criteriaNode.LocalName)
                {
                    case "EqualCriteria":
                        // ReSharper disable PossibleNullReferenceException
                        var equalCriteriaConfiguration = new Tuple<Type, string, object>(typeof (EqualCriteria<>), criteriaNode.Attributes["field"].Value, criteriaNode.Attributes["value"].Value);
                        // ReSharper restore PossibleNullReferenceException
                        criteriaConfigurations.Add(equalCriteriaConfiguration);
                        break;

                    case "PoolCriteria":
                        // ReSharper disable PossibleNullReferenceException
                        var poolCriteriaConfiguration = new Tuple<Type, string, object>(typeof (PoolCriteria<>), criteriaNode.Attributes["field"].Value, criteriaNode.Attributes["values"].Value.Split(','));
                        // ReSharper restore PossibleNullReferenceException
                        criteriaConfigurations.Add(poolCriteriaConfiguration);
                        break;

                    case "IntervalCriteria":
                        // ReSharper disable PossibleNullReferenceException
                        var intervalCriteriaConfiguration = new Tuple<Type, string, object>(typeof (IntervalCriteria<>), criteriaNode.Attributes["field"].Value, new Tuple<string, string>(criteriaNode.Attributes["fromValue"].Value, criteriaNode.Attributes["toValue"].Value));
                        // ReSharper restore PossibleNullReferenceException
                        criteriaConfigurations.Add(intervalCriteriaConfiguration);
                        break;
                }
            }
            return criteriaConfigurations;
        }

        /// <summary>
        /// Event handler for XML validation erros.
        /// </summary>
        /// <param name="sender">Object which raise the event.</param>
        /// <param name="eventArgs">Arguments for the event.</param>
        private static void ValidationEventHandler(object sender, ValidationEventArgs eventArgs)
        {
            if (sender == null)
            {
                throw new ArgumentNullException("sender");
            }
            if (eventArgs == null)
            {
                throw new ArgumentNullException("eventArgs");
            }
            switch (eventArgs.Severity)
            {
                case XmlSeverityType.Warning:
                case XmlSeverityType.Error:
                    throw new DeliveryEngineSystemException(Resource.GetExceptionMessage(ExceptionMessage.XmlValidationError, eventArgs.Message), eventArgs.Exception);
            }
        }

        #endregion
    }
}

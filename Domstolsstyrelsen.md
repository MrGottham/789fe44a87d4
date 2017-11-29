1. Tilret primærnøglen på tabellen AFGOER i ########.xml til at være:
```xml
<pn>
    <titel>Sagsident</titel>
    <titel>PLøbenr</titel>
    <!--
    <titel>Dato</titel>
    <titel>Tid</titel>
    <titel>RLøbenr</titel>
    -->
</pn>
```

2. Tilret feltnavnet på Interval i tabellen PARM i ########.xml til at være:
```xml
<feltdef>
    <titel>"Interval"</titel>
    <datatype>time</datatype>
    <bredde>8</bredde>
    <feltinfo>Tidsinterval for berammelser</feltinfo>
</feltdef>
```

3. Tilføj SQL forespørgelser i bunden af ########.xml:
```xml
```

4. Kør det nye afleveringsprogram: DsiNext.DeliveryEngine.Gui.ArchiveMaker.exe /ArchiveInformationPackageID:AVID.SA.20029
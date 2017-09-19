# DeliveryEngine

## Sourcekode
Sourcekoden til afleveringsmotoren kan hentes på [GitHub](https://github.com/MrGottham/789fe44a87d4)

## Kørsel af DeliveryEngine
Syntaks: DsiNext.DeliveryEngine.Gui.ArchiveMaker.exe [/ArchiveInformationPackageID:[ID]] [/ValidationOnly:[true | false]] [/TablesHandledSimultaneity:[Antal]] [/AcceptWarnings:Antal]] [/RemoveMissingRelationshipsOnForeignKeys:[true | false]] [/NumberOfForeignTablesToCache:[Antal]]

- ArchiveInformationPackageID (påkrævet) angiver ID for den arkivinformationspakke, der skal dannes, eksempelvis XXX
- ValidationOnly (optionel) der med værdien true angiver, at der kun udføres validering
- TablesHandledSimultaneity (optinel) angiver, hvor mange tabeller, der skal dannes arkiv for samtidig
- AcceptWarnings (optionel) angiver antallet af advasler før dannelse af arkiveringen fejler
- RemoveMissingRelationshipsOnForeignKeys (optionel) der med værdien true angiver, at poster med manglende foreign key fjernes
- NumberOfForeignTablesToCache (optioenl) angiver antallet af foreign key tabeller, der skal caches

## Konfiguration
Filen DsiNext.DeliveryEngine.Gui.ArchiveMaker.config indeholder konfigurationsværdier til brug for afleveringsmotoren.

Det værende sig:
- Folder til logning
- Folder til kilde (det gamle afleveringsformat)
- Folder til destination (det nye afleveringsformat)
- Konfigurationsværdier, som skal indgå i den nye afleveringsformat og som ikke kan trækkes fra det gamle afleveringsformat
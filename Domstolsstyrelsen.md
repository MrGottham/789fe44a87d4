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

3. Tilføj SQL forespørgelser til ########.xml:
```xml
<saq>
	<saqinfo>Søg auktionssag ud fra sagsdatointerval</saqinfo>
	<saqdata>CREATE TABLE f (fradato DATE, tildato DATE)	INSERT INTO f (fradato,tildato) VALUES(,)	SELECT BsNr, BsÅr FROM SAGLISTE WHERE SystemNr=5 AND Mdato&gt;=(SELECT fradato FROM f) AND Mdato&lt;=(SELECT tildato FROM f)	DROP TABLE f</saqdata>
</saq>
<saq>
	<saqinfo>Søg auktionssag ud fra sagsnummer</saqinfo>
	<saqdata>CREATE TABLE f (bsnr INTEGER, bsaar INTEGER)	INSERT INTO f (bsnrm,bsaar) VALUES(,)	SELECT * FROM SAGLISTE WHERE SystemNr=5 AND BsNr=(SELECT bsnr FROM f) AND BsÅr=(SELECT bsaar FROM f)	DROP TABLE f</saqdata>
</saq>
<saq>
	<saqinfo>Søg auktionssag ud fra sagspart</saqinfo>
	<saqdata>CREATE TABLE f (soegenavn VARCHAR(100))	INSERT INTO f (soegenavn) VALUES('')	SELECT SAGLISTE.BsNr, SAGLISTE.BsÅr FROM SAGLISTE, PARTER WHERE SAGLISTE.Sagsident=PARTER.Sagsident AND PARTER.SystemNr=5 AND PARTER.Søgenavn LIKE (SELECT soegenavn FROM f) + '%'	DROP TABLE f</saqdata>
</saq>
<saq>
	<saqinfo>Søg civilsag ud fra sagsdatointerval</saqinfo>
	<saqdata>CREATE TABLE f (fradato DATE, tildato DATE)	INSERT INTO f (fradato,tildato) VALUES(,)	SELECT BsNr, BsÅr FROM SAGLISTE WHERE SystemNr=1 AND Mdato&gt;=(SELECT fradato FROM f) AND Mdato&lt;=(SELECT tildato FROM f)	DROP TABLE f</saqdata>
</saq>
<saq>
	<saqinfo>Søg civilsag ud fra sagsnummer</saqinfo>
	<saqdata>CREATE TABLE f (bsnr INTEGER, bsaar INTEGER)	INSERT INTO f (bsnrm,bsaar) VALUES(,)	SELECT * FROM SAGLISTE WHERE SystemNr=1 AND BsNr=(SELECT bsnr FROM f) AND BsÅr=(SELECT bsaar FROM f)	DROP TABLE f</saqdata>
</saq>
<saq>
	<saqinfo>Søg civilsag ud fra sagspart</saqinfo>
	<saqdata>CREATE TABLE f (soegenavn VARCHAR(100))	INSERT INTO f (soegenavn) VALUES('')	SELECT SAGLISTE.BsNr, SAGLISTE.BsÅr FROM SAGLISTE, PARTER WHERE SAGLISTE.Sagsident=PARTER.Sagsident AND PARTER.SystemNr=1 AND PARTER.Søgenavn LIKE (SELECT soegenavn FROM f) + '%'	DROP TABLE f</saqdata>
</saq>
<saq>
	<saqinfo>Søg dødsbo skiftesag ud fra sagsdatointerval</saqinfo>
	<saqdata>CREATE TABLE f (fradato DATE, tildato DATE)	INSERT INTO f (fradato,tildato) VALUES(,)	SELECT BsNr, BsÅr FROM SAGLISTE WHERE SystemNr=24 AND Mdato&gt;=(SELECT fradato FROM f) AND Mdato&lt;=(SELECT tildato FROM f)	DROP TABLE f</saqdata>
</saq>
<saq>
	<saqinfo>Søg dødsbo skiftesag ud fra sagsnummer</saqinfo>
	<saqdata>CREATE TABLE f (bsnr INTEGER, bsaar INTEGER)	INSERT INTO f (bsnrm,bsaar) VALUES(,)	SELECT * FROM SAGLISTE WHERE SystemNr=24 AND BsNr=(SELECT bsnr FROM f) AND BsÅr=(SELECT bsaar FROM f)	DROP TABLE f</saqdata>
</saq>
<saq>
	<saqinfo>Søg dødsbo skiftesag ud fra sagspart</saqinfo>
	<saqdata>CREATE TABLE f (soegenavn VARCHAR(100))	INSERT INTO f (soegenavn) VALUES('')	SELECT SAGLISTE.BsNr, SAGLISTE.BsÅr FROM SAGLISTE, PARTER WHERE SAGLISTE.Sagsident=PARTER.Sagsident AND PARTER.SystemNr=24 AND PARTER.Søgenavn LIKE (SELECT soegenavn FROM f) + '%'	DROP TABLE f</saqdata>
</saq>
<saq>
	<saqinfo>Søg fogedsag ud fra sagsdatointerval</saqinfo>
	<saqdata>CREATE TABLE f (fradato DATE, tildato DATE)	INSERT INTO f (fradato,tildato) VALUES(,)	SELECT BsNr, BsÅr FROM SAGLISTE WHERE SystemNr=2 AND Mdato&gt;=(SELECT fradato FROM f) AND Mdato&lt;=(SELECT tildato FROM f)	DROP TABLE f</saqdata>
</saq>
<saq>
	<saqinfo>Søg fogedsag ud fra sagsnummer</saqinfo>
	<saqdata>CREATE TABLE f (bsnr INTEGER, bsaar INTEGER)	INSERT INTO f (bsnrm,bsaar) VALUES(,)	SELECT * FROM SAGLISTE WHERE SystemNr=2 AND BsNr=(SELECT bsnr FROM f) AND BsÅr=(SELECT bsaar FROM f)	DROP TABLE f</saqdata>
</saq>
<saq>
	<saqinfo>Søg fogedsag ud fra sagspart</saqinfo>
	<saqdata>CREATE TABLE f (soegenavn VARCHAR(100))	INSERT INTO f (soegenavn) VALUES('')	SELECT SAGLISTE.BsNr, SAGLISTE.BsÅr FROM SAGLISTE, PARTER WHERE SAGLISTE.Sagsident=PARTER.Sagsident AND PARTER.SystemNr=2 AND PARTER.Søgenavn LIKE (SELECT soegenavn FROM f) + '%'	DROP TABLE f</saqdata>
</saq>
<saq>
	<saqinfo>Søg insolvens skiftesag ud fra sagsdatointerval</saqinfo>
	<saqdata>CREATE TABLE f (fradato DATE, tildato DATE)	INSERT INTO f (fradato,tildato) VALUES(,)	SELECT BsNr, BsÅr FROM SAGLISTE WHERE SystemNr=4 AND Mdato&gt;=(SELECT fradato FROM f) AND Mdato&lt;=(SELECT tildato FROM f)	DROP TABLE f</saqdata>
</saq>
<saq>
	<saqinfo>Søg insolvens skiftesag ud fra sagsnummer</saqinfo>
	<saqdata>CREATE TABLE f (bsnr INTEGER, bsaar INTEGER)	INSERT INTO f (bsnrm,bsaar) VALUES(,)	SELECT * FROM SAGLISTE WHERE SystemNr=4 AND BsNr=(SELECT bsnr FROM f) AND BsÅr=(SELECT bsaar FROM f)	DROP TABLE f</saqdata>
</saq>
<saq>
	<saqinfo>Søg insolvens skiftesag ud fra sagspart</saqinfo>
	<saqdata>CREATE TABLE f (soegenavn VARCHAR(100))	INSERT INTO f (soegenavn) VALUES('')	SELECT SAGLISTE.BsNr, SAGLISTE.BsÅr FROM SAGLISTE, PARTER WHERE SAGLISTE.Sagsident=PARTER.Sagsident AND PARTER.SystemNr=4 AND PARTER.Søgenavn LIKE (SELECT soegenavn FROM f) + '%'	DROP TABLE f</saqdata>
</saq>
<saq>
	<saqinfo>Søg straffesag ud fra sagsdatointerval</saqinfo>
	<saqdata>CREATE TABLE f (fradato DATE, tildato DATE)	INSERT INTO f (fradato,tildato) VALUES(,)	SELECT BsNr, BsÅr FROM SAGLISTE WHERE SystemNr=8 AND Mdato&gt;=(SELECT fradato FROM f) AND Mdato&lt;=(SELECT tildato FROM f)	DROP TABLE f</saqdata>
</saq>
<saq>
	<saqinfo>Søg straffesag ud fra sagsnummer</saqinfo>
	<saqdata>CREATE TABLE f (bsnr INTEGER, bsaar INTEGER)	INSERT INTO f (bsnrm,bsaar) VALUES(,)	SELECT * FROM SAGLISTE WHERE SystemNr=8 AND BsNr=(SELECT bsnr FROM f) AND BsÅr=(SELECT bsaar FROM f)	DROP TABLE f</saqdata>
</saq>
<saq>
	<saqinfo>Søg straffesag ud fra sagspart</saqinfo>
	<saqdata>CREATE TABLE f (soegenavn VARCHAR(100))	INSERT INTO f (soegenavn) VALUES('')	SELECT SAGLISTE.BsNr, SAGLISTE.BsÅr FROM SAGLISTE, PARTER WHERE SAGLISTE.Sagsident=PARTER.Sagsident AND PARTER.SystemNr=8 AND PARTER.Søgenavn LIKE (SELECT soegenavn FROM f) + '%'	DROP TABLE f</saqdata>
</saq>
```

4. Kør det nye afleveringsprogram: DsiNext.DeliveryEngine.Gui.ArchiveMaker.exe /ArchiveInformationPackageID:AVID.SA.20029
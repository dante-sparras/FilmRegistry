# INLÄMNINGSUPPGIFT INDIVIDUELL

## Följande kursmål examineras

- Färdigheter: Genomföra databaskopplingar med ADO.NET samt Entity Framework och LINQ.

## Syfte

Syftet med uppgiften är att du ska visa att du självständigt kan skapa en databaskoppling mellan en C#-applikation och en SQL Server-databas med hjälp av ADO.NET.

Du ska använda ADO.NET för att läsa, söka, lägga till och ta bort data från en databas.

## Scenario

Du ska skapa en mindre applikation för att hantera ett filmregister.

Applikationen ska vara en Console Application i C# som ansluter till en SQL Server-databas. I databasen lagras filmer och deras genrer.

Databasen ska innehålla två relaterade tabeller:

- Movie
- Genre

En film ska tillhöra en genre.

## Uppgift

Applikationen ska använda ADO.NET för kommunikationen med databasen.

För att erhålla Godkänt ska applikationen kunna:

- Ansluta till en SQL Server-databas med ADO.NET.
- Visa alla filmer tillsammans med tillhörande genre.
- Söka efter filmer utifrån genre.
- Lägga till en ny film och koppla filmen till en befintlig genre.
- Ta bort en film.
- Använda parametriserade SQL-frågor där användardata skickas till databasen.
- Hantera databasanslutningar på ett korrekt sätt.

Du ska använda relevanta delar av ADO.NET, exempelvis SqlConnection, SqlCommand, SqlParameter och SqlDataReader.

## Kodstruktur

Koden ska vara strukturerad och läsbar. All kod ska inte ligga direkt i Program.cs.

Dataåtkomsten ska separeras från användargränssnittet, exempelvis genom en MovieRepository eller motsvarande klass.

## Exempel på meny

När programmet startar kan användaren exempelvis mötas av:

```text
--- Filmregister ---
1. Visa alla filmer
2. Sök filmer efter genre
3. Lägg till film
4. Ta bort film
0. Avsluta
Välj:
```

## Databas

Du skapar själv databasen och tabellerna Movie och Genre i SQL Server.

Tabellerna ska ha en korrekt relation där en film tillhör en genre.

Lägg till ett antal filmer och genrer så att applikationens funktioner går att testa.

När filmer visas ska information från båda tabellerna kunna presenteras, exempel:

- Alien (1979) - Sci-Fi
- The Shining (1980) - Horror
- Die Hard (1988) – Action

## Inlämning

Projektet lämnas in med länk till ditt repo på Github i lärplattformen. OBS! det måste vara public i alla fall tills rättning är klar.

Applikationen ska kunna startas och testas. Koden ska vara strukturerad och läsbar.

Uppgiften ska hållas enkel och fokusera på kraven ovan. Ytterligare funktionalitet krävs inte och påverkar inte bedömningen.

## Bedömning

Uppgiften bedöms IG/G.

För att erhålla betyget Godkänt ska samtliga krav i uppgiften vara genomförda.

Applikationen ska visa att du självständigt kan:

- genomföra en databaskoppling med ADO.NET,
- läsa och förändra data i databasen,
- använda parametriserade SQL-frågor,
- arbeta med data från relaterade tabeller,
- och strukturera applikationen så att dataåtkomsten är separerad från användargränssnittet.

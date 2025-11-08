ASP.NET Core Razor Pages app that:
1. fetches current weather data (temperature) from Open-Meteo API based on most popular polish cities (Poznan, Warszawa, Wroclaw, Krakow) locations in selected time frame;
2. stores the result in a SQLite database using Entity Framework Core;
3. displays the saved observations on a UI page in a tabular manner based on one selected city;
4. fetches the data asynchronously (async/await) both from the API and database;
5. uses dependency injections.

When starting for the first time, enter the following commands in the terminal:
1. dotnet restore;
2. dotnet ef database update;
3. dotnet run.

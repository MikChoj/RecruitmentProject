namespace BAERecruitmentProject.Services;

public static class CityDefinitions
{
    public static readonly CityLocation[] Cities =
    [
        new CityLocation("Warszawa", 52.2297, 21.0122),
        new CityLocation("Poznań", 52.4064, 16.9252),
        new CityLocation("Wrocław", 51.1079, 17.0385),
        new CityLocation("Kraków", 50.0647, 19.9450)
    ];
}

public record CityLocation(string Name, double Latitude, double Longitude);
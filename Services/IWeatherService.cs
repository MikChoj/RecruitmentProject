using System.Collections.Generic;
using System.Threading.Tasks;
using BAERecruitmentProject.Models;

namespace BAERecruitmentProject.Services;

public interface IWeatherService
{
    Task FetchAndStoreAllCitiesAsync();
    Task<List<WeatherObservation>> GetObservationsForCityAsync(string cityName);
    Task<List<WeatherObservation>> GetObservationsForCityAsync(string cityName, DateOnly? start = null, DateOnly? end = null);
    IReadOnlyList<CityLocation> GetSupportedCities();
    Task ClearAllObservationsAsync();
    Task FetchAndStoreAllCitiesInRangeAsync(DateOnly startDate, DateOnly endDate);
}
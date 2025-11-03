using BAERecruitmentProject.Data;
using BAERecruitmentProject.Models;
using Microsoft.EntityFrameworkCore;

namespace BAERecruitmentProject.Services;

public class WeatherService : IWeatherService
{
    private readonly IWeatherApiClient _apiClient;
    private readonly WeatherDbContext _dbContext;

    public WeatherService(IWeatherApiClient apiClient, WeatherDbContext dbContext)
    {
        _apiClient = apiClient;
        _dbContext = dbContext;
    }

    public IReadOnlyList<CityLocation> GetSupportedCities() => CityDefinitions.Cities;

    public async Task FetchAndStoreAllCitiesAsync()
    {
        var nowUtc = DateTime.UtcNow;

        var results = await Task.WhenAll(
            CityDefinitions.Cities.Select(async city =>
            {
                var temp = await _apiClient.GetCurrentTemperatureAsync(city.Latitude, city.Longitude);
                return (city, temp);
            })
        );

        foreach (var (city, temp) in results)
        {
            if (temp is null) continue;

            var exists = await _dbContext.WeatherObservations.AnyAsync(o =>
                o.CityName == city.Name && o.ObservedAtUtc == nowUtc);

            if (exists) continue;

            await _dbContext.WeatherObservations.AddAsync(new WeatherObservation
            {
                CityName = city.Name,
                TemperatureC = temp.Value,
                ObservedAtUtc = nowUtc
            });
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task FetchAndStoreAllCitiesInRangeAsync(DateOnly startDate, DateOnly endDate)
    {
        if (endDate < startDate)
            (startDate, endDate) = (endDate, startDate);

        var results = await Task.WhenAll(
            CityDefinitions.Cities.Select(async city =>
            {
                var points = await _apiClient.GetHourlyTemperaturesAsync(
                    city.Latitude, city.Longitude, startDate, endDate);
                return (city, points);
            })
        );

        foreach (var (city, points) in results)
        {
            if (points is null || points.Count == 0) continue;

            var minTime = points.Min(p => p.timeUtc);
            var maxTime = points.Max(p => p.timeUtc);

            var existingTimes = await _dbContext.WeatherObservations
                .Where(o => o.CityName == city.Name &&
                            o.ObservedAtUtc >= minTime &&
                            o.ObservedAtUtc <= maxTime)
                .Select(o => o.ObservedAtUtc)
                .ToListAsync();

            var existingSet = new HashSet<DateTime>(existingTimes);

            foreach (var (timeUtc, temp) in points)
            {
                if (existingSet.Contains(timeUtc)) continue;

                await _dbContext.WeatherObservations.AddAsync(new WeatherObservation
                {
                    CityName = city.Name,
                    TemperatureC = temp,
                    ObservedAtUtc = timeUtc
                });
            }
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<WeatherObservation>> GetObservationsForCityAsync(string cityName)
    {
        return await _dbContext.WeatherObservations
            .Where(o => o.CityName == cityName)
            .OrderByDescending(o => o.ObservedAtUtc)
            .ToListAsync();
    }

    public Task<List<WeatherObservation>> GetObservationsForCityAsync(string cityName, DateOnly? start = null, DateOnly? end = null)
    {
        var query = _dbContext.WeatherObservations
            .Where(o => o.CityName == cityName);

        if (start.HasValue)
        {
            var s = start.Value.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
            query = query.Where(o => o.ObservedAtUtc >= s);
        }
        if (end.HasValue)
        {
            var e = end.Value.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc);
            query = query.Where(o => o.ObservedAtUtc <= e);
        }

        return query.OrderByDescending(o => o.ObservedAtUtc).ToListAsync();
    }

    public async Task ClearAllObservationsAsync()
    {
        await _dbContext.Database.ExecuteSqlRawAsync("DELETE FROM WeatherObservations");
    }
}
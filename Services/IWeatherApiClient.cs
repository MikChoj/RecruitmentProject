using System.Threading.Tasks;

namespace BAERecruitmentProject.Services;

public interface IWeatherApiClient
{
    Task<double?> GetCurrentTemperatureAsync(double latitude, double longitude);
    Task<IReadOnlyList<(DateTime timeUtc, double temp)>> GetHourlyTemperaturesAsync(
        double latitude,
        double longitude,
        DateOnly startDate,
        DateOnly endDate);
}
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace BAERecruitmentProject.Services;

public class OpenMeteoWeatherApiClient : IWeatherApiClient
{
    private readonly HttpClient _httpClient;

    public OpenMeteoWeatherApiClient(HttpClient httpClient)
    {
        httpClient.BaseAddress = new Uri("https://api.open-meteo.com/");
        _httpClient = httpClient;
    }

    public async Task<double?> GetCurrentTemperatureAsync(double latitude, double longitude)
    {
        var url = $"v1/forecast?latitude={latitude}&longitude={longitude}&current_weather=true";

        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        using var stream = await response.Content.ReadAsStreamAsync();

        var json = await JsonDocument.ParseAsync(stream);
        if (json.RootElement.TryGetProperty("current_weather", out var current))
        {
            if (current.TryGetProperty("temperature", out var tempProp))
            {
                return tempProp.GetDouble();
            }
        }

        return null;
    }

    public async Task<IReadOnlyList<(DateTime timeUtc, double temp)>> GetHourlyTemperaturesAsync(
        double latitude, double longitude, DateOnly startDate, DateOnly endDate)
    {
        var url =
            $"v1/forecast?latitude={latitude}&longitude={longitude}" +
            $"&hourly=temperature_2m&start_date={startDate:yyyy-MM-dd}&end_date={endDate:yyyy-MM-dd}&timezone=UTC";

        using var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync();
        using var json = await JsonDocument.ParseAsync(stream);

        if (!json.RootElement.TryGetProperty("hourly", out var hourly))
            return Array.Empty<(DateTime, double)>();

        if (!hourly.TryGetProperty("time", out var times) ||
            !hourly.TryGetProperty("temperature_2m", out var temps) ||
            times.ValueKind != JsonValueKind.Array ||
            temps.ValueKind != JsonValueKind.Array)
            return Array.Empty<(DateTime, double)>();

        var count = Math.Min(times.GetArrayLength(), temps.GetArrayLength());
        var result = new List<(DateTime, double)>(capacity: count);

        for (int i = 0; i < count; i++)
        {
            var tStr = times[i].GetString();

            if (tStr is null) continue;

            if (!DateTime.TryParse(tStr, null,
                System.Globalization.DateTimeStyles.AssumeUniversal | System.Globalization.DateTimeStyles.AdjustToUniversal,
                out var timeUtc))
                continue;

            var tempVal = temps[i].GetDouble();
            result.Add((timeUtc, tempVal));
        }

        return result;
    }
}
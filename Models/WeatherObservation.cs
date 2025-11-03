using System.ComponentModel.DataAnnotations;

namespace BAERecruitmentProject.Models;

public class WeatherObservation
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string CityName { get; set; } = string.Empty;

    public double TemperatureC { get; set; }

    public DateTime ObservedAtUtc { get; set; }
}
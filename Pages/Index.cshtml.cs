using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BAERecruitmentProject.Models;
using BAERecruitmentProject.Services;

namespace BAERecruitmentProject.Pages;

public class IndexModel : PageModel
{
    private readonly IWeatherService _weatherService;

    public IndexModel(IWeatherService weatherService)
    {
        _weatherService = weatherService;
    }

    public IReadOnlyList<CityLocation> Cities { get; private set; } = [];

    public List<WeatherObservation> Observations { get; private set; } = [];

    [BindProperty(SupportsGet = true)]
    public string SelectedCity { get; set; } = "Warszawa";

    [BindProperty(SupportsGet = true)]
    public DateTime StartDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime EndDate { get; set; }

    public async Task OnGetAsync()
    {
        Cities = _weatherService.GetSupportedCities();

        if (string.IsNullOrWhiteSpace(SelectedCity))
            SelectedCity = Cities.First().Name;

        if (StartDate == default)
            StartDate = DateTime.UtcNow.Date.AddDays(-1);
            
        if (EndDate == default)
            EndDate = DateTime.UtcNow.Date;

        Observations = await _weatherService.GetObservationsForCityAsync(SelectedCity, DateOnly.FromDateTime(StartDate), DateOnly.FromDateTime(EndDate));
    }

    public async Task<IActionResult> OnPostFetchAsync()
    {
        await _weatherService.FetchAndStoreAllCitiesAsync();

        return RedirectToPage(new { SelectedCity });
    }

    public async Task<IActionResult> OnPostFetchRangeAsync()
    {
        var start = DateOnly.FromDateTime(StartDate.Date);
        var end   = DateOnly.FromDateTime(EndDate.Date);

        await _weatherService.FetchAndStoreAllCitiesInRangeAsync(start, end);

        return RedirectToPage(new { SelectedCity, StartDate = StartDate.ToString("yyyy-MM-dd"), EndDate = EndDate.ToString("yyyy-MM-dd") });
    }

    public async Task<IActionResult> OnPostClearAsync()
    {
        await _weatherService.ClearAllObservationsAsync();
        return RedirectToPage(new { SelectedCity });
    }
}
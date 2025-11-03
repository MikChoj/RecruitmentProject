using Microsoft.EntityFrameworkCore;
using BAERecruitmentProject.Data;
using BAERecruitmentProject.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

var connectionString = builder.Configuration.GetConnectionString("WeatherDb") ?? "Data Source=weather.db";

builder.Services.AddDbContext<WeatherDbContext>(options => options.UseSqlite(connectionString));

builder.Services.AddHttpClient<IWeatherApiClient, OpenMeteoWeatherApiClient>();

builder.Services.AddScoped<IWeatherService, WeatherService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();

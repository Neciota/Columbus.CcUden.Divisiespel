using Columbus.CcUden.Divisiespel.Calculator;
using Columbus.CcUden.Divisiespel.Console;
using Columbus.CcUden.Divisiespel.Console.Pages;
using Columbus.CcUden.Divisiespel.Fetcher;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

#region Logic
builder.Services.AddScoped<IHtmlParser, HtmlParser>();
builder.Services.AddScoped<CompuClubFetcher>();
builder.Services.AddScoped<IStandingsCalculator, StandingsCalculator>();
#endregion

#region Stores
builder.Services.AddScoped<YearStore>();
#endregion

#region Pages
builder.Services.AddScoped<Router>();
builder.Services.AddTransient<HomePage>();
builder.Services.AddTransient<AddFlightPage>();
builder.Services.AddTransient<EditYearPage>();
builder.Services.AddTransient<ViewAddedFlightsPage>();
builder.Services.AddTransient<ViewStandingsPage>();
#endregion

var host = builder.Build();

Router router = host.Services.GetRequiredService<Router>();
await router.NavigateToAsync<HomePage>();
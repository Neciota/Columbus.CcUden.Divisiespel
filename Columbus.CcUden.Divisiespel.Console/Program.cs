using Columbus.CcUden.Divisiespel.Calculator;
using Columbus.CcUden.Divisiespel.Console;
using Columbus.CcUden.Divisiespel.Console.Pages;
using Columbus.CcUden.Divisiespel.Fetcher;
using Columbus.CcUden.Divisiespel.Persistence;
using Columbus.CcUden.Divisiespel.Writer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

#region Logic
builder.Services.AddScoped<IHtmlParser, HtmlParser>();
builder.Services.AddScoped<CompuClubFetcher>();
builder.Services.AddScoped<IStandingsCalculator, StandingsCalculator>();
builder.Services.AddScoped<PdfWriter>();
#endregion

#region Stores
builder.Services.AddScoped<YearStore>();
builder.Services.AddScoped<StandingsStore>();
#endregion

#region Pages
builder.Services.AddScoped<Router>();
builder.Services.AddTransient<HomePage>();
builder.Services.AddTransient<AddFlightPage>();
builder.Services.AddTransient<EditYearPage>();
builder.Services.AddTransient<ExportStandingsPage>();
builder.Services.AddTransient<ViewAddedFlightsPage>();
builder.Services.AddTransient<ViewLeaguesPage>();
builder.Services.AddTransient<ViewStandingsPage>();
builder.Services.AddTransient<ViewUnregisteredOwners>();
#endregion

var host = builder.Build();

Router router = host.Services.GetRequiredService<Router>();
await router.NavigateToAsync<HomePage>();
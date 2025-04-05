using Columbus.CcUden.Divisiespel.Calculator;
using Columbus.CcUden.Divisiespel.Fetcher;
using Columbus.CcUden.Divisiespel.Models;
using Columbus.CcUden.Divisiespel.Writer;
using Spectre.Console;
using System.IO;
using System.Web;

namespace Columbus.CcUden.Divisiespel.Console.Pages
{
    internal class AddFlightPage(
        Router router, 
        CompuClubFetcher fetcher, 
        IStandingsCalculator standingsCalculator,
        YearStore yearStore,
        StandingsStore standingsStore) : Page(router)
    {
        private readonly CompuClubFetcher _fetcher = fetcher;
        private readonly IStandingsCalculator _calculator = standingsCalculator;
        private readonly YearStore _yearStore = yearStore;
        private readonly StandingsStore _standingsStore = standingsStore;

        public override async Task ShowAsync()
        {
            await base.ShowAsync();

            await InitializeSessionAsync();
            (string flightCode, string path) = await GetFlightPathAsync();
            IEnumerable<OwnerResult> ownerResults = await GetOwnerResultsAsync(flightCode, path);
            await ShowOwnerResults(ownerResults, flightCode);
        }

        private async Task InitializeSessionAsync()
        {
            await AnsiConsole.Status()
                .StartAsync("CompuClub laden...", async ctx =>
                {
                    ctx.Spinner(Spinner.Known.Star);
                    ctx.SpinnerStyle(Style.Parse("green"));

                    await _fetcher.TryUpdateSessionIdAsync();
                    await _fetcher.SetYear(_yearStore.Year);
                });
        }

        private async Task<(string, string)> GetFlightPathAsync()
        {
            IEnumerable<string> paths = [];
            await AnsiConsole.Status()
                .StartAsync("Vluchten laden...", async ctx =>
                {
                    ctx.Spinner(Spinner.Known.Star);
                    ctx.SpinnerStyle(Style.Parse("green"));

                    paths = await _fetcher.GetCcFlightLinks();
                });

            string[] flightCodes = paths.Select(x => HttpUtility.ParseQueryString(x)["vlc"])
                .Where(x => !string.IsNullOrEmpty(x))
                .Cast<string>()
                .ToArray();

            string selectedFlightCode = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Selecteer een vluchtcode:")
                    .PageSize(10)
                    .MoreChoicesText("[grey](Beweeg omhoog/omlaag om meer vluchtcodes te zien.)[/]")
                    .AddChoices(flightCodes));
            return (selectedFlightCode, paths.First(p => p.Contains($"vlc={selectedFlightCode}", StringComparison.InvariantCultureIgnoreCase)));
        }

        private async Task<IEnumerable<OwnerResult>> GetOwnerResultsAsync(string flightCode, string path)
        {
            IEnumerable<ResultLine> results = [];
            await AnsiConsole.Status()
                .StartAsync($"Vlucht {flightCode} laden...", async ctx =>
                {
                    ctx.Spinner(Spinner.Known.Star);
                    ctx.SpinnerStyle(Style.Parse("green"));

                    results = await _fetcher.GetResults(path);
                });

            return _calculator.GetOwnerResultsFromSingleFlight(results);
        }

        private async Task ShowOwnerResults(IEnumerable<OwnerResult> ownerResults, string flightCode)
        {
            Table table = new();
            table.AddColumn("Naam");
            table.AddColumn("Punten");

            foreach (OwnerResult ownerResult in ownerResults)
                table.AddRow(ownerResult.Name, ownerResult.GetPoints().ToString());

            AnsiConsole.Write(table);

            var shouldStoreResults = AnsiConsole.Prompt(
                new TextPrompt<bool>("Resultaten opslaan.")
                    .AddChoice(true)
                    .AddChoice(false)
                    .WithConverter(choice => choice ? "j" : "n"));

            if (!shouldStoreResults)
                return;

            StandingsYear currentStandings = await _standingsStore.GetByYearAsync(_yearStore.Year);
            StandingsYear updatedStandings = _calculator.GetUpdatedStandingsFromResults(currentStandings, flightCode, ownerResults);
            await _standingsStore.SaveAsync(updatedStandings);
        }
    }
}
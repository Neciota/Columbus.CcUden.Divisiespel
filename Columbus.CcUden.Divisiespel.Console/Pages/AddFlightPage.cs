using Columbus.CcUden.Divisiespel.Calculator;
using Columbus.CcUden.Divisiespel.Fetcher;
using Columbus.CcUden.Divisiespel.Models;
using Spectre.Console;
using System.Web;

namespace Columbus.CcUden.Divisiespel.Console.Pages
{
    internal class AddFlightPage(
        Router router, 
        CompuClubFetcher fetcher, 
        IStandingsCalculator standingsCalculator,
        YearStore yearStore) : Page(router)
    {
        private readonly CompuClubFetcher _fetcher = fetcher;
        private readonly IStandingsCalculator _calculator = standingsCalculator;
        private readonly YearStore _yearStore = yearStore;

        public override async Task ShowAsync()
        {
            await base.ShowAsync();

            bool hasSession = false;
            await AnsiConsole.Status()
                .StartAsync("CompuClub laden...", async ctx =>
                {
                    ctx.Spinner(Spinner.Known.Star);
                    ctx.SpinnerStyle(Style.Parse("green"));

                    hasSession = await _fetcher.TryUpdateSessionIdAsync();
                    await _fetcher.SetYear(_yearStore.Year);
                });

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
            string selectedPath = paths.First(p => p.Contains($"vlc={selectedFlightCode}", StringComparison.InvariantCultureIgnoreCase));

            IEnumerable<ResultLine> results = [];
            await AnsiConsole.Status()
                .StartAsync($"Vlucht {selectedFlightCode} laden...", async ctx =>
                {
                    ctx.Spinner(Spinner.Known.Star);
                    ctx.SpinnerStyle(Style.Parse("green"));

                    results = await _fetcher.GetResults(selectedPath);
                });

            IEnumerable<OwnerResult> ownerResults = _calculator.GetOwnerResultsFromSingleFlight(results);
            //foreach (OwnerResult ownerResult in ownerResults)
            //    Console.WriteLine($"{ownerResult.Name,-20} {ownerResult.Occurences,-1} {ownerResult.HasDesignated}");
        }
    }
}

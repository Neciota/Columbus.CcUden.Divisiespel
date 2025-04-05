using Columbus.CcUden.Divisiespel.Models;
using Columbus.CcUden.Divisiespel.Persistence;
using Spectre.Console;

namespace Columbus.CcUden.Divisiespel.Console.Pages
{
    internal class ViewAddedFlightsPage(Router router, YearStore yearStore, StandingsStore standingsStore) : Page(router)
    {
        private readonly YearStore _yearStore = yearStore;
        private readonly StandingsStore _standingsStore = standingsStore;

        public override async Task ShowAsync()
        {
            await base.ShowAsync();

            var year = new Panel($"Jaar: {_yearStore.Year}")
                .Border(BoxBorder.Square)
                .Padding(1, 1)
                .Expand();
            AnsiConsole.Write(year);

            StandingsYear standingsYear = await _standingsStore.GetByYearAsync(_yearStore.Year);

            Table table = new();
            table.AddColumn("Vluchten:");

            foreach (string flight in standingsYear.OwnerResultByFlight.Keys)
                table.AddRow(flight);

            AnsiConsole.Write(table);

            AnsiConsole.Prompt(new TextPrompt<string>("Terug naar hoofdmenu?").AllowEmpty());
        }
    }
}

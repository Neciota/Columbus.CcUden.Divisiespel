using Columbus.CcUden.Divisiespel.Models;
using Columbus.CcUden.Divisiespel.Persistence;
using Spectre.Console;

namespace Columbus.CcUden.Divisiespel.Console.Pages
{
    internal class ViewAddedFlightsPage(Router router, YearStore yearStore, StandingsStore standingsStore) : Page(router, yearStore)
    {
        private readonly StandingsStore _standingsStore = standingsStore;

        public override async Task ShowAsync()
        {
            await base.ShowAsync();

            StandingsYear standingsYear = await _standingsStore.GetByYearAsync(_yearStore.Year);

            Table table = new();
            table.AddColumn("Vluchten:");

            foreach (FlightCode flight in standingsYear.GetFlights())
                table.AddRow(flight.ToString());

            AnsiConsole.Write(table);

            AnsiConsole.Prompt(new TextPrompt<string>("Terug naar hoofdmenu?").AllowEmpty());
        }
    }
}

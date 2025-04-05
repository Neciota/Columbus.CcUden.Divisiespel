using Columbus.CcUden.Divisiespel.Models;
using Columbus.CcUden.Divisiespel.Persistence;
using Spectre.Console;

namespace Columbus.CcUden.Divisiespel.Console.Pages
{
    internal class ViewStandingsPage(Router router, YearStore yearStore, StandingsStore standingsStore) : Page(router)
    {
        private readonly YearStore _yearStore = yearStore;
        private readonly StandingsStore _standingsStore = standingsStore;

        public override async Task ShowAsync()
        {
            await base.ShowAsync();

            StandingsYear standingsYear = await _standingsStore.GetByYearAsync(_yearStore.Year);

            Table table = new();

            table.AddColumn("Liefhebber");
            FlightCode[] flights = standingsYear.GetFlights();
            foreach (var flight in flights)
                table.AddColumn(flight.ToString());
            table.AddColumn("Totaal");

            string[] allOwners = standingsYear.GetAllOwners();
            Dictionary<(FlightCode Flight, string Name), int> pointsByOwnerFlight = standingsYear.GetPointsByOwnerAndFlight();
            foreach (var owner in allOwners)
            {
                IEnumerable<int> points = flights.Select(flight => pointsByOwnerFlight.GetValueOrDefault((flight, owner), 0));
                string[] rowData = points
                    .Append(points.Sum())
                    .Select(p => p.ToString())
                    .Prepend(owner)
                    .ToArray();

                table.AddRow(rowData);
            }

            AnsiConsole.Write(table);

            AnsiConsole.Prompt(new TextPrompt<string>("Terug naar hoofdmenu?").AllowEmpty());
        }
    }
}

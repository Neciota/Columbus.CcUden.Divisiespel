using Columbus.CcUden.Divisiespel.Models;
using Columbus.CcUden.Divisiespel.Persistence;
using Spectre.Console;

namespace Columbus.CcUden.Divisiespel.Console.Pages
{
    internal class ViewStandingsPage(Router router, YearStore yearStore, StandingsStore standingsStore) : Page(router, yearStore)
    {
        private readonly StandingsStore _standingsStore = standingsStore;

        public override async Task ShowAsync()
        {
            await base.ShowAsync();

            StandingsYear standingsYear = await _standingsStore.GetByYearAsync(_yearStore.Year);

            Table table = new();

            table.AddColumn(string.Empty);
            table.AddColumn("Liefhebber");
            FlightCode[] flights = standingsYear.GetFlights();
            foreach (var flight in flights)
                table.AddColumn(flight.ToString());
            table.AddColumn("Totaal");

            OwnerStandings[] ownerStandings = standingsYear.GetOwnerStandingsByOwner();
            Dictionary<(FlightCode Flight, Owner Owner), int> pointsByOwnerFlight = standingsYear.GetPointsByOwnerAndFlight();
            foreach (League league in standingsYear.Leagues)
            {
                int leagueIndex = 1;
                table.AddRow(string.Empty, $"[green]{league.Name}[/]");
                foreach (var ownerStanding in ownerStandings.Where(o => league.Owners.Contains(o.Owner)))
                {
                    IEnumerable<int> points = flights.Select(flight => pointsByOwnerFlight.GetValueOrDefault((flight, ownerStanding.Owner), 0));
                    string[] rowData = points
                        .Append(ownerStanding.TotalPoints)
                        .Select(p => $"{(leagueIndex % 2 == 0 ? "[white]" : "[grey]")}{p}[/]")
                        .Prepend($"{(leagueIndex % 2 == 0 ? "[white]" : "[grey]")}{ownerStanding.Owner}[/]")
                        .Prepend($"{(leagueIndex % 2 == 0 ? "[white]" : "[grey]")}{leagueIndex}[/]")
                        .ToArray();

                    table.AddRow(rowData);
                    leagueIndex++;
                }
            }

            AnsiConsole.Write(table);

            AnsiConsole.Prompt(new TextPrompt<string>("Terug naar hoofdmenu?").AllowEmpty());
        }
    }
}

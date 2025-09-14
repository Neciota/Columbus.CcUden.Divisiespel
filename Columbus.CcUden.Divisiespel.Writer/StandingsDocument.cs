using Columbus.CcUden.Divisiespel.Models;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace Columbus.CcUden.Divisiespel.Writer
{
    internal class StandingsDocument(StandingsYear standingsYear, IEnumerable<FlightCode> flightsToShow) : IDocument
    {
        private readonly StandingsYear _standingsYear = standingsYear;
        private readonly IEnumerable<FlightCode> _flightsToShow = flightsToShow;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Margin(50);

                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposeBody);
                page.Footer().Element(ComposeFooter);
            });
        }

        private static void ComposeHeader(IContainer container)
        {
            container.Column(column =>
            {
                column.Item().Text("CC Uden - Divisiespel").FontSize(20).SemiBold();
                column.Item().PaddingBottom(30).Text($"Stand per {DateTime.Now:d MMMM yyyy}");
            });
        }

        private void ComposeBody(IContainer container) 
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(1f);
                    columns.RelativeColumn(5f);
                    foreach (FlightCode _ in _flightsToShow)
                        columns.RelativeColumn(1f);
                    columns.RelativeColumn(2f);
                });

                table.Header(header =>
                {
                    header.Cell().Text("Pos.");
                    header.Cell().Text("Liefhebber");
                    foreach (FlightCode flight in _flightsToShow)
                        header.Cell().Text(flight.ToString());
                    header.Cell().Text("Totaal");
                });

                OwnerStandings[] ownerStandings = _standingsYear.GetOwnerStandingsByOwner();
                Dictionary<(FlightCode Flight, Owner Owner), int> pointsByOwnerFlight = _standingsYear.GetPointsByOwnerAndFlight();
                foreach (League league in _standingsYear.Leagues)
                {
                    table.Cell().Text(string.Empty);
                    table.Cell();
                    foreach (FlightCode _ in _flightsToShow)
                        table.Cell();
                    table.Cell();
                    
                    table.Cell();
                    table.Cell().Text(league.Name);
                    foreach (FlightCode _ in _flightsToShow)
                        table.Cell();
                    table.Cell();

                    int leagueIndex = 1;
                    foreach (var ownerStanding in ownerStandings.Where(o => league.Owners.Contains(o.Owner)))
                    {
                        table.Cell().Text(leagueIndex.ToString());
                        table.Cell().Text(ownerStanding.Owner.ToString());
                        foreach (FlightCode flight in _flightsToShow)
                            table.Cell().Text(pointsByOwnerFlight.GetValueOrDefault((flight, ownerStanding.Owner), 0).ToString());
                        table.Cell().Text(ownerStanding.TotalPoints.ToString());

                        leagueIndex++;
                    }
                }
            });
        }

        private void ComposeFooter(IContainer container) 
        {
            container.Column(column =>
            {
                column.Item().PaddingTop(30).Text($"Aanwezige vluchten: {string.Join(", ", _standingsYear.GetFlights())}.");
            });
        }
    }
}

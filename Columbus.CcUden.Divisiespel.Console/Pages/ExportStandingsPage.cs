using Columbus.CcUden.Divisiespel.Models;
using Columbus.CcUden.Divisiespel.Persistence;
using Columbus.CcUden.Divisiespel.Writer;
using Spectre.Console;

namespace Columbus.CcUden.Divisiespel.Console.Pages
{
    internal class ExportStandingsPage(Router router, YearStore yearStore, StandingsStore standingsStore, PdfWriter pdfWriter) : Page(router, yearStore)
    {
        private readonly StandingsStore _standingsStore = standingsStore;
        private readonly PdfWriter _pdfWriter = pdfWriter;

        public override async Task ShowAsync()
        {
            await base.ShowAsync();

            StandingsYear standingsYear = await _standingsStore.GetByYearAsync(_yearStore.Year);

            string saveFolderPath = AnsiConsole.Prompt(new TextPrompt<string>("Locatie op de schijf waar de PDF moet worden opgeslagen?"));

            List<FlightCode> flights = AnsiConsole.Prompt(new MultiSelectionPrompt<FlightCode>()
                .Title("Welke vluchten moeten worden getoond?")
                .PageSize(10)
                .MoreChoicesText("[grey](Beweeg omhoog/omlaag om meer vluchtcodes te zien.)[/]")
                .InstructionsText("[grey](Druk op [blue]<spatiebalk>[/] om een vlucht te selecteren, [green]<enter>[/] om te bevestigen)[/]")
                .AddChoices(standingsYear.GetFlights()));

            string pdfPath = await _pdfWriter.WriteStandingsAsync(_yearStore.Year, saveFolderPath, [.. flights]);

            AnsiConsole.Markup($"PDF is opgeslagen naar: [grey]{pdfPath}[/]");
            AnsiConsole.WriteLine();
            AnsiConsole.Prompt(new TextPrompt<string>("Terug naar hoofdmenu?").AllowEmpty());
        }
    }
}

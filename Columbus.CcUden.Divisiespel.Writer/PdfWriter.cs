using Columbus.CcUden.Divisiespel.Models;
using Columbus.CcUden.Divisiespel.Persistence;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace Columbus.CcUden.Divisiespel.Writer
{
    public class PdfWriter(StandingsStore standingsStore)
    {
        private readonly StandingsStore _standingsStore = standingsStore;

        public async Task<string> WriteStandingsAsync(int year, string folderPath, params FlightCode[] flightsToShow)
        {
            StandingsYear standingsYear = await _standingsStore.GetByYearAsync(year);
            IDocument document = new StandingsDocument(standingsYear, flightsToShow);

            byte[] pdf = document.GeneratePdf();

            string filePath = Path.Combine(folderPath, $"CC Uden Divisiespel {DateTime.Now:dd-MM-yyyy}.pdf");
            await File.WriteAllBytesAsync(filePath, pdf);

            return filePath;
        }
    }
}

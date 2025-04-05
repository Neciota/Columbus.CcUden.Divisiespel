using Columbus.CcUden.Divisiespel.Models;
using System.Text.Json;

namespace Columbus.CcUden.Divisiespel.Persistence
{
    public class StandingsStore()
    {
        private readonly string _saveFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Columbus.CcUden.Divisiespel");
        private StandingsYear? _loadedStandings;

        public async Task<StandingsYear> GetByYearAsync(int year)
        {
            if (_loadedStandings?.Year == year)
                return _loadedStandings;

            if (!Directory.Exists(_saveFolder))
                Directory.CreateDirectory(_saveFolder);

            string saveFile = GetFilePathForYear(year);
            if (File.Exists(saveFile))
            {
                await using FileStream saveStream = File.OpenRead(saveFile);
                _loadedStandings = await JsonSerializer.DeserializeAsync<StandingsYear>(saveStream);
            }

            _loadedStandings ??= new StandingsYear { Year = year };
            return _loadedStandings;
        }

        public async Task SaveAsync(StandingsYear standingsYear)
        {
            _loadedStandings = standingsYear;

            string saveFile = GetFilePathForYear(standingsYear.Year);
            await using FileStream saveStream = File.OpenWrite(saveFile);

            saveStream.SetLength(0);
            await saveStream.FlushAsync();

            await JsonSerializer.SerializeAsync(saveStream, _loadedStandings);
        }

        private string GetFilePathForYear(int year) => Path.Combine(_saveFolder, $"database_{year}.json");
    }
}

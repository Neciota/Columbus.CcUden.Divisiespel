using Columbus.CcUden.Divisiespel.Models;
using Columbus.CcUden.Divisiespel.Persistence;
using Spectre.Console;

namespace Columbus.CcUden.Divisiespel.Console.Pages
{
    internal class ViewLeaguesPage(Router router, YearStore yearStore, StandingsStore standingsStore) : Page(router, yearStore)
    {
        private readonly StandingsStore _standingsStore = standingsStore;
        private bool _keepAlive = true;

        private const string CREATE_LEAGUE = "Divisie toevoegen.";
        private const string DELETE_LEAGUE = "Divisie verwijderen.";
        private const string IMPORT_LEAGUES = "Divisies importeren.";
        private const string CREATE_OWNER = "Liefhebber toevoegen.";
        private const string DELETE_OWNER = "Liefhebber verwijderen.";
        private const string PROMOTE_OWNER = "Liefhebber promoveren.";
        private const string DEMOTE_OWNER = "Liefhebber degraderen.";
        private const string BACK_TO_MAIN_MENU = "Terug naar hoofdmenu.";

        public override async Task ShowAsync()
        {
            do
            {
                await base.ShowAsync();

                StandingsYear standingsYear = await _standingsStore.GetByYearAsync(_yearStore.Year);

                if (standingsYear.Leagues.Count > 0)
                    ShowLeaguesTable(standingsYear);

                string option = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .AddChoices(
                            CREATE_LEAGUE,
                            DELETE_LEAGUE,
                            IMPORT_LEAGUES,
                            CREATE_OWNER,
                            DELETE_OWNER,
                            PROMOTE_OWNER,
                            DEMOTE_OWNER,
                            BACK_TO_MAIN_MENU)
                );

                Task action = option switch
                {
                    CREATE_LEAGUE => CreateLeagueAsync(standingsYear),
                    DELETE_LEAGUE => DeleteLeagueAsync(standingsYear),
                    IMPORT_LEAGUES => ImportLeaguesAsync(standingsYear),
                    CREATE_OWNER => CreateOwnerAsync(standingsYear),
                    DELETE_OWNER => DeleteOwnerAsync(standingsYear),
                    PROMOTE_OWNER => PromoteOwnerAsync(standingsYear),
                    DEMOTE_OWNER => DemoteOwnerAsync(standingsYear),
                    BACK_TO_MAIN_MENU => ExitPageAsync(),
                    _ => throw new NotImplementedException($"No action implemented for option {option}")
                };
                await action;
            } while (_keepAlive);
        }

        private static void ShowLeaguesTable(StandingsYear standingsYear)
        {
            Table table = new();

            foreach (League league in standingsYear.Leagues.OrderBy(l => l.Rank))
                table.AddColumn(league.Name);

            int mostOwners = standingsYear.Leagues.Count > 0 ? standingsYear.Leagues.Max(league => league.Owners.Count) : 0;
            for (int i = 0; i < mostOwners; i++)
                table.AddRow(standingsYear.Leagues.Select(league => i < league.Owners.Count ? league.Owners[i].ToString() : string.Empty).ToArray());

            AnsiConsole.Write(table);
        }

        private async Task CreateLeagueAsync(StandingsYear standingsYear)
        {
            var name = AnsiConsole.Prompt(new TextPrompt<string>("Naam van de divisie?"));
            var rank = AnsiConsole.Prompt(new TextPrompt<int>("Rang van de divisie?"));

            League league = new()
            {
                Rank = rank,
                Name = name,
            };

            standingsYear.Leagues.Add(league);
            await _standingsStore.SaveAsync(standingsYear);
        }

        private async Task DeleteLeagueAsync(StandingsYear standingsYear)
        {
            League selectedLeague = AnsiConsole.Prompt(
                new SelectionPrompt<League>()
                    .Title("Selecteer een divisie:")
                    .PageSize(10)
                    .MoreChoicesText("[grey](Beweeg omhoog/omlaag om meer divisies te zien.)[/]")
                    .AddChoices(standingsYear.Leagues.OrderBy(l => l.Rank))
                    .EnableSearch()
                    .SearchPlaceholderText("Type om te zoeken."));

            standingsYear.Leagues.Remove(selectedLeague);
            await _standingsStore.SaveAsync(standingsYear);
        }

        private async Task ImportLeaguesAsync(StandingsYear currentStandingsYear)
        {
            int year = AnsiConsole.Prompt(new TextPrompt<int>("Jaar:"));

            StandingsYear oldStandingsYear = await _standingsStore.GetByYearAsync(year);
            currentStandingsYear.Leagues = oldStandingsYear.Leagues;

            await _standingsStore.SaveAsync(currentStandingsYear);
        }

        private async Task CreateOwnerAsync(StandingsYear standingsYear)
        {
            Owner name = new(AnsiConsole.Prompt(new TextPrompt<string>("Naam van de liefhebber?")));

            League selectedLeague = AnsiConsole.Prompt(
                new SelectionPrompt<League>()
                    .Title("Selecteer een divisie:")
                    .PageSize(10)
                    .MoreChoicesText("[grey](Beweeg omhoog/omlaag om meer divisies te zien.)[/]")
                    .AddChoices(standingsYear.Leagues)
                    .EnableSearch()
                    .SearchPlaceholderText("Type om te zoeken."));

            selectedLeague.AddOwner(name);
            await _standingsStore.SaveAsync(standingsYear);
        }

        private async Task DeleteOwnerAsync(StandingsYear standingsYear)
        {
            Owner selectedOwner = AnsiConsole.Prompt(
                new SelectionPrompt<Owner>()
                    .Title("Selecteer een divisie:")
                    .PageSize(10)
                    .MoreChoicesText("[grey](Beweeg omhoog/omlaag om meer divisies te zien.)[/]")
                    .AddChoices(standingsYear.GetAllOwners())
                    .EnableSearch()
                    .SearchPlaceholderText("Type om te zoeken."));

            foreach (League league in standingsYear.Leagues)
                league.RemoveOwner(selectedOwner);

            await _standingsStore.SaveAsync(standingsYear);
        }

        private async Task PromoteOwnerAsync(StandingsYear standingsYear)
        {
            await base.ShowAsync();

            Owner selectedOwner = AnsiConsole.Prompt(
                new SelectionPrompt<Owner>()
                    .Title("Selecteer een liefhebber:")
                    .PageSize(10)
                    .MoreChoicesText("[grey](Beweeg omhoog/omlaag om meer liefhebbers te zien.)[/]")
                    .AddChoices(standingsYear.GetAllOwners())
                    .EnableSearch()
                    .SearchPlaceholderText("Type om te zoeken."));

            League currentLeague = standingsYear.Leagues.First(league => league.Owners.Contains(selectedOwner));
            int index = standingsYear.Leagues.IndexOf(currentLeague);
            if (index == 0)
                return; // Cannot promote beyond first league.

            currentLeague.RemoveOwner(selectedOwner);
            standingsYear.Leagues[index - 1].AddOwner(selectedOwner);

            await _standingsStore.SaveAsync(standingsYear);
        }

        private async Task DemoteOwnerAsync(StandingsYear standingsYear)
        {
            await base.ShowAsync();

            Owner selectedOwner = AnsiConsole.Prompt(
                new SelectionPrompt<Owner>()
                    .Title("Selecteer een liefhebber:")
                    .PageSize(10)
                    .MoreChoicesText("[grey](Beweeg omhoog/omlaag om meer liefhebbers te zien.)[/]")
                    .AddChoices(standingsYear.GetAllOwners())
                    .EnableSearch()
                    .SearchPlaceholderText("Type om te zoeken."));

            League currentLeague = standingsYear.Leagues.First(league => league.Owners.Contains(selectedOwner));
            int index = standingsYear.Leagues.IndexOf(currentLeague);
            if (index == (standingsYear.Leagues.Count - 1))
                return; // Cannot demote beyond last league

            currentLeague.RemoveOwner(selectedOwner);
            standingsYear.Leagues[index + 1].AddOwner(selectedOwner);

            await _standingsStore.SaveAsync(standingsYear);
        }

        private Task ExitPageAsync()
        {
            _keepAlive = false;
            return Task.CompletedTask;
        }
    }
}

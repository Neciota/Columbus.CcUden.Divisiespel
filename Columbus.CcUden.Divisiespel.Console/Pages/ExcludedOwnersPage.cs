using Columbus.CcUden.Divisiespel.Models;
using Columbus.CcUden.Divisiespel.Persistence;
using Spectre.Console;

namespace Columbus.CcUden.Divisiespel.Console.Pages
{
    internal class ExcludedOwnersPage(Router router, YearStore yearStore, StandingsStore standingsStore) : Page(router, yearStore)
    {
        private const string ADD_EXCLUDED_OWNERS = "Uitgesloten liefhebber toevoegen.";
        private const string REMOVE_EXCLUDED_OWNERS = "Uitgesloten liefhebber verwijderen.";
        private const string BACK_TO_MAIN_MENU = "Terug naar hoofdmenu.";

        private readonly StandingsStore _standingsStore = standingsStore;

        private bool _keepAlive = true;

        public override async Task ShowAsync()
        {
            do
            {
                await base.ShowAsync();

                StandingsYear standings = await _standingsStore.GetByYearAsync(_yearStore.Year);

                Table table = new();
                table.AddColumn("Uitgesloten liefhebbers:");

                foreach (string ownerName in standings.ExcludedOwners)
                    table.AddRow(ownerName);

                AnsiConsole.Write(table);

                string option = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .AddChoices(
                            ADD_EXCLUDED_OWNERS,
                            REMOVE_EXCLUDED_OWNERS,
                            BACK_TO_MAIN_MENU)
                );

                Task action = option switch
                {
                    ADD_EXCLUDED_OWNERS => AddNewAsync(standings),
                    REMOVE_EXCLUDED_OWNERS => RemoveAsync(standings),
                    BACK_TO_MAIN_MENU => ExitPageAsync(),
                    _ => throw new NotImplementedException($"No action implemented for option {option}")
                };
                await action;
            }
            while (_keepAlive);
        }

        private async Task AddNewAsync(StandingsYear standings)
        {
            await base.ShowAsync();

            string owner = AnsiConsole.Prompt(
                new TextPrompt<string>("(Uitslag)naam liefhebber?"));

            standings.ExcludedOwners.Add(owner);
            await _standingsStore.SaveAsync(standings);
        }

        private async Task RemoveAsync(StandingsYear standings)
        {
            if (standings.ExcludedOwners.Count == 0)
                return;

            await base.ShowAsync();

            string owner = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Selecteer een liefhebber om weg te verwijderen:")
                    .PageSize(10)
                    .MoreChoicesText("[grey](Beweeg omhoog/omlaag om meer vluchtcodes te zien.)[/]")
                    .AddChoices(standings.ExcludedOwners)
            );

            standings.ExcludedOwners.Remove(owner);
            await _standingsStore.SaveAsync(standings);
        }

        private Task ExitPageAsync()
        {
            _keepAlive = false;
            return Task.CompletedTask;
        }
    }
}

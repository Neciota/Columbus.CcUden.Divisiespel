using Spectre.Console;

namespace Columbus.CcUden.Divisiespel.Console.Pages
{
    internal class HomePage(Router router, YearStore yearStore) : Page(router, yearStore)
    {
        private const string SEE_ADDED_FLIGHTS = "Reeds toegevoegde vluchten bekijken.";
        private const string SEE_CURRENT_STANDINGS = "Huidige stand bekijken.";
        private const string ADD_FLIGHT = "Vlucht toevoegen.";
        private const string EDIT_YEAR = "Jaartal aanpassen.";
        private const string SEE_EXCLUDED_OWNERS = "Uitgesloten liefhebbers bekijken.";
        private const string EXIT = "Afsluiten.";

        private bool _keepAlive = true;

        public override async Task ShowAsync()
        {
            do
            {
                await base.ShowAsync();

                string result = AnsiConsole.Console.Prompt(new SelectionPrompt<string>()
                    .Title("Navigeer naar een pagina:")
                    .PageSize(10)
                    .MoreChoicesText("Beweeg omhoog/omlaag om meer te zien.")
                    .AddChoices(
                        SEE_ADDED_FLIGHTS,
                        SEE_CURRENT_STANDINGS,
                        ADD_FLIGHT,
                        EDIT_YEAR,
                        SEE_EXCLUDED_OWNERS,
                        EXIT
                    ));

                Task navigation = result switch
                {
                    SEE_ADDED_FLIGHTS => _router.NavigateToAsync<ViewAddedFlightsPage>(),
                    SEE_CURRENT_STANDINGS => _router.NavigateToAsync<ViewStandingsPage>(),
                    ADD_FLIGHT => _router.NavigateToAsync<AddFlightPage>(),
                    EDIT_YEAR => _router.NavigateToAsync<EditYearPage>(),
                    SEE_EXCLUDED_OWNERS => _router.NavigateToAsync<ExcludedOwnersPage>(),
                    EXIT => ShutdownAsync(),
                    _ => throw new NotImplementedException($"No implementation for option {result}.")
                };

                await navigation;
            } while (_keepAlive);
        }

        private Task ShutdownAsync()
        {
            _keepAlive = false;
            return Task.CompletedTask;
        }
    }
}

using Spectre.Console;

namespace Columbus.CcUden.Divisiespel.Console.Pages
{
    internal class HomePage(Router router, YearStore yearStore) : Page(router)
    {
        private const string SEE_ADDED_FLIGHTS = "Reeds toegevoegde vluchten bekijken.";
        private const string SEE_CURRENT_STANDINGS = "Huidige stand bekijken.";
        private const string ADD_FLIGHT = "Vlucht toevoegen.";
        private const string EDIT_YEAR = "Jaartal aanpassen.";
        private const string EXIT = "Afsluiten.";

        private readonly YearStore _yearStore = yearStore;

        public override async Task ShowAsync()
        {
            await base.ShowAsync();

            var year = new Panel($"Jaar: {_yearStore.Year}")
                .Border(BoxBorder.Square)
                .Padding(1, 1)
                .Expand();

            AnsiConsole.Write(year);

            string result = AnsiConsole.Console.Prompt(new SelectionPrompt<string>()
                .Title("Navigeer naar een pagina:")
                .PageSize(10)
                .MoreChoicesText("Beweeg omhoog/omlaag om meer te zien.")
                .AddChoices(
                    SEE_ADDED_FLIGHTS,
                    SEE_CURRENT_STANDINGS,
                    ADD_FLIGHT,
                    EDIT_YEAR,
                    EXIT
                ));

            Task navigation = result switch
            {
                SEE_ADDED_FLIGHTS => _router.NavigateToAsync<ViewAddedFlightsPage>(),
                SEE_CURRENT_STANDINGS => _router.NavigateToAsync<ViewStandingsPage>(),
                ADD_FLIGHT => _router.NavigateToAsync<AddFlightPage>(),
                EDIT_YEAR => _router.NavigateToAsync<EditYearPage>(),
                EXIT => Task.CompletedTask,
                _ => throw new NotImplementedException("No implementation for this ")
            };

            await navigation;
        }
    }
}

using Spectre.Console;

namespace Columbus.CcUden.Divisiespel.Console.Pages
{
    internal class EditYearPage(Router router, YearStore yearStore) : Page(router)
    {
        private readonly YearStore _yearStore = yearStore;

        public override async Task ShowAsync()
        {
            await base.ShowAsync();

            int year = AnsiConsole.Prompt(new TextPrompt<int>("Jaar:"));
            _yearStore.Year = year;

            await _router.NavigateToAsync<HomePage>();
        }
    }
}

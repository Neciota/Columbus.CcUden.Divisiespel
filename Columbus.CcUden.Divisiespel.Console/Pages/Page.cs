using Spectre.Console;

namespace Columbus.CcUden.Divisiespel.Console.Pages
{
    internal abstract class Page
    {
        protected readonly Router _router;
        protected readonly YearStore _yearStore;

        protected Page(Router router, YearStore yearStore)
        {
            _router = router;
            _yearStore = yearStore;
        }

        public virtual Task ShowAsync()
        {
            AnsiConsole.Clear();

            var title = new Panel("[bold]CC Uden - Divisiespel[/]")
                .Border(BoxBorder.Square)
                .Padding(1, 1)
                .Expand();

            AnsiConsole.Write(title);

            var year = new Panel($"Jaar: {_yearStore.Year}")
                .Border(BoxBorder.Square)
                .Padding(1, 1)
                .Expand();

            AnsiConsole.Write(year);

            return Task.CompletedTask;
        }
    }
}

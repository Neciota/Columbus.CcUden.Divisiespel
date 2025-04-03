using Spectre.Console;

namespace Columbus.CcUden.Divisiespel.Console.Pages
{
    internal abstract class Page
    {
        protected readonly Router _router;

        protected Page(Router router)
        {
            _router = router;
        }

        public virtual Task ShowAsync()
        {
            AnsiConsole.Clear();

            var title = new Panel("[bold]CC Uden - Divisiespel[/]")
                .Border(BoxBorder.Square)
                .Padding(1, 1)
                .Expand();

            AnsiConsole.Write(title);

            return Task.CompletedTask;
        }
    }
}

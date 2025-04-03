using Columbus.CcUden.Divisiespel.Console.Pages;
using Microsoft.Extensions.DependencyInjection;

namespace Columbus.CcUden.Divisiespel.Console
{
    internal class Router(IServiceProvider serviceProvider)
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;

        public async Task NavigateToAsync<TPage>() where TPage : Page
        {
            TPage page = _serviceProvider.GetRequiredService<TPage>();

            await page.ShowAsync();
        }
    }
}

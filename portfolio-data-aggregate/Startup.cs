using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using portfolio_data_aggregate.Cache;

[assembly: FunctionsStartup(typeof(portfolio_data_aggregate.Startup))]
namespace portfolio_data_aggregate
{
    public class Startup: FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddHttpClient("GitHub", client =>
            {
                client.BaseAddress = new System.Uri("https://api.github.com/repos/");
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Azure Function");
            });

            builder.Services.AddMemoryCache();
            builder.Services.AddSingleton<ILanguageDataCache, MemoryLanguageDataCache>();
        }
    }
}

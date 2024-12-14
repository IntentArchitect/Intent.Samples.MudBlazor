using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using MudBlazor.ExampleApp.Client.HttpClients.Implementations;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.HttpClientConfiguration", Version = "2.0")]

namespace MudBlazor.ExampleApp.Client.HttpClients
{
    public static class HttpClientConfiguration
    {
        public static void AddHttpClients(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddHttpClient<ICustomersService, CustomersServiceHttpClient>(http =>
                {
                    http.BaseAddress = GetUrl(configuration, "MudBlazorExampleApp");
                });

            services
                .AddHttpClient<IInvoiceService, InvoiceServiceHttpClient>(http =>
                {
                    http.BaseAddress = GetUrl(configuration, "MudBlazorExampleApp");
                });

            services
                .AddHttpClient<IProductsService, ProductsServiceHttpClient>(http =>
                {
                    http.BaseAddress = GetUrl(configuration, "MudBlazorExampleApp");
                })
                .AddHttpMessageHandler(sp =>
                {
                    return sp.GetRequiredService<AuthorizationMessageHandler>()
                        .ConfigureHandler(
                            authorizedUrls: new[] { GetUrl(configuration, "MudBlazorExampleApp").AbsoluteUri });
                });
        }

        private static Uri GetUrl(IConfiguration configuration, string applicationName)
        {
            var url = configuration.GetValue<Uri?>($"Urls:{applicationName}");

            return url ?? throw new Exception($"Configuration key \"Urls:{applicationName}\" is not set");
        }
    }
}
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.ProgramTemplate", Version = "1.0")]

namespace MudBlazor.ExampleApp.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            await LoadAppSettings(builder);
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddClientServices(builder.Configuration);
            builder.Services.Configure<AuthApiEndpoints>(builder.Configuration.GetSection("Urls"));

            builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>();
            //builder.Services.AddScoped<CustomAuthenticationStateProvider>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<Func<IAuthService>>(sp => sp.GetRequiredService<IAuthService>);
            builder.Services.AddScoped<IAccessTokenProvider, AccessTokenProvider>();
            
            builder.Services.AddTransient<AuthorizationMessageHandler>();
            builder.Services.AddAuthorizationCore();

            builder.Services.AddMudServices();

            await builder.Build().RunAsync();
        }

        public static async Task LoadAppSettings(WebAssemblyHostBuilder builder)
        {
            var configProxy = new HttpClient() { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
            using var response = await configProxy.GetAsync("appsettings.json");
            using var stream = await response.Content.ReadAsStreamAsync();
            builder.Configuration.AddJsonStream(stream);
        }
    }
    public class AuthApiEndpoints
    {
        public string Login { get; set; }
        public string Refresh { get; set; }
    }
}
using System.Reflection;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor.ExampleApp.Client.Common.Validation;
using MudBlazor.ExampleApp.Client.HttpClients;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.DependencyInjection", Version = "1.0")]

namespace MudBlazor.ExampleApp.Client
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddClientServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly(), lifetime: ServiceLifetime.Transient);
            services.AddHttpClients(configuration);
            services.AddScoped<IValidatorProvider, ValidatorProvider>();
            // [IntentIgnore]
            services.AddScoped<IAuthService, PlaceholderAuthService>();
            return services;
        }
    }

    [IntentIgnore]
    public class PlaceholderAuthService : IAuthService
    {
        public Task Register(string username, string password)
        {
            throw new InvalidOperationException("Register is not available during server-side prerendering.");
        }

        public Task<bool> Login(string username, string password)
        {
            throw new InvalidOperationException("Login is not available during server-side prerendering.");
        }

        public Task Logout()
        {
            throw new InvalidOperationException("Logout is not available during server-side prerendering.");
        }
    }
}
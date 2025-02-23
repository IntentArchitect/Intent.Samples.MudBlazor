using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.JwtAuth.AuthServiceInterface", Version = "1.0")]

namespace MudBlazor.ExampleApp.Client.Common.Auth;

public interface IAuthService
{
    Task Register(string username, string password);
    Task<bool> Login(string username, string password);
    Task Logout();
    Task<string?> GetAccessTokenAsync();
}
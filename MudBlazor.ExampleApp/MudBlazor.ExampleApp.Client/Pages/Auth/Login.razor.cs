using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MudBlazor.ExampleApp.Client.HttpClients;
using System.Net.Http;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.RazorComponentCodeBehindTemplate", Version = "1.0")]

namespace MudBlazor.ExampleApp.Client.Pages.Auth
{
    public partial class Login
    {
        [Inject]
        public IAuthService AuthService { get; set; } = default!;
        [Inject]
        public NavigationManager NavigationManager{ get; set; } = default!;

        [SupplyParameterFromQuery]
        private string? ReturnUrl { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }

        private string errorMessage;
        protected override async Task OnInitializedAsync()
        {
        }

        private async Task OnLoginClicked()
        {
            errorMessage = null;
            var success = await AuthService.Login(Username, Password);
            if (success)
            {
                NavigationManager.NavigateTo(ReturnUrl);
            }
            //else if (result.RequiresTwoFactor)
            //{
            //    RedirectManager.RedirectTo(
            //        "Account/LoginWith2fa",
            //        new() { ["returnUrl"] = ReturnUrl, ["rememberMe"] = Input.RememberMe });
            //}
            //else if (result.IsLockedOut)
            //{
            //    Logger.LogWarning("User account locked out.");
            //    RedirectManager.RedirectTo("Account/Lockout");
            //}
            else
            {
                errorMessage = "Error: Invalid login attempt.";
            }
        }

    }
}
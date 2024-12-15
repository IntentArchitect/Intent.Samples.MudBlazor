using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;
using MudBlazor.ExampleApp.Client.HttpClients;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.RazorComponentCodeBehindTemplate", Version = "1.0")]

namespace MudBlazor.ExampleApp.Client.Pages.Auth
{
    [IntentMerge]
    public partial class Login
    {
        private MudForm _form;
        private bool _onLoginClickedProcessing = false;

        [Inject]
        public IAuthService AuthService { get; set; } = default!;
        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;
        [Inject]
        public ISnackbar Snackbar { get; set; } = default!;

        [SupplyParameterFromQuery]
        private string? ReturnUrl { get; set; }
        public string ErrorMessage { get; set; }

        public LoginModel Model { get; set; } = new();
        protected override async Task OnInitializedAsync()
        {
        }

        [IntentIgnore]
        private async Task OnLoginClicked()
        {
            await _form!.Validate();
            if (!_form.IsValid)
            {
                return;
            }
            var success = await AuthService.Login(Model.Email, Model.Password);
            if (success)
            {
                NavigationManager.NavigateTo(ReturnUrl ?? "/");
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
                Snackbar.Add("Error: Invalid login attempt.", Severity.Error);
            }
        }

    }
}
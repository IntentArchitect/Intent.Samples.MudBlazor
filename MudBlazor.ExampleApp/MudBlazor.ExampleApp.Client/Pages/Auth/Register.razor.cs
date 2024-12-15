using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.RazorComponentCodeBehindTemplate", Version = "1.0")]

namespace MudBlazor.ExampleApp.Client.Pages.Auth
{
    public partial class Register
    {
        [Inject]
        public IAuthService AuthService { get; set; } = default!;
        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;

        public string Email { get; set; }
        public string Password { get; set; }

        protected override async Task OnInitializedAsync()
        {
        }

        private async Task OnRegisterClicked()
        {
            await AuthService.Register(Email, Password);
            NavigationManager.NavigateTo("Auth/Login");
        }
    }
}
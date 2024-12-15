using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.RazorComponentCodeBehindTemplate", Version = "1.0")]

namespace MudBlazor.ExampleApp.Client.Pages.Auth
{
    [IntentMerge]
    public partial class Register
    {
        private MudForm _form;
        private bool _onRegisterClickedProcessing = false;
        public RegisterModel Model { get; set; } = new();
        [Inject]
        public IAuthService AuthService { get; set; } = default!;
        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
        }

        [IntentMerge]
        private async Task OnRegisterClicked()
        {
            await _form!.Validate();
            if (!_form.IsValid)
            {
                return;
            }
            await AuthService.Register(Model.Email, Model.Password);
            NavigationManager.NavigateTo("Auth/Login");
        }
    }
}
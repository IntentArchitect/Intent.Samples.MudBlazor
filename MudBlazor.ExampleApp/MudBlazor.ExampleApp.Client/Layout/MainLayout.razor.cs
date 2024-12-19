using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.RazorLayoutCodeBehindTemplate", Version = "1.0")]

namespace MudBlazor.ExampleApp.Client.Layout
{
    public partial class MainLayout
    {
        private bool _drawerOpen = true;

        [Inject]
        public IAuthService AuthService { get; set; } = default!;

        public void DrawerToggle()
        {
            _drawerOpen = !_drawerOpen;
        }

        private async Task Logout()
        {
            await AuthService.Logout();
        }
    }
}
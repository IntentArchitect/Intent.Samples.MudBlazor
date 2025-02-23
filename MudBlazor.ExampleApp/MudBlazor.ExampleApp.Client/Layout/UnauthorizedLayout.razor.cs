using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.RazorLayoutCodeBehindTemplate", Version = "1.0")]

namespace MudBlazor.ExampleApp.Client.Layout
{
    public partial class UnauthorizedLayout
    {
    }
}
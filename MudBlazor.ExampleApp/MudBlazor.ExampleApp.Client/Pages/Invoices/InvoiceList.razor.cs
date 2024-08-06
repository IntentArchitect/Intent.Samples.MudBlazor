using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.RazorComponentCodeBehindTemplate", Version = "1.0")]

namespace MudBlazor.ExampleApp.Client.Pages.Invoices
{
    public partial class InvoiceList
    {
        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;
        protected override async Task OnInitializedAsync()
        {
        }

        private void AddInvoiceClick()
        {
            NavigationManager.NavigateTo("/invoices/invoice-add");
        }
    }
}
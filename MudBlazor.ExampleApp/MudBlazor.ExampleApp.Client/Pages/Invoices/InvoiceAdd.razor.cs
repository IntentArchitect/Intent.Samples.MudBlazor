using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;
using MudBlazor.ExampleApp.Client.HttpClients;
using MudBlazor.ExampleApp.Client.HttpClients.Contracts.Services.Customers;
using MudBlazor.ExampleApp.Client.HttpClients.Contracts.Services.Invoices;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.RazorComponentCodeBehindTemplate", Version = "1.0")]

namespace MudBlazor.ExampleApp.Client.Pages.Invoices
{
    public partial class InvoiceAdd
    {
        private MudForm _form;
        private bool _onSaveClickedProcessing = false;
        public InvoiceDto Model { get; set; } = new();
        public List<CustomerLookupDto> CustomerIdOptions { get; set; } = [];
        [Inject]
        public ICustomersService CustomersService { get; set; } = default!;
        [Inject]
        public ISnackbar Snackbar { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                CustomerIdOptions = await CustomersService.GetCustomersLookupAsync();
            }
            catch (Exception e)
            {
                Snackbar.Add(e.Message, Severity.Error);
            }
            finally
            {
            }
        }

        private async Task OnSaveClicked()
        {
            await _form!.Validate();
            if (!_form.IsValid)
            {
                return;
            }
        }

        private void OnCancelClicked()
        {
        }
    }
}
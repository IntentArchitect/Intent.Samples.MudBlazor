using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;
using MudBlazor.ExampleApp.Client.HttpClients;
using MudBlazor.ExampleApp.Client.HttpClients.Contracts.Services.Customers;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.RazorComponentCodeBehindTemplate", Version = "1.0")]

namespace MudBlazor.ExampleApp.Client.Pages.Customers
{
    public partial class CustomerAdd
    {
        private MudForm _form;
        private bool _onSaveClickedProcessing = false;
        public CustomerDto Model { get; set; } = new();
        [Inject]
        public ICustomersService CustomersService { get; set; } = default!;
        [Inject]
        public ISnackbar Snackbar { get; set; } = default!;
        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;

        private async Task OnInitializeAsync()
        {
        }

        private async Task OnSaveClicked()
        {
            try
            {
                _onSaveClickedProcessing = true;
                await _form!.Validate();
                if (!_form.IsValid)
                {
                    return;
                }
                await CustomersService.CreateCustomerAsync(new CreateCustomerCommand
                {
                    Name = Model.Name,
                    AccountNo = Model.AccountNo,
                    Address = new CreateCustomerAddressDto
                    {
                        Line1 = Model.AddressLine1,
                        Line2 = Model.AddressLine2,
                        City = Model.AddressCity,
                        Country = Model.AddressCountry,
                        Postal = Model.AddressPostal
                    }
                });
                NavigationManager.NavigateTo("/customers");
            }
            catch (Exception e)
            {
                Snackbar.Add(e.Message, Severity.Error);
            }
            finally
            {
                _onSaveClickedProcessing = false;
            }
        }

        private void OnCancelClicked()
        {
            NavigationManager.NavigateTo("/customers");
        }
    }
}
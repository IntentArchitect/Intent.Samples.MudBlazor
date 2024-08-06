using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;
using MudBlazor.ExampleApp.Client.HttpClients;
using MudBlazor.ExampleApp.Client.HttpClients.Contracts.Services.Customers;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.RazorComponentCodeBehindTemplate", Version = "1.0")]

namespace MudBlazor.ExampleApp.Client.Pages.Customers
{
    public partial class CustomerEdit
    {
        private MudForm _form;
        private bool _onSaveClickedProcessing = false;
        private bool _onDeleteClickedProcessing = false;
        [Parameter]
        public Guid CustomerId { get; set; }
        public CustomerDto? Model { get; set; }
        [Inject]
        public ICustomersService CustomersService { get; set; } = default!;
        [Inject]
        public ISnackbar Snackbar { get; set; } = default!;
        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;
        [Inject]
        public IDialogService DialogService { get; set; } = default;
        protected override async Task OnInitializedAsync()
        {
            try
            {
                Model = await CustomersService.GetCustomerByIdAsync(CustomerId);
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
            try
            {
                _onSaveClickedProcessing = true;
                await _form!.Validate();
                if (!_form.IsValid)
                {
                    return;
                }
                await CustomersService.UpdateCustomerAsync(CustomerId, new UpdateCustomerCommand
                {
                    Id = Model.Id,
                    Name = Model.Name,
                    AccountNo = Model?.AccountNo,
                    Address = new UpdateCustomerAddressDto
                    {
                        Line1 = Model.AddressLine1,
                        Line2 = Model?.AddressLine2,
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

        private async Task OnDeleteClicked()
        {
            try
            {
                var parameters = new DialogParameters<ConfirmationDialog>
                {
                    { x => x.ContentText, "Do you really want to delete these records? This process cannot be undone." },
                    { x => x.ButtonText, "Delete" },
                    { x => x.Color, Color.Error }
                };

                var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall };

                var result = await DialogService.ShowAsync<ConfirmationDialog>("Delete", parameters, options);

                if ((await result.Result).Data as bool? != true)
                {
                    return;
                }
                _onDeleteClickedProcessing = true;
                await CustomersService.DeleteCustomerAsync(CustomerId);
            }
            catch (Exception e)
            {
                Snackbar.Add(e.Message, Severity.Error);
            }
            finally
            {
                _onDeleteClickedProcessing = false;
            }

        }

        private void OnCancelClicked()
        {
            NavigationManager.NavigateTo("/customers");
        }
    }
}
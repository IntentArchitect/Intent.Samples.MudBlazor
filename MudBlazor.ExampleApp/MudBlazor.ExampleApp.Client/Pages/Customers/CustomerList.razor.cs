using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;
using MudBlazor.ExampleApp.Client.HttpClients;
using MudBlazor.ExampleApp.Client.HttpClients.Common;
using MudBlazor.ExampleApp.Client.HttpClients.Contracts.Services.Customers;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.RazorComponentCodeBehindTemplate", Version = "1.0")]

namespace MudBlazor.ExampleApp.Client.Pages.Customers
{
    public partial class CustomerList
    {
        public PagedResult<CustomerDto>? Model { get; set; }
        [Inject]
        public ICustomersService CustomersService { get; set; } = default!;
        [Inject]
        public ISnackbar Snackbar { get; set; } = default!;
        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;

        private async Task FetchDataGridData(int pageNo, int pageSize, string sorting)
        {
            try
            {
                Model = await CustomersService.GetCustomersAsync(
                    pageNo,
                    pageSize,
                    sorting);
            }
            catch (Exception e)
            {
                Snackbar.Add(e.Message, Severity.Error);
            }
            finally
            {
            }
        }

        private void DataGridRowClick(string rowId)
        {
            NavigationManager.NavigateTo($"/customers/{rowId}");
        }

        private void AddCustomerClick()
        {
            NavigationManager.NavigateTo("/customers/add");
        }

        private async Task<GridData<CustomerDto>> LoadDataGridData(GridState<CustomerDto> state)
        {
            var pageNo = state.Page + 1;
            var pageSize = state.PageSize;
            var sorting = string.Join(", ", state.SortDefinitions.Select(x => $"{x.SortBy} {(x.Descending ? "desc" : "asc")}"));
            await FetchDataGridData(pageNo, pageSize, sorting);
            return new GridData<CustomerDto>() { TotalItems = Model.TotalCount, Items = Model.Data };
        }
    }
}
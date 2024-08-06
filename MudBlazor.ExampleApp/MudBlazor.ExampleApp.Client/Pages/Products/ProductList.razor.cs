using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;
using MudBlazor.ExampleApp.Client.HttpClients;
using MudBlazor.ExampleApp.Client.HttpClients.Common;
using MudBlazor.ExampleApp.Client.HttpClients.Contracts.Services.Products;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.RazorComponentCodeBehindTemplate", Version = "1.0")]

namespace MudBlazor.ExampleApp.Client.Pages.Products
{
    public partial class ProductList
    {
        public PagedResult<ProductDto>? Model { get; set; }
        [Inject]
        public IProductsService ProductsService { get; set; } = default!;
        [Inject]
        public ISnackbar Snackbar { get; set; } = default!;
        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;

        private async Task FetchDataGridData(int pageNo, int pageSize, string sorting)
        {
            try
            {
                Model = await ProductsService.GetProductsAsync(
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

        private void OnRowClick(string rowId)
        {
            NavigationManager.NavigateTo($"/products/{rowId}");
        }

        private void AddProductClick()
        {
            NavigationManager.NavigateTo("/products/add");
        }

        private async Task<GridData<ProductDto>> LoadDataGridData(GridState<ProductDto> state)
        {
            var pageNo = state.Page + 1;
            var pageSize = state.PageSize;
            var sorting = string.Join(", ", state.SortDefinitions.Select(x => $"{x.SortBy} {(x.Descending ? "desc" : "asc")}"));
            await FetchDataGridData(pageNo, pageSize, sorting);
            return new GridData<ProductDto>() { TotalItems = Model.TotalCount, Items = Model.Data };
        }
    }
}
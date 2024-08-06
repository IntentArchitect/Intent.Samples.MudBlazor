using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;
using MudBlazor.ExampleApp.Client.HttpClients;
using MudBlazor.ExampleApp.Client.HttpClients.Contracts.Services.Products;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.RazorComponentCodeBehindTemplate", Version = "1.0")]

namespace MudBlazor.ExampleApp.Client.Pages.Products
{
    public partial class ProductEdit
    {
        private MudForm _form;
        private bool _onSaveClickedProcessing = false;
        [Parameter]
        public Guid ProductId { get; set; }
        public ProductDto? Model { get; set; }
        [Inject]
        public IProductsService ProductsService { get; set; } = default!;
        [Inject]
        public ISnackbar Snackbar { get; set; } = default!;
        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                Model = await ProductsService.GetProductByIdAsync(ProductId);
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
                await ProductsService.UpdateProductAsync(ProductId, new UpdateProductCommand
                {
                    Name = Model.Name,
                    Description = Model.Description,
                    Price = Model.Price,
                    ImageUrl = Model?.ImageUrl,
                    Id = Model.Id
                });
                NavigationManager.NavigateTo("/products");
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
            NavigationManager.NavigateTo("/products");
        }
    }
}
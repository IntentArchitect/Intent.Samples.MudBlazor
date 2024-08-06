using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.Dtos.FluentValidation.DtoValidator", Version = "2.0")]

namespace MudBlazor.ExampleApp.Client.HttpClients.Contracts.Services.Customers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateCustomerAddressDtoValidator : AbstractValidator<UpdateCustomerAddressDto>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateCustomerAddressDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Line1)
                .NotNull();

            RuleFor(v => v.City)
                .NotNull();

            RuleFor(v => v.Country)
                .NotNull();

            RuleFor(v => v.Postal)
                .NotNull();
        }
    }
}
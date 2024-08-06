using System;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using MudBlazor.ExampleApp.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace MudBlazor.ExampleApp.Application.Invoices.GetInvoiceById
{
    public class GetInvoiceByIdQuery : IRequest<InvoiceDto>, IQuery
    {
        public GetInvoiceByIdQuery(Guid id, int pageNo, int pageSize, string? orderBy)
        {
            Id = id;
            PageNo = pageNo;
            PageSize = pageSize;
            OrderBy = orderBy;
        }

        public Guid Id { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public string? OrderBy { get; set; }
    }
}
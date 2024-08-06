using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using MudBlazor.ExampleApp.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace MudBlazor.ExampleApp.Domain.Entities
{
    public class Invoice : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public string InvoiceNo { get; set; }

        public DateTime IssuedDate { get; set; }

        public DateTime DueDate { get; set; }

        public string? Reference { get; set; }

        public Guid CustomerId { get; set; }

        public virtual ICollection<InvoiceLine> OrderLines { get; set; } = new List<InvoiceLine>();

        public virtual Customer Customer { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}
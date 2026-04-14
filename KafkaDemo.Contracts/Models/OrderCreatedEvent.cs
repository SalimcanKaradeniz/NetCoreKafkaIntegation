using System;
using System.Collections.Generic;
using System.Text;

namespace KafkaDemo.Contracts.Models
{
    public record OrderCreatedEvent(
            Guid OrderId,
            string CustomerId,
            decimal Amount,
            DateTime CreatedAt
        );
}

using System;
using System.Collections.Generic;
using CQRSTutorial.Core;

namespace Cafe.Domain.Events
{
    public class FoodServed : IEvent
    {
        public Guid Id { get; set; }
        public Guid AggregateId { get; set; }
        public Guid CommandId { get; set; }

        public List<int> MenuNumbers;
    }
}
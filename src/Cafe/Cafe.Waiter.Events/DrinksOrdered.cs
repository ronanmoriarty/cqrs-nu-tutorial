﻿using System;
using System.Collections.Generic;
using Cafe.Waiter.Contracts.Commands;
using CQSplit.Core;

namespace Cafe.Waiter.Events
{
    public class DrinksOrdered : IEvent
    {
        public Guid Id { get; set; }
        public Guid AggregateId { get; set; }
        public Guid CommandId { get; set; }
        public List<OrderedItem> Items { get; set; }
    }
}

﻿using System;
using System.Collections.Generic;
using Cafe.Waiter.Contracts.Commands;

namespace Cafe.Waiter.Commands
{
    public class MarkFoodServedCommand : IMarkFoodServedCommand
    {
        public Guid Id { get; set; }
        public Guid AggregateId { get; set; }
        public List<int> MenuNumbers { get; set; }
    }
}
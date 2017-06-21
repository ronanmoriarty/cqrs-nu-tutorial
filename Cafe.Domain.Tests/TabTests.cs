﻿using System;
using System.Collections.Generic;
using Cafe.Domain.Commands;
using Cafe.Domain.Events;
using Cafe.Domain.Exceptions;
using NUnit.Framework;

namespace Cafe.Domain.Tests
{
    [TestFixture]
    public class TabTests : EventTestsBase<Tab>
    {
        private readonly Guid _tabId = new Guid("91EBA94D-3A5F-45FD-BEC4-712E631C732C");
        private readonly int _tableNumber = 123;
        private readonly string _waiter = "John Smith";
        private const decimal FoodPrice = 12m;
        private const int FoodMenuNumber = 12;
        private const string FoodDescription = "Tikka Masala";
        private const decimal DrinkPrice = 2m;
        private const int DrinkMenuNumber = 13;
        private const string DrinkDescription = "Coca Cola";

        [Test]
        public void CanOpenANewTab()
        {
            When(new OpenTab
            {
                TabId = _tabId,
                TableNumber = _tableNumber,
                Waiter = _waiter
            });

            Then(new TabOpened
            {
                Id = _tabId,
                TableNumber = _tableNumber,
                Waiter = _waiter
            });
        }

        [Test]
        public void CannotOrderWhenTabHasNotBeenOpenedYet()
        {
            Assert.That(() =>
                When(new PlaceOrder { TabId = _tabId, Items = null }),
                Throws.Exception.InstanceOf<TabNotOpen>());
        }

        [Test]
        public void CanOrderFoodWhenTabHasAlreadyBeenOpened()
        {
            var foodOrderedItem = GetFoodOrderedItem();
            var orderedItems = new List<OrderedItem> { foodOrderedItem };

            Given(new OpenTab
            {
                TabId = _tabId,
                TableNumber = _tableNumber,
                Waiter = _waiter
            });

            When(new PlaceOrder
                {
                    TabId = _tabId,
                    Items = orderedItems
                }
            );

            Then(new FoodOrdered
            {
                Id = _tabId,
                Items = orderedItems
            });
        }

        [Test]
        public void CanOrderDrinksWhenTabHasAlreadyBeenOpened()
        {
            var drinksOrderedItem = GetDrinkOrderedItem();
            var orderedItems = new List<OrderedItem> { drinksOrderedItem };

            Given(new OpenTab
            {
                TabId = _tabId,
                TableNumber = _tableNumber,
                Waiter = _waiter
            });

            When(new PlaceOrder
                {
                    TabId = _tabId,
                    Items = orderedItems
                }
            );

            Then(new DrinksOrdered
            {
                Id = _tabId,
                Items = orderedItems
            });
        }

        [Test]
        public void CanOrderFoodAndDrinksWhenTabHasAlreadyBeenOpened()
        {
            var foodOrderedItem = GetFoodOrderedItem();
            var drinksOrderedItem = GetDrinkOrderedItem();
            var orderedItems = new List<OrderedItem> {foodOrderedItem, drinksOrderedItem};

            Given(new OpenTab
            {
                TabId = _tabId,
                TableNumber = _tableNumber,
                Waiter = _waiter
            });

            When(new PlaceOrder
                {
                    TabId = _tabId,
                    Items = orderedItems
                }
            );

            Then(new DrinksOrdered
                {
                    Id = _tabId,
                    Items = new List<OrderedItem> {drinksOrderedItem}
                },
                new FoodOrdered
                {
                    Id = _tabId,
                    Items = new List<OrderedItem> { foodOrderedItem }
                }
            );
        }

        private OrderedItem GetFoodOrderedItem()
        {
            return new OrderedItem
            {
                Description = FoodDescription,
                IsDrink = false,
                MenuNumber = FoodMenuNumber,
                Price = FoodPrice
            };
        }

        private OrderedItem GetDrinkOrderedItem()
        {
            return new OrderedItem
            {
                Description = DrinkDescription,
                IsDrink = true,
                MenuNumber = DrinkMenuNumber,
                Price = DrinkPrice
            };
        }
    }
}

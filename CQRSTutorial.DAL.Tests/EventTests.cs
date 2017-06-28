﻿using Cafe.Domain.Events;
using NUnit.Framework;

namespace CQRSTutorial.DAL.Tests
{
    [TestFixture, Category(TestConstants.Integration)]
    public class EventTests
    {
        private IEventRepository _eventRepository;
        private const string Waiter = "John";
        private const int TableNumber = 123;

        [SetUp]
        public void SetUp()
        {
            _eventRepository = new EventRepository(SessionFactory.Instance);
        }

        [Test]
        public void InsertAndRead()
        {
            var tabOpened = new TabOpened
            {
                TableNumber = TableNumber,
                Waiter = Waiter
            };

            _eventRepository.Add(tabOpened);

            var retrievedEvent = _eventRepository.Read(tabOpened.Id);

            Assert.That(retrievedEvent is TabOpened);
            var retrievedTabOpenedEvent = (TabOpened) retrievedEvent;
            Assert.That(retrievedTabOpenedEvent.Id, Is.EqualTo(tabOpened.Id));
            Assert.That(retrievedTabOpenedEvent.TableNumber, Is.EqualTo(TableNumber));
            Assert.That(retrievedTabOpenedEvent.Waiter, Is.EqualTo(Waiter));
        }
    }
}

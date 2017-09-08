﻿using System;
using Cafe.Waiter.Events;
using CQRSTutorial.Core;
using CQRSTutorial.DAL.Tests.Common;
using NUnit.Framework;

namespace Cafe.Waiter.Service.Tests
{
    [TestFixture]
    public class EventHandlerTests
    {
        private readonly Guid _id = new Guid("6565A114-E911-4153-A15E-38361FB52C00");
        private readonly Guid _aggregateId = new Guid("48C24556-4F62-4F8D-9751-1437BAA1159A");
        private readonly Guid _commandId = new Guid("C168EA4F-E956-4A46-8571-371CC5639868");
        private readonly int _tableNumber = 123;
        private readonly string _waiter = "Joe";
        private readonly SqlExecutor _sqlExecutor = new SqlExecutor(WriteModelConnectionStringProviderFactory.Instance);

        [SetUp]
        public void SetUp()
        {
            DeleteAnyExistingRowsWithSameIds();
            var eventHandler = GetEventHandler();

            WhenEventHandled(eventHandler);
        }

        [Test]
        public void When_handling_events_it_saves_to_EventStore()
        {
            AssertThatEventSavedInEventStore();
        }

        [Test]
        public void When_handling_events_it_saves_to_EventsToPublish_table()
        {
            AssertThatEventWillBePublished();
        }

        private void DeleteAnyExistingRowsWithSameIds()
        {
            _sqlExecutor.ExecuteNonQuery($"DELETE FROM dbo.Events WHERE Id = '{_id}'");
            _sqlExecutor.ExecuteNonQuery($"DELETE FROM dbo.EventsToPublish WHERE Id = '{_id}'");
        }

        private static IEventHandler GetEventHandler()
        {
            return Container.Instance.Resolve<IEventHandler>();
        }

        private void WhenEventHandled(IEventHandler eventHandler)
        {
            eventHandler.Handle(new IEvent[]
            {
                new TabOpened
                {
                    Id = _id,
                    AggregateId = _aggregateId,
                    CommandId = _commandId,
                    TableNumber = _tableNumber,
                    Waiter = _waiter
                }
            });
        }

        private void AssertThatEventSavedInEventStore()
        {
            var numberOfEventsWithCurrentId =
                _sqlExecutor.ExecuteScalar<int>($"SELECT COUNT(*) FROM dbo.Events WHERE Id = '{_id}'");
            Assert.That(numberOfEventsWithCurrentId, Is.EqualTo(1));
        }

        private void AssertThatEventWillBePublished()
        {
            var numberOfEventsToPublishWithCurrentId =
                _sqlExecutor.ExecuteScalar<int>($"SELECT COUNT(*) FROM dbo.EventsToPublish WHERE Id = '{_id}'");
            Assert.That(numberOfEventsToPublishWithCurrentId, Is.EqualTo(1));
        }
    }
}

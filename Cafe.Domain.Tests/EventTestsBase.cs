﻿using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute;
using NUnit.Framework;

namespace Cafe.Domain.Tests
{
    public abstract class EventTestsBase<TCommandHandler, TCommand>
        where TCommandHandler : ICommandHandler<TCommand>
    {
        private readonly TCommandHandler _commandHandler;
        private readonly IEventPublisher _eventPublisher;

        protected EventTestsBase()
        {
            _eventPublisher = Substitute.For<IEventPublisher>();
            // ReSharper disable once VirtualMemberCallInConstructor
            _commandHandler = CreateCommandHandler(_eventPublisher);
        }

        protected abstract TCommandHandler CreateCommandHandler(IEventPublisher eventPublisher);

        protected void WhenCommandReceived(TCommand command)
        {
            _commandHandler.Handle(command);
        }

        protected void AssertEventPublished<TEvent>(Func<TEvent, bool> matchCriteria)
        {
            _eventPublisher.Received(1).Publish(Arg.Is<IEnumerable<Event>>(events => AtLeastOneEventMatchesCriteria(matchCriteria, events)));
        }

        private bool AtLeastOneEventMatchesCriteria<TEvent>(Func<TEvent, bool> matchCriteria, IEnumerable<Event> events)
        {
            var allEventsOfMatchingType = events.Where(evt => evt is TEvent).Cast<TEvent>();
            var listOfAllEventsOfMatchingType = allEventsOfMatchingType as IList<TEvent> ?? allEventsOfMatchingType.ToList();
            Assert.IsTrue(listOfAllEventsOfMatchingType.Count > 0, $"No events of type {typeof(TEvent).FullName} received.");
            return listOfAllEventsOfMatchingType.Any(matchCriteria);
        }
    }
}
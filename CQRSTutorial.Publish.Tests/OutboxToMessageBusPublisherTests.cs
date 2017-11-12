﻿using System;
using System.Collections.Generic;
using System.Linq;
using CQRSTutorial.DAL;
using MassTransit;
using NSubstitute;
using NUnit.Framework;

namespace CQRSTutorial.Publish.Tests
{
    [TestFixture]
    public class OutboxToMessageBusPublisherTests
    {
        private IEventToPublishRepository _eventToPublishRepository;
        private TestEvent _event;
        private IEventToPublishSerializer _eventToPublishSerializer;
        private IBusControl _busControl;
        private OutboxToMessageBusPublisher _outboxToMessageBusPublisher;
        private EventToPublish _eventToPublish;
        private IUnitOfWorkFactory _unitOfWorkFactory;
        private IUnitOfWork _unitOfWork;
        private bool _invokingActionInTransaction;
        private bool _eventToPublishDeletedInTransaction;

        [SetUp]
        public void SetUp()
        {
            // TODO: don't like all the mocking and stubbing - very brittle. Consider rewriting integrating with in-memory masstransit bus and db (wasn't sure about how to capture the transactional bit though in an integration test).
            _event = new TestEvent();
            _eventToPublish = new EventToPublish();
            SetupEventToPublishSerializer();
            AssumingThereIsOneEventToPublish();
            NoteWhenEventToPublishBeingDeletedAsPartOfTransaction();
            _busControl = Substitute.For<IBusControl>();
            SetUpUnitOfWork();
            SetUpUnitOfWorkFactory();
            _outboxToMessageBusPublisher = new OutboxToMessageBusPublisher(
                _eventToPublishRepository,
                _busControl,
                _eventToPublishSerializer,
                _unitOfWorkFactory);

            _outboxToMessageBusPublisher.PublishQueuedMessages();
        }

        private void SetupEventToPublishSerializer()
        {
            _eventToPublishSerializer = Substitute.For<IEventToPublishSerializer>();
            _eventToPublishSerializer.Deserialize(_eventToPublish).Returns(_event);
        }

        private void AssumingThereIsOneEventToPublish()
        {
            _eventToPublishRepository = Substitute.For<IEventToPublishRepository>();
            _eventToPublishRepository
                .GetEventsAwaitingPublishing()
                .Returns(new List<EventToPublish> {_eventToPublish});
        }

        private void NoteWhenEventToPublishBeingDeletedAsPartOfTransaction()
        {
            _eventToPublishRepository.When(x => x.Delete(Arg.Is(_eventToPublish))).Do(callInfo =>
            {
                if (_invokingActionInTransaction)
                {
                    _eventToPublishDeletedInTransaction = true;
                }
            });
        }

        private void SetUpUnitOfWork()
        {
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _unitOfWork.Enrolling(Arg.Is(_eventToPublishRepository)).Returns(_unitOfWork);
            _unitOfWork.When(x => x.ExecuteInTransaction(Arg.Any<Action>())).Do(callInfo =>
            {
                _invokingActionInTransaction = true;
                var actionToInvokeInTransaction = (Action) callInfo.Args().First();
                actionToInvokeInTransaction.Invoke();
                _invokingActionInTransaction = false;
            });
        }

        private void SetUpUnitOfWorkFactory()
        {
            _unitOfWorkFactory = Substitute.For<IUnitOfWorkFactory>();
            _unitOfWorkFactory.Create().Returns(_unitOfWork);
        }

        [Test]
        public void Publishes_event_to_message_bus()
        {
            _busControl.Received(1).Publish(_event);
        }

        [Test]
        public void Removes_published_event_from_eventsToPublish_list()
        {
            Assert.That(_eventToPublishDeletedInTransaction);
        }
    }
}

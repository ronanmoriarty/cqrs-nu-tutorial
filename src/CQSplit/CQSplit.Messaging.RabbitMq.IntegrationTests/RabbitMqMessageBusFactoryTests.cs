﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CQSplit.Messaging.Tests.Common;
using MassTransit;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace CQSplit.Messaging.RabbitMq.IntegrationTests
{
    [TestFixture]
    [Category("Integration")]
    public class RabbitMqMessageBusFactoryTests
    {
        private RabbitMqMessageBusFactory _rabbitMqMessageBusFactory;
        private RabbitMqSendEndpointProvider _rabbitMqSendEndpointProvider;
        private IBusControl _busControl;
        private ManualResetEvent _manualResetEvent;
        private Consumer<TestMessage> _consumer;
        private const string QueueName = "RabbitMqMessageBusFactoryTests_Queue";

        [SetUp]
        public void SetUp()
        {
            _manualResetEvent = new ManualResetEvent(true);
            _consumer = new Consumer<TestMessage>(_manualResetEvent);
            IConsumer[] consumers = { _consumer };

            var configurationRoot = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            _rabbitMqMessageBusFactory = new RabbitMqMessageBusFactory(
                new RabbitMqHostConfiguration(configurationRoot),
                new RabbitMqReceiveEndpointConfigurator(
                    new ConsumerRegistrar(
                        new PreviouslyConstructedConsumerFactory(consumers),
                        new ConsumerTypeProvider(typeof(Consumer<TestMessage>)),
                        new ReceiveEndpointConfiguration(QueueName)
                    )
                )
            );

            _busControl = _rabbitMqMessageBusFactory.Create();
            _rabbitMqSendEndpointProvider = new RabbitMqSendEndpointProvider(_busControl, new RabbitMqHostConfiguration(configurationRoot));
        }

        [Test]
        public async Task Sent_messages_are_received()
        {
            var sendEndpoint = await _rabbitMqSendEndpointProvider.GetSendEndpoint(QueueName);

            var id = Guid.NewGuid();
            await sendEndpoint.Send(new TestMessage { Id = id });

            _manualResetEvent.WaitOne(TimeSpan.FromSeconds(5));

            Assert.That(_consumer.ReceivedMessage);
            Assert.That(_consumer.ReceivedMessages.Single(message => message.Id == id), Is.Not.Null);
        }

        public class TestMessage
        {
            public Guid Id { get; set; }
        }
    }

}

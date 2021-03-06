﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cafe.Waiter.Commands;
using Cafe.Waiter.Contracts.Commands;
using Cafe.Waiter.Web.Models;
using CQSplit.Messaging;
using MassTransit;
using NSubstitute;
using NUnit.Framework;
using ISendEndpointProvider = CQSplit.Messaging.ISendEndpointProvider;

namespace Cafe.Waiter.Web.Tests.Controllers.TabController
{
    [TestFixture]
    public class When_creating_tab
    {
        private const string Waiter = "John";
        private const int TableNumber = 5;
        private Web.Controllers.TabController _tabController;
        private ISendEndpoint _sendEndpoint;
        private ICommandSender _commandSender;
        private ISendEndpointProvider _sendEndpointProvider;
        private CreateTabModel _model;
        private ICommandSendConfiguration _commandSendConfiguration;
        private const string CommandServiceQueueName = "cafe.waiter.command.service";

        [SetUp]
        public void SetUp()
        {
            _model = new CreateTabModel
            {
                Waiter = Waiter,
                TableNumber = TableNumber
            };

            _sendEndpoint = Substitute.For<ISendEndpoint>();
            _sendEndpointProvider = Substitute.For<ISendEndpointProvider>();
            _sendEndpointProvider.GetSendEndpoint(Arg.Is<string>(queueName => queueName == CommandServiceQueueName)).Returns(Task.FromResult(_sendEndpoint));
            _commandSendConfiguration = Substitute.For<ICommandSendConfiguration>();
            _commandSendConfiguration.QueueName.Returns(CommandServiceQueueName);
            _commandSender = new CommandSender(_sendEndpointProvider, _commandSendConfiguration);
        }

        [Test]
        public async Task OpenTab_command_sent_to_message_bus_with_ids_set()
        {
            await WhenTabCreated();
            await _sendEndpoint.Received().Send(Arg.Is<IOpenTabCommand>(command => PropertiesMatch(command))); // don't care too much about other values (TableNumber and waiter name) at the moment - happy setting them to arbitrary values for display purposes - no need to assert them.
        }

        private async Task WhenTabCreated()
        {
            _tabController = CreateTabController();
            await _tabController.Create(_model);
        }

        private Web.Controllers.TabController CreateTabController()
        {
            return new Web.Controllers.TabController(null, null,_commandSender, null);
        }

        private bool PropertiesMatch(IOpenTabCommand command)
        {
            return command.Id != Guid.Empty
                && command.AggregateId != Guid.Empty
                && command.Waiter == Waiter
                && command.TableNumber == TableNumber;
        }

        private Guid GetTabId()
        {
            var receivedCalls = _sendEndpoint.ReceivedCalls();
            var sendCall = receivedCalls.Single(call => call.GetMethodInfo().Name == "Send");
            var arguments = sendCall.GetArguments();
            Assert.That(arguments[0] is IOpenTabCommand);
            Assert.That(arguments[1] is CancellationToken);
            var openTabCommand = (OpenTabCommand)arguments.First();
            return openTabCommand.AggregateId;
        }
    }
}

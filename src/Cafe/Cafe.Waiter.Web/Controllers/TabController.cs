﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cafe.Waiter.Commands;
using Cafe.Waiter.Queries.DAL.Models;
using Cafe.Waiter.Web.Models;
using Cafe.Waiter.Web.Repositories;
using CQSplit.Messaging;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace Cafe.Waiter.Web.Controllers
{
    [Route("api/[controller]")]
    public class TabController : Controller
    {
        private readonly ITabDetailsRepository _tabDetailsRepository;
        private readonly IOpenTabsRepository _openTabsRepository;
        private readonly ICommandSender _commandSender;
        private readonly IPlaceOrderCommandFactory _placeOrderCommandFactory;
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public TabController(ITabDetailsRepository tabDetailsRepository,
            IOpenTabsRepository openTabsRepository,
            ICommandSender commandSender,
            IPlaceOrderCommandFactory placeOrderCommandFactory)
        {
            _tabDetailsRepository = tabDetailsRepository;
            _openTabsRepository = openTabsRepository;
            _commandSender = commandSender;
            _placeOrderCommandFactory = placeOrderCommandFactory;
        }

        // GET api/tab/5
        [HttpGet("{tabId}")]
        public TabDetails Get(Guid tabId)
        {
            return _tabDetailsRepository.GetTabDetails(tabId);
        }

        // GET api/tab
        [HttpGet]
        public IEnumerable<OpenTab> Get()
        {
            return _openTabsRepository.GetOpenTabs();
        }

        [HttpPost]
        [Route("PlaceOrder")]
        public void PlaceOrder(TabDetails tabDetails)
        {
            var placeOrderCommand = _placeOrderCommandFactory.Create(tabDetails);
            _commandSender.Send(placeOrderCommand);
        }

        [HttpPost]
        [Route("Create")]
        public async Task Create([FromBody]CreateTabModel model)
        {
            var openTabCommand = CreateOpenTabCommand(model);
            _logger.Debug($"Sending {nameof(OpenTabCommand)} command [Id {openTabCommand.Id} for aggregate {openTabCommand.AggregateId}]");
            await _commandSender.Send(openTabCommand);
        }

        private OpenTabCommand CreateOpenTabCommand(CreateTabModel model)
        {
            return new OpenTabCommand
            {
                Id = Guid.NewGuid(),
                AggregateId = Guid.NewGuid(),
                Waiter = model.Waiter,
                TableNumber = model.TableNumber
            };
        }
    }
}
﻿using Cafe.Waiter.Events;
using Cafe.Waiter.Queries.DAL.Models;
using Cafe.Waiter.Queries.DAL.Repositories;

namespace Cafe.Waiter.EventProjecting.Service.Projectors
{
    public class TabOpenedProjector : ITabOpenedProjector
    {
        private readonly IOpenTabsRepository _openTabsRepository;

        public TabOpenedProjector(IOpenTabsRepository openTabsRepository)
        {
            _openTabsRepository = openTabsRepository;
        }

        public void Project(TabOpened message)
        {
            _openTabsRepository.Insert(new OpenTab
            {
                Id = message.Id,
                TableNumber = message.TableNumber,
                Waiter = message.Waiter,
                Status = TabStatus.Seated
            });
        }
    }
}
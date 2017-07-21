﻿using System.Data;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;

namespace CQRSTutorial.DAL
{
    public class NHibernateConfiguration
    {
        private readonly IConnectionStringProviderFactory _connectionStringProviderFactory;

        public NHibernateConfiguration(IConnectionStringProviderFactory connectionStringProviderFactory)
        {
            _connectionStringProviderFactory = connectionStringProviderFactory;
        }

        public ISessionFactory CreateSessionFactory(IsolationLevel isolationLevel = IsolationLevel.Unspecified)
        {
            var msSqlConfiguration = MsSqlConfiguration
                .MsSql2012
                .IsolationLevel(isolationLevel)
                .ConnectionString(x => x.Is(_connectionStringProviderFactory.GetConnectionStringProvider().GetConnectionString()));

            var cfg = new CustomAutomappingConfiguration();
            return Fluently
                .Configure()
                .Database(msSqlConfiguration)
                .Mappings(m =>
                {
                    m.AutoMappings.Add(
                        AutoMap.AssemblyOf<EventToPublish>(cfg)
                            .UseOverridesFromAssemblyOf<EventToPublishMapping>());
                })
                .BuildSessionFactory();
        }
    }
}
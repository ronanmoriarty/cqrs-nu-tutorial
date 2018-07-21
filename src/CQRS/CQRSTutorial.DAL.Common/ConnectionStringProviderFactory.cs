﻿using Microsoft.Extensions.Configuration;

namespace CQRSTutorial.DAL.Common
{
    public class ConnectionStringProviderFactory
    {
        private readonly IConfigurationRoot _configurationRoot;

        public ConnectionStringProviderFactory(IConfigurationRoot configurationRoot)
        {
            _configurationRoot = configurationRoot;
        }

        public IConnectionStringProvider GetConnectionStringProvider()
        {
            return new ConnectionStringOverride(_configurationRoot["connectionString"]);
        }
    }
}

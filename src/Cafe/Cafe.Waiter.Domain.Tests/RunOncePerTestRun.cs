﻿using System.IO;
using log4net;
using log4net.Config;
using NUnit.Framework;

namespace Cafe.Waiter.Domain.Tests
{
    [SetUpFixture]
    public class RunOncePerTestRun
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var loggerRepository = LogManager.GetRepository();
            XmlConfigurator.Configure(loggerRepository, new FileInfo("log4net.config"));
        }
    }
}
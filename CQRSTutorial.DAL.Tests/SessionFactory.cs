﻿using NHibernate;

namespace CQRSTutorial.DAL.Tests
{
    public class SessionFactory
    {
        public static ISessionFactory WriteInstance { get; set; }
        public static ISessionFactory ReadInstance { get; set; }
    }
}
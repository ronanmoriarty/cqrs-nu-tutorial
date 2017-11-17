﻿using System;

namespace CQRSTutorial.DAL
{
    public interface IUnitOfWork : IDisposable
    {
        void Commit();
        IUnitOfWork Enrolling(params IHaveUnitOfWork[] haveUnitOfWorks);
        void ExecuteInTransaction(Action action);
    }
}
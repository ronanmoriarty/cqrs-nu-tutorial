﻿using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Cafe.Waiter.AcceptanceTests
{
    public class ContainsSingleTabConstraintBuilder
    {
        private readonly ChromeDriver _chromeDriver;
        private string _waiter;
        private int _tableNumber;

        public ContainsSingleTabConstraintBuilder(ChromeDriver chromeDriver)
        {
            _chromeDriver = chromeDriver;
        }

        public ContainsSingleTabConstraintBuilder WithWaiter(string waiter)
        {
            _waiter = waiter;
            return this;
        }

        public ContainsSingleTabConstraintBuilder WithTableNumber(int tableNumber)
        {
            _tableNumber = tableNumber;
            return this;
        }

        public static implicit operator bool(ContainsSingleTabConstraintBuilder builder)
        {
            return builder.ContainsTab();
        }

        public bool ContainsTab()
        {
            try
            {
                _chromeDriver.FindElementByXPath($"//td[text() = \"{_waiter}\"]"); // TODO: search at row-level for rows containing BOTH table number and waiter.
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }
    }
}
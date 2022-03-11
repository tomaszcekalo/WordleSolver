using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordleSolver
{
    internal class KeyboardSelector
    {
        public By Key(char letter)
        {
            return By.CssSelector($"button[data-key=\"{letter}\"]");
        }
    }
}
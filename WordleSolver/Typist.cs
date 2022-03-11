using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordleSolver
{
    internal interface ITypist
    {
        void InputWord(string word);
    }

    internal class Typist : ITypist
    {
        public Typist(Dictionary<char, IWebElement> keyboard)
        {
            Keyboard = keyboard;
        }

        public Dictionary<char, IWebElement> Keyboard { get; }

        public void InputWord(string word)
        {
            for (int i = 0; i < 5; i++)
            {
                Keyboard[word[i]].Click();
            }
            Keyboard['↵'].Click();
        }
    }
}
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordleSolver
{
    internal class WordGuesser
    {
        public WordGuesser(
            WordleData wordleData,
            IWebElement gameApp)
        {
            WordleData = wordleData;
            GameApp = gameApp;
        }

        public List<string> GetPossibleWords()
        {
            IEnumerable<string> words = WordleData.Words.ToList();
            List<char> present = new List<char>();
            List<char> absent = new List<char>();
            var nonEmptyRows = GameApp.GetShadowRoot().FindElements(By.CssSelector("#board game-row:not([letters=\"\"]"));

            foreach (var row in nonEmptyRows)
            {
                var letters = row.GetShadowRoot().FindElements(By.CssSelector("div.row game-tile"));
                var guess = row.GetAttribute("letters");
                for (int i = 0; i < 5; i++)
                {
                    var letterProperty = letters[i].GetAttribute("letter");
                    var character = letterProperty[0];
                    var dataState = letters[i].GetAttribute("evaluation");
                    if (dataState == "correct")
                    {
                        words = words.Where(x => x[i] == character).ToList();
                    }
                    else if (dataState == "present")
                    {
                        present.Add(character);
                    }
                    else if (dataState == "absent")
                    {
                        absent.Add(character);
                    }
                }
            }

            foreach (var p in present)
            {
                words = words.Where(x => x.Contains(p));
            }

            foreach (var a in absent.Distinct().Except(present))
            {
                words = words.Where(x => !x.Contains(a));
            }
            var possibleWords = words.ToList();
            return possibleWords;
        }

        public WordleData WordleData { get; }
        public IWebElement GameApp { get; }
    }
}
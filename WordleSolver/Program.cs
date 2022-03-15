// See https://aka.ms/new-console-template for more information
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using WordleSolver;

Console.WriteLine("Hello, World!");

var options = new ChromeOptions();
options.AddArgument("--disable-blink-features=AutomationControlled");
using var driver = new ChromeDriver(".", options);
var timeouts = driver.Manage().Timeouts();
timeouts.ImplicitWait = TimeSpan.FromSeconds(10);
timeouts.PageLoad = TimeSpan.FromSeconds(10);
timeouts.AsynchronousJavaScript = TimeSpan.FromSeconds(10);
string url = "https://www.nytimes.com/games/wordle/index.html";
driver.Navigate().GoToUrl(url);

driver.FindElement(By.Id("pz-gdpr-btn-accept")).Click();
var gameApp = driver.FindElement(By.CssSelector("game-app"));
var modal = gameApp.GetShadowRoot().FindElement(By.CssSelector("game-modal"));
var gameIcon = modal.GetShadowRoot().FindElement(By.CssSelector("game-icon"));
Thread.Sleep(1000);
gameIcon.Click();
Thread.Sleep(1000);
var ks = new KeyboardSelector();
var kbsr = gameApp.GetShadowRoot().FindElement(By.CssSelector("game-keyboard")).GetShadowRoot();

WebDriverWait wait = new WebDriverWait(driver, new TimeSpan(0, 0, 25));

var overlay = modal
    .GetShadowRoot()
    .FindElement(By.CssSelector("div.overlay"));
var enter = kbsr.FindElement(ks.Key('↵'));
wait.Until(drv =>
!overlay.Displayed && enter.Displayed);

var kb = new Dictionary<char, IWebElement>();
Thread.Sleep(5000);
for (char letter = 'a'; letter <= 'z'; letter++)
{
    kb.Add(letter, kbsr.FindElement(ks.Key(letter)));
}
kb.Add('↵', kbsr.FindElement(ks.Key('↵')));
ITypist typist = new Typist(kb);
Thread.Sleep(5000);

typist.InputWord("weary");
Thread.Sleep(5000);

var text = File.ReadAllText("word.json");
var wordleDB = JsonConvert.DeserializeObject<WordleData>(text);
List<string> possibleWords = wordleDB.Words.ToList();

while (possibleWords.Count > 1)
{
    IEnumerable<string> words = wordleDB.Words.ToList();
    List<char> present = new List<char>();
    List<char> absent = new List<char>();
    var nonEmptyRows = gameApp.GetShadowRoot().FindElements(By.CssSelector("#board game-row:not([letters=\"\"]"));

    if (nonEmptyRows.Count >= 6)
    {
        break;
    }
    int correctCount = 0;
    foreach (var row in nonEmptyRows)
    {
        var letters = row.GetShadowRoot().FindElements(By.CssSelector("div.row game-tile"));
        var guess = row.GetAttribute("letters");
        correctCount = 0;
        for (int i = 0; i < 5; i++)
        {
            var letterProperty = letters[i].GetAttribute("letter");
            var character = letterProperty[0];
            var dataState = letters[i].GetAttribute("evaluation");
            if (dataState == "correct")
            {
                words = words.Where(x => x[i] == character).ToList();
                correctCount++;
            }
            else if (dataState == "present")
            {
                words = words.Where(x => x[i] != character).ToList();
                present.Add(character);
            }
            else if (dataState == "absent")
            {
                absent.Add(character);
            }
        }
        if (correctCount == 5)
        {
            break;
        }
    }
    if (correctCount == 5)
    {
        break;
    }

    foreach (var p in present)
    {
        words = words.Where(x => x.Contains(p));
    }

    foreach (var a in absent.Distinct().Except(present))
    {
        words = words.Where(x => !x.Contains(a));
    }

    possibleWords = words.ToList();
    typist.InputWord(possibleWords.First());
    Thread.Sleep(5000);
}

Console.ReadKey();
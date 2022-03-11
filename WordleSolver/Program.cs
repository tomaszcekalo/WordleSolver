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
//driver.FindElement(By.CssSelector("game-app game-modal game-icon")).Click();
var ks = new KeyboardSelector();
var kbsr = gameApp.GetShadowRoot().FindElement(By.CssSelector("game-keyboard")).GetShadowRoot();

WebDriverWait wait = new WebDriverWait(driver, new TimeSpan(0, 0, 25));

var overlay = driver
    .FindElement(By.CssSelector("game-app"))
    .GetShadowRoot()
    .FindElement(By.CssSelector("game-modal"))
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

//kb['w'].Click();
//kb['e'].Click();
//kb['a'].Click();
//kb['r'].Click();
//kb['y'].Click();
//kb['↵'].Click();
typist.InputWord("weary");
Thread.Sleep(5000);

//kb['s'].Click();
//kb['c'].Click();
//kb['i'].Click();
//kb['o'].Click();
//kb['n'].Click();
//kb['↵'].Click();
typist.InputWord("scion");
Thread.Sleep(5000);

var nonEmptyRows = gameApp.GetShadowRoot().FindElements(By.CssSelector("#board game-row:not([letters=\"\"]"));

var text = File.ReadAllText("word.json");
var wordleDB = JsonConvert.DeserializeObject<WordleData>(text);
IEnumerable<string> words = wordleDB.Words.ToList();
List<char> present = new List<char>();
List<char> absent = new List<char>();

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

Console.ReadKey();
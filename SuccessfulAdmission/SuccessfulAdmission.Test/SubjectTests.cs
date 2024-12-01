using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;

namespace SuccessfulAdmission.Test;

public class SubjectTests
{
    private IWebDriver driver;
    private WebDriverWait wait;

    [SetUp]
    public void SetUp()
    {
        driver = new EdgeDriver();
        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
    }

    [TearDown]
    public void TearDown()
    {
        driver.Quit();
    }
    
    [Test]
    public void SubjectTest_SuccessfulAddSubject()
    {
        driver.Navigate().GoToUrl("https://localhost:44327/Home/Enter");

        var usernameField = driver.FindElement(By.Name("login"));
        var passwordField = driver.FindElement(By.Name("password"));
        var loginButton = driver.FindElement(By.CssSelector("input[type='submit']"));

        usernameField.SendKeys("admin");
        passwordField.SendKeys("123456");
        loginButton.Click();

        wait.Until(d => d.Url == "https://localhost:44327/");
        driver.Navigate().GoToUrl("https://localhost:44327/Home/SubjectCreate");

        var nameField = driver.FindElement(By.Id("name"));
        var countField = driver.FindElement(By.Id("maxPointsStr"));
        var submitButton = driver.FindElement(By.CssSelector("input[type='submit']"));
        
        nameField.SendKeys("Тестоведение");
        countField.SendKeys("100");
        
        submitButton.Click();
        
        var currentUrl = driver.Url;
        Assert.IsTrue(currentUrl.Contains("/Home/Subjects"));
        
        var subjectRows = driver.FindElements(By.CssSelector("#subjectTable tbody tr"));
        bool subjectFound = subjectRows.Any(row => row.Text.Contains("Тестоведение"));
        Assert.IsTrue(subjectFound, "Новый предмет не отображается в таблице");
    }
    
    [Test]
    public void SubjectTest_SuccessfulEditSubject()
    {
        driver.Navigate().GoToUrl("https://localhost:44327/Home/Enter");

        var usernameField = driver.FindElement(By.Name("login"));
        var passwordField = driver.FindElement(By.Name("password"));
        var loginButton = driver.FindElement(By.CssSelector("input[type='submit']"));

        usernameField.SendKeys("admin");
        passwordField.SendKeys("123456");
        loginButton.Click();

        wait.Until(d => d.Url == "https://localhost:44327/");
        driver.Navigate().GoToUrl("https://localhost:44327/Home/SubjectSetting?id=13");
        
        var nameField = driver.FindElement(By.Id("name"));
        var countField = driver.FindElement(By.Id("maxPointsStr"));
        var submitButton = driver.FindElement(By.CssSelector("input[type='submit']"));
        
        nameField.Clear();
        nameField.SendKeys("Тестонововедение");
        countField.Clear();
        countField.SendKeys("99");
        
        submitButton.Click();
        
        var currentUrl = driver.Url;
        Assert.IsTrue(currentUrl.Contains("/Home/Subjects"));
        
        var subjectRows = driver.FindElements(By.CssSelector("#subjectTable tbody tr"));
        bool subjectFound = subjectRows.Any(row => row.Text.Contains("Тестонововедение"));
        Assert.IsTrue(subjectFound, "Отредактированный предмет не отображается в таблице");
        
        bool oldSubjectFound = subjectRows.Any(row => row.Text.Contains("Тестоведение"));
        Assert.IsFalse(oldSubjectFound, "Старый предмет все еще отображается в таблице");
    }
    
    [Test]
    public void SubjectTest_SuccessfulDeleteSubject()
    {
        driver.Navigate().GoToUrl("https://localhost:44327/Home/Enter");

        var usernameField = driver.FindElement(By.Name("login"));
        var passwordField = driver.FindElement(By.Name("password"));
        var loginButton = driver.FindElement(By.CssSelector("input[type='submit']"));

        usernameField.SendKeys("admin");
        passwordField.SendKeys("123456");
        loginButton.Click();

        wait.Until(d => d.Url == "https://localhost:44327/");
        driver.Navigate().GoToUrl("https://localhost:44327/Home/SubjectSetting?id=13");
        
        var deleteButton = driver.FindElement(By.CssSelector("input[type='button'][value='Удалить']"));
        deleteButton.Click();
        
        var currentUrl = driver.Url;
        Assert.IsTrue(currentUrl.Contains("/Home/Subjects"));
        
        var subjectRows = driver.FindElements(By.CssSelector("#subjectTable tbody tr"));
        bool subjectFound = subjectRows.Any(row => row.Text.Contains("Тестонововедение"));
        Assert.IsFalse(subjectFound, "Удаленный предмет все еще отображается в таблице");
    }
    
    [Test]
    public void SubjectTest_SuccessfulSearchSubject()
    {
        driver.Navigate().GoToUrl("https://localhost:44327/Home/Enter");

        var usernameField = driver.FindElement(By.Name("login"));
        var passwordField = driver.FindElement(By.Name("password"));
        var loginButton = driver.FindElement(By.CssSelector("input[type='submit']"));

        usernameField.SendKeys("admin");
        passwordField.SendKeys("123456");
        loginButton.Click();

        wait.Until(d => d.Url == "https://localhost:44327/");
        driver.Navigate().GoToUrl("https://localhost:44327/Home/Subjects");
        
        var searchInput = driver.FindElement(By.Id("searchInput"));
        var searchButton = driver.FindElement(By.CssSelector("button.btn.btn-primary"));
        
        searchInput.SendKeys("Русский язык");
        
        searchButton.Click();
        
        var resultRows = driver.FindElements(By.CssSelector("#subjectTable tbody tr"));
        Assert.IsTrue(resultRows.Any(row => row.Text.Contains("Русский язык")));
    }
    
    [Test]
    public void SubjectTest_SuccessfulSortSubject()
    {
        driver.Navigate().GoToUrl("https://localhost:44327/Home/Enter");

        var usernameField = driver.FindElement(By.Name("login"));
        var passwordField = driver.FindElement(By.Name("password"));
        var loginButton = driver.FindElement(By.CssSelector("input[type='submit']"));

        usernameField.SendKeys("admin");
        passwordField.SendKeys("123456");
        loginButton.Click();

        wait.Until(d => d.Url == "https://localhost:44327/");
        driver.Navigate().GoToUrl("https://localhost:44327/Home/Subjects");
        
        var nameColumnHeader = driver.FindElement(By.CssSelector("th.text-info[onclick='sortTable(0)']"));
        
        nameColumnHeader.Click();
        
        Thread.Sleep(1000);
        
        var subjectRowsAsc = driver.FindElements(By.CssSelector("#subjectTable tbody tr"));
        var namesAsc = subjectRowsAsc.Select(row => row.FindElement(By.CssSelector("td")).Text).ToList();

        var namesAscSorted = namesAsc.OrderBy(name => name).ToList();
        CollectionAssert.AreEqual(namesAsc, namesAscSorted, "Сортировка по возрастанию не работает");
        
        nameColumnHeader.Click();
        
        Thread.Sleep(1000);
        
        var subjectRowsDesc = driver.FindElements(By.CssSelector("#subjectTable tbody tr"));
        var namesDesc = subjectRowsDesc.Select(row => row.FindElement(By.CssSelector("td")).Text).ToList();
        
        var namesDescSorted = namesDesc.OrderByDescending(name => name).ToList();
        CollectionAssert.AreEqual(namesDesc, namesDescSorted, "Сортировка по убыванию не работает");
    }
    
    [Test]
    public void SubjectTest_NoRights()
    {
        driver.Navigate().GoToUrl("https://localhost:44327/Home/Enter");

        var usernameField = driver.FindElement(By.Name("login"));
        var passwordField = driver.FindElement(By.Name("password"));
        var loginButton = driver.FindElement(By.CssSelector("input[type='submit']"));

        usernameField.SendKeys("user1");
        passwordField.SendKeys("123456");
        loginButton.Click();

        wait.Until(d => d.Url == "https://localhost:44327/");
        driver.Navigate().GoToUrl("https://localhost:44327/Home/SubjectSetting?id=1");
        
        var deleteButton = driver.FindElement(By.CssSelector("input[type='button'][value='Удалить']"));
        deleteButton.Click();
        wait.Until(d => driver.SwitchTo().Alert() != null);

        IAlert alert = driver.SwitchTo().Alert();
        string alertText = alert.Text;

        Assert.IsTrue(alertText.Contains("Недостаточно прав для данного действия"), "Ошибка не отображается корректно");
        alert.Accept();
    }
}
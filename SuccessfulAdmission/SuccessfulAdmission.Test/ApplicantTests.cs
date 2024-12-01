using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;

namespace SuccessfulAdmission.Test;

public class ApplicantTests
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
    public void ApplicantTest_SuccessfulAddApplicant()
    {
        driver.Navigate().GoToUrl("https://localhost:44327/Home/Enter");

        var usernameField = driver.FindElement(By.Name("login"));
        var passwordField = driver.FindElement(By.Name("password"));
        var loginButton = driver.FindElement(By.CssSelector("input[type='submit']"));

        usernameField.SendKeys("admin");
        passwordField.SendKeys("123456");
        loginButton.Click();

        wait.Until(d => d.Url == "https://localhost:44327/");
        driver.Navigate().GoToUrl("https://localhost:44327/Home/ApplicantCreate");

        var nameField = driver.FindElement(By.Id("name"));
        var submitButton = driver.FindElement(By.CssSelector("input[type='submit']"));
        
        nameField.SendKeys("Т.Т. Абитуриент");
        
        submitButton.Click();
        
        var currentUrl = driver.Url;
        Assert.IsTrue(currentUrl.Contains("/Home/Applicants"));
        
        var applicantRows = driver.FindElements(By.CssSelector("#applicantTable tbody tr"));
        bool applicantFound = applicantRows.Any(row => row.Text.Contains("Т.Т. Абитуриент"));
        Assert.IsTrue(applicantFound, "Новый абитуриент не отображается в таблице");
    }
    
    [Test]
    public void ApplicantTest_SuccessfulEditApplicant()
    {
        driver.Navigate().GoToUrl("https://localhost:44327/Home/Enter");

        var usernameField = driver.FindElement(By.Name("login"));
        var passwordField = driver.FindElement(By.Name("password"));
        var loginButton = driver.FindElement(By.CssSelector("input[type='submit']"));

        usernameField.SendKeys("admin");
        passwordField.SendKeys("123456");
        loginButton.Click();

        wait.Until(d => d.Url == "https://localhost:44327/");
        driver.Navigate().GoToUrl("https://localhost:44327/Home/ApplicantSetting?id=9");
        
        var nameField = driver.FindElement(By.Id("name"));
        var submitButton = driver.FindElement(By.CssSelector("input[type='submit']"));
        
        nameField.Clear();
        nameField.SendKeys("Тест Тест Тест");
        
        submitButton.Click();
        
        var currentUrl = driver.Url;
        Assert.IsTrue(currentUrl.Contains("/Home/Applicants"));
        
        var applicantRows = driver.FindElements(By.CssSelector("#applicantTable tbody tr"));
        bool applicantFound = applicantRows.Any(row => row.Text.Contains("Тест Тест Тест"));
        Assert.IsTrue(applicantFound, "Отредактированный абитуриент не отображается в таблице");
        
        bool oldApplicantFound = applicantRows.Any(row => row.Text.Contains("Т.Т. Абитуриент"));
        Assert.IsFalse(oldApplicantFound, "Старый абитуриент все еще отображается в таблице");
    }
    
    [Test]
    public void ApplicantTest_SuccessfulDeleteApplicant()
    {
        driver.Navigate().GoToUrl("https://localhost:44327/Home/Enter");

        var usernameField = driver.FindElement(By.Name("login"));
        var passwordField = driver.FindElement(By.Name("password"));
        var loginButton = driver.FindElement(By.CssSelector("input[type='submit']"));

        usernameField.SendKeys("admin");
        passwordField.SendKeys("123456");
        loginButton.Click();

        wait.Until(d => d.Url == "https://localhost:44327/");
        driver.Navigate().GoToUrl("https://localhost:44327/Home/ApplicantSetting?id=9");
        
        var deleteButton = driver.FindElement(By.CssSelector("input[type='button'][value='Удалить']"));
        deleteButton.Click();
        
        var currentUrl = driver.Url;
        Assert.IsTrue(currentUrl.Contains("/Home/Applicants"));
        
        var applicantRows = driver.FindElements(By.CssSelector("#applicantTable tbody tr"));
        bool applicantFound = applicantRows.Any(row => row.Text.Contains("Специальность в области искусственного интеллекта"));
        Assert.IsFalse(applicantFound, "Удаленный абитуриент все еще отображается в таблице");
    }
    
    [Test]
    public void ApplicantTest_SuccessfulSearchApplicant()
    {
        driver.Navigate().GoToUrl("https://localhost:44327/Home/Enter");

        var usernameField = driver.FindElement(By.Name("login"));
        var passwordField = driver.FindElement(By.Name("password"));
        var loginButton = driver.FindElement(By.CssSelector("input[type='submit']"));

        usernameField.SendKeys("admin");
        passwordField.SendKeys("123456");
        loginButton.Click();

        wait.Until(d => d.Url == "https://localhost:44327/");
        driver.Navigate().GoToUrl("https://localhost:44327/Home/Applicants");
        
        var searchInput = driver.FindElement(By.Id("searchInput"));
        var searchButton = driver.FindElement(By.CssSelector("button.btn.btn-primary"));
        
        searchInput.SendKeys("Иванов И.И.");
        
        searchButton.Click();
        
        var resultRows = driver.FindElements(By.CssSelector("#applicantTable tbody tr"));
        Assert.IsTrue(resultRows.Any(row => row.Text.Contains("Иванов И.И.")));
    }
    
    [Test]
    public void ApplicantTest_SuccessfulSortApplicant()
    {
        driver.Navigate().GoToUrl("https://localhost:44327/Home/Enter");

        var usernameField = driver.FindElement(By.Name("login"));
        var passwordField = driver.FindElement(By.Name("password"));
        var loginButton = driver.FindElement(By.CssSelector("input[type='submit']"));

        usernameField.SendKeys("admin");
        passwordField.SendKeys("123456");
        loginButton.Click();

        wait.Until(d => d.Url == "https://localhost:44327/");
        driver.Navigate().GoToUrl("https://localhost:44327/Home/Applicants");
        
        var nameColumnHeader = driver.FindElement(By.CssSelector("th.text-info[onclick='sortTable(0)']"));
        
        nameColumnHeader.Click();
        
        Thread.Sleep(1000);
        
        var applicantRowsAsc = driver.FindElements(By.CssSelector("#specialityTable tbody tr"));
        var namesAsc = applicantRowsAsc.Select(row => row.FindElement(By.CssSelector("td")).Text).ToList();

        var namesAscSorted = namesAsc.OrderBy(name => name).ToList();
        CollectionAssert.AreEqual(namesAsc, namesAscSorted, "Сортировка по возрастанию не работает");
        
        nameColumnHeader.Click();
        
        Thread.Sleep(1000);
        
        var applicantRowsDesc = driver.FindElements(By.CssSelector("#specialityTable tbody tr"));
        var namesDesc = applicantRowsDesc.Select(row => row.FindElement(By.CssSelector("td")).Text).ToList();
        
        var namesDescSorted = namesDesc.OrderByDescending(name => name).ToList();
        CollectionAssert.AreEqual(namesDesc, namesDescSorted, "Сортировка по убыванию не работает");
    }
    
    [Test]
    public void ApplicantTest_NoRights()
    {
        driver.Navigate().GoToUrl("https://localhost:44327/Home/Enter");

        var usernameField = driver.FindElement(By.Name("login"));
        var passwordField = driver.FindElement(By.Name("password"));
        var loginButton = driver.FindElement(By.CssSelector("input[type='submit']"));

        usernameField.SendKeys("user1");
        passwordField.SendKeys("123456");
        loginButton.Click();

        wait.Until(d => d.Url == "https://localhost:44327/");
        driver.Navigate().GoToUrl("https://localhost:44327/Home/ApplicantSetting?id=1");
        
        var deleteButton = driver.FindElement(By.CssSelector("input[type='button'][value='Удалить']"));
        deleteButton.Click();
        wait.Until(d => driver.SwitchTo().Alert() != null);

        IAlert alert = driver.SwitchTo().Alert();
        string alertText = alert.Text;

        Assert.IsTrue(alertText.Contains("Недостаточно прав для данного действия"), "Ошибка не отображается корректно");
        alert.Accept();
    }
}
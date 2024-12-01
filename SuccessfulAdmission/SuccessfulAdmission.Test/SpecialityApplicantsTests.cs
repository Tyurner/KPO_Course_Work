using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;

namespace SuccessfulAdmission.Test;

public class SpecialityApplicantsTests
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
    public void SpecialityApplicantsTest_Add()
    {
        driver.Navigate().GoToUrl("https://localhost:44327/Home/Enter");

        var usernameField = driver.FindElement(By.Name("login"));
        var passwordField = driver.FindElement(By.Name("password"));
        var loginButton = driver.FindElement(By.CssSelector("input[type='submit']"));

        usernameField.SendKeys("admin");
        passwordField.SendKeys("123456");
        loginButton.Click();

        wait.Until(d => d.Url == "https://localhost:44327/");
        driver.Navigate().GoToUrl("https://localhost:44327/Home/SpecialityApplicants/1");

        var applicantSelect = driver.FindElement(By.Id("applicantId"));
        var submitButton = driver.FindElement(By.CssSelector("input[type='submit']"));
        
        var options = applicantSelect.FindElements(By.TagName("option"));
        var applicantOption = options.FirstOrDefault(option => option.Text.Contains("Иванов И.И."));
        applicantOption.Click();
        
        submitButton.Click();
        
        Thread.Sleep(1000);
        var tableRows = driver.FindElements(By.CssSelector("#applicantsTable tbody tr"));
        bool applicantFound = tableRows.Any(row => row.Text.Contains("Иванов И.И."));
        Assert.IsTrue(applicantFound, "Абитуриент не был добавлен в список");
    }
    
    [Test]
    public void SpecialityApplicantsTests_Delete()
    {
        driver.Navigate().GoToUrl("https://localhost:44327/Home/Enter");

        var usernameField = driver.FindElement(By.Name("login"));
        var passwordField = driver.FindElement(By.Name("password"));
        var loginButton = driver.FindElement(By.CssSelector("input[type='submit']"));

        usernameField.SendKeys("admin");
        passwordField.SendKeys("123456");
        loginButton.Click();

        wait.Until(d => d.Url == "https://localhost:44327/");
        driver.Navigate().GoToUrl("https://localhost:44327/Home/SpecialityApplicants/1");
        
        var applicantsTable = driver.FindElement(By.Id("applicantsTable"));
        var rows = applicantsTable.FindElements(By.CssSelector("tbody tr"));
        
        var applicantName = "Иванов И.И.";
        var rowToDelete = rows.FirstOrDefault(row => row.Text.Contains(applicantName));

        Assert.IsNotNull(rowToDelete, "Абитуриент с именем '" + applicantName + "' не найден в таблице.");
        
        var deleteButton = rowToDelete.FindElement(By.CssSelector("input[value='Удалить']"));
        
        deleteButton.Click();
        Thread.Sleep(1000);
        applicantsTable = driver.FindElement(By.Id("applicantsTable"));
        rows = applicantsTable.FindElements(By.CssSelector("tbody tr"));
        var applicantFound = rows.Any(row => row.Text.Contains(applicantName));
        Assert.IsFalse(applicantFound, "Абитуриент с именем '" + applicantName + "' не был удален из списка.");
    }
    
    [Test]
    public void SpecialityApplicantsTests_Search()
    {
        driver.Navigate().GoToUrl("https://localhost:44327/Home/Enter");

        var usernameField = driver.FindElement(By.Name("login"));
        var passwordField = driver.FindElement(By.Name("password"));
        var loginButton = driver.FindElement(By.CssSelector("input[type='submit']"));

        usernameField.SendKeys("admin");
        passwordField.SendKeys("123456");
        loginButton.Click();

        wait.Until(d => d.Url == "https://localhost:44327/");
        driver.Navigate().GoToUrl("https://localhost:44327/Home/SpecialityApplicants/1");
        
        var searchField = driver.FindElement(By.Id("searchInput"));
        var searchButton = driver.FindElement(By.CssSelector("button[onclick='searchApplicants()']"));
        
        searchField.SendKeys("Петров П.П.");
        
        searchButton.Click();
        
        Thread.Sleep(1000);
        
        var tableRows = driver.FindElements(By.CssSelector("#applicantsTable tbody tr"));
        bool applicantFound = tableRows.Any(row => row.Text.Contains("Петров П.П."));
        Assert.IsTrue(applicantFound, "Абитуриент не найден после поиска");
    }
    
    [Test]
    public void SpecialityApplicantsTests_Filter()
    {
        driver.Navigate().GoToUrl("https://localhost:44327/Home/Enter");

        var usernameField = driver.FindElement(By.Name("login"));
        var passwordField = driver.FindElement(By.Name("password"));
        var loginButton = driver.FindElement(By.CssSelector("input[type='submit']"));

        usernameField.SendKeys("admin");
        passwordField.SendKeys("123456");
        loginButton.Click();

        wait.Until(d => d.Url == "https://localhost:44327/");
        driver.Navigate().GoToUrl("https://localhost:44327/Home/SpecialityApplicants/1");
        
        var passButton = driver.FindElement(By.Id("passButton"));
        var failButton = driver.FindElement(By.Id("failButton"));
        var allButton = driver.FindElement(By.Id("allButton"));
        
        passButton.Click();
        Thread.Sleep(1000);
        
        var failingApplicants = driver.FindElements(By.CssSelector("#applicantsTable tbody tr[data-pass='false']"));
        Assert.IsFalse(failingApplicants.Any(), "Не должно быть не проходящих абитуриентов");
        
        failButton.Click();
        Thread.Sleep(1000);
        
        var passingApplicantsOnly = driver.FindElements(By.CssSelector("#applicantsTable tbody tr[data-pass='true']"));
        Assert.IsFalse(passingApplicantsOnly.Any(), "Не должно быть проходящих абитуриентов");
        
        allButton.Click();
        Thread.Sleep(1000);

        var allApplicants = driver.FindElements(By.CssSelector("#applicantsTable tbody tr"));
        Assert.IsTrue(allApplicants.Count > 0, "Все абитуриенты должны отображаться");
    }
}
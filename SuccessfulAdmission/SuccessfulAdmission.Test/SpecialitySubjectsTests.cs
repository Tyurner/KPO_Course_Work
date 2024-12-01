using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;

namespace SuccessfulAdmission.Test;

public class SpecialitySubjectsTests
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
    public void ApplicantSubjectsTest_Add()
    {
        driver.Navigate().GoToUrl("https://localhost:44327/Home/Enter");

        var usernameField = driver.FindElement(By.Name("login"));
        var passwordField = driver.FindElement(By.Name("password"));
        var loginButton = driver.FindElement(By.CssSelector("input[type='submit']"));

        usernameField.SendKeys("admin");
        passwordField.SendKeys("123456");
        loginButton.Click();

        wait.Until(d => d.Url == "https://localhost:44327/");
        driver.Navigate().GoToUrl("https://localhost:44327/Home/SpecialitySubjects/1");

        var subjectSelect = driver.FindElement(By.Id("subjectId"));
        var submitButton = driver.FindElement(By.CssSelector("input[type='submit']"));
        
        var options = subjectSelect.FindElements(By.TagName("option"));
        var subjectOption = options.FirstOrDefault(option => option.Text.Contains("Русский язык"));
        subjectOption.Click();
        
        submitButton.Click();
        
        Thread.Sleep(1000);
        var tableRows = driver.FindElements(By.CssSelector("table tbody tr"));
        bool subjectFound = tableRows.Any(row => row.Text.Contains("Русский язык"));
        Assert.IsTrue(subjectFound, "Предмет не был добавлен в список");
    }

    [Test]
    public void SpecialitySubjectsTests_Delete()
    {
        driver.Navigate().GoToUrl("https://localhost:44327/Home/Enter");

        var usernameField = driver.FindElement(By.Name("login"));
        var passwordField = driver.FindElement(By.Name("password"));
        var loginButton = driver.FindElement(By.CssSelector("input[type='submit']"));

        usernameField.SendKeys("admin");
        passwordField.SendKeys("123456");
        loginButton.Click();

        wait.Until(d => d.Url == "https://localhost:44327/");
        driver.Navigate().GoToUrl("https://localhost:44327/Home/SpecialitySubjects/1");
        
        var subjectsTable = driver.FindElement(By.CssSelector("table"));
        var rows = subjectsTable.FindElements(By.CssSelector("tbody tr"));
        
        var subjectName = "Русский язык";
        var rowToDelete = rows.FirstOrDefault(row => row.Text.Contains(subjectName));

        Assert.IsNotNull(rowToDelete, "Предмет с названием '" + subjectName + "' не найден в таблице.");
        
        var deleteButton = rowToDelete.FindElement(By.CssSelector("input[value='Удалить']"));
        
        deleteButton.Click();
        Thread.Sleep(1000);
        subjectsTable = driver.FindElement(By.CssSelector("table"));
        rows = subjectsTable.FindElements(By.CssSelector("tbody tr"));
        var subjectFound = rows.Any(row => row.Text.Contains(subjectName));
        Assert.IsFalse(subjectFound, "Предмет с названием '" + subjectName + "' не был удален из списка.");
    }
}
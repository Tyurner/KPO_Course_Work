using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;

namespace SuccessfulAdmission.Test;

public class UsersTests
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
    public void UsersTests_Promote()
    {
        driver.Navigate().GoToUrl("https://localhost:44327/Home/Enter");

        var usernameField = driver.FindElement(By.Name("login"));
        var passwordField = driver.FindElement(By.Name("password"));
        var loginButton = driver.FindElement(By.CssSelector("input[type='submit']"));

        usernameField.SendKeys("admin");
        passwordField.SendKeys("123456");
        loginButton.Click();

        wait.Until(d => d.Url == "https://localhost:44327/");
        driver.Navigate().GoToUrl("https://localhost:44327/Home/Users");
        Thread.Sleep(1000);
        
        var rows = driver.FindElements(By.CssSelector("table tbody tr"));
        var rowToPromote = rows.FirstOrDefault(row => row.Text.Contains("user1"));

        Assert.IsNotNull(rowToPromote, "Пользователь с логином 'user1' не найден в таблице.");
        var promoteButton = rowToPromote.FindElement(By.CssSelector("input[value='Повысить']"));
        promoteButton.Click();
        Thread.Sleep(1000);
        
        rows = driver.FindElements(By.CssSelector("table tbody tr"));
        var updatedRow = rows.FirstOrDefault(row => row.Text.Contains("user1"));

        Assert.IsNotNull(updatedRow, "Пользователь с логином 'user1' не найден после повышения.");
        
        var roleCell = updatedRow.FindElements(By.TagName("td")).ElementAt(2);
        Assert.AreEqual("Админ", roleCell.Text.Trim(), "Роль пользователя 'user1' не изменилась на Админ.");
    }
    
    [Test]
    public void UsersTests_NoRights()
    {
        driver.Navigate().GoToUrl("https://localhost:44327/Home/Enter");

        var usernameField = driver.FindElement(By.Name("login"));
        var passwordField = driver.FindElement(By.Name("password"));
        var loginButton = driver.FindElement(By.CssSelector("input[type='submit']"));

        usernameField.SendKeys("user1");
        passwordField.SendKeys("123456");
        loginButton.Click();

        wait.Until(d => d.Url == "https://localhost:44327/");
        driver.Navigate().GoToUrl("https://localhost:44327/Home/Users");
        wait.Until(d => driver.SwitchTo().Alert() != null);

        IAlert alert = driver.SwitchTo().Alert();
        string alertText = alert.Text;

        Assert.IsTrue(alertText.Contains("Эта страница доступна только админам"), "Ошибка не отображается корректно");
        alert.Accept();
    }
}
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;

namespace SuccessfulAdmission.Test;

public class RegisterTest
{
    private IWebDriver driver;
    private WebDriverWait wait;

    [SetUp]
    public void SetUp()
    {
        driver = new EdgeDriver();
        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
    }

    [TearDown]
    public void TearDown()
    {
        driver.Quit();
    }
    
    [Test]
    public void RegisterTest_SuccessfulRegistered()
    {
        driver.Navigate().GoToUrl("https://localhost:44327/Home/Register");

        var usernameField = driver.FindElement(By.Name("login"));
        var passwordField = driver.FindElement(By.Name("password"));
        var emailField = driver.FindElement(By.Name("email"));
        var registerButton = driver.FindElement(By.CssSelector("input[type='submit']"));

        usernameField.SendKeys("userTest");
        passwordField.SendKeys("123456");
        emailField.SendKeys("test@mail.ru");
        registerButton.Click();
            
        wait.Until(d => d.Url == "https://localhost:44327/Home/Enter");

        Assert.AreEqual("https://localhost:44327/Home/Enter", driver.Url);
    }
    
    [Test]
    public void RegisterTest_InvalidEmptyRegister()
    {
        driver.Navigate().GoToUrl("https://localhost:44327/Home/Register");

        var usernameField = driver.FindElement(By.Name("login"));
        var passwordField = driver.FindElement(By.Name("password"));
        var emailField = driver.FindElement(By.Name("email"));
        var registerButton = driver.FindElement(By.CssSelector("input[type='submit']"));

        usernameField.SendKeys("");
        passwordField.SendKeys("");
        emailField.SendKeys("");
        registerButton.Click();
            
        wait.Until(d => driver.SwitchTo().Alert() != null);

        IAlert alert = driver.SwitchTo().Alert();
        string alertText = alert.Text;

        Assert.IsTrue(alertText.Contains("Введите почту, логин, пароль"), "Ошибка входа не отображается корректно");
        alert.Accept();
    }
    
    [Test]
    public void RegisterTest_LoginExistRegister()
    {
        driver.Navigate().GoToUrl("https://localhost:44327/Home/Register");

        var usernameField = driver.FindElement(By.Name("login"));
        var passwordField = driver.FindElement(By.Name("password"));
        var emailField = driver.FindElement(By.Name("email"));
        var registerButton = driver.FindElement(By.CssSelector("input[type='submit']"));

        usernameField.SendKeys("admin");
        passwordField.SendKeys("123456");
        emailField.SendKeys("test3@mail.ru");
        registerButton.Click();
            
        wait.Until(d => driver.SwitchTo().Alert() != null);

        IAlert alert = driver.SwitchTo().Alert();
        string alertText = alert.Text;

        Assert.IsTrue(alertText.Contains("Пользователь с таким логином уже существует"), "Ошибка входа не отображается корректно");
        alert.Accept();
    }
    
    [Test]
    public void RegisterTest_EmailExistRegister()
    {
        driver.Navigate().GoToUrl("https://localhost:44327/Home/Register");

        var usernameField = driver.FindElement(By.Name("login"));
        var passwordField = driver.FindElement(By.Name("password"));
        var emailField = driver.FindElement(By.Name("email"));
        var registerButton = driver.FindElement(By.CssSelector("input[type='submit']"));

        usernameField.SendKeys("test4");
        passwordField.SendKeys("123456");
        emailField.SendKeys("admin@example.com");
        registerButton.Click();
            
        wait.Until(d => driver.SwitchTo().Alert() != null);

        IAlert alert = driver.SwitchTo().Alert();
        string alertText = alert.Text;

        Assert.IsTrue(alertText.Contains("Пользователь с такой почтой уже существует"), "Ошибка входа не отображается корректно");
        alert.Accept();
    }
    
    [Test]
    public void RegisterTest_EmailInvalidRegister()
    {
        driver.Navigate().GoToUrl("https://localhost:44327/Home/Register");

        var usernameField = driver.FindElement(By.Name("login"));
        var passwordField = driver.FindElement(By.Name("password"));
        var emailField = driver.FindElement(By.Name("email"));
        var registerButton = driver.FindElement(By.CssSelector("input[type='submit']"));

        usernameField.SendKeys("test5");
        passwordField.SendKeys("123456");
        emailField.SendKeys("test5");
        registerButton.Click();
            
        wait.Until(d => driver.SwitchTo().Alert() != null);

        IAlert alert = driver.SwitchTo().Alert();
        string alertText = alert.Text;

        Assert.IsTrue(alertText.Contains("Введите корректную почту"), "Ошибка входа не отображается корректно");
        alert.Accept();
    }
    
    [Test]
    public void RegisterTest_LoginShortRegister()
    {
        driver.Navigate().GoToUrl("https://localhost:44327/Home/Register");

        var usernameField = driver.FindElement(By.Name("login"));
        var passwordField = driver.FindElement(By.Name("password"));
        var emailField = driver.FindElement(By.Name("email"));
        var registerButton = driver.FindElement(By.CssSelector("input[type='submit']"));

        usernameField.SendKeys("test");
        passwordField.SendKeys("123456");
        emailField.SendKeys("test6@mail.ru");
        registerButton.Click();
            
        wait.Until(d => driver.SwitchTo().Alert() != null);

        IAlert alert = driver.SwitchTo().Alert();
        string alertText = alert.Text;

        Assert.IsTrue(alertText.Contains("Логин не должен быть короче 5 символов"), "Ошибка входа не отображается корректно");
        alert.Accept();
    }
    
    [Test]
    public void RegisterTest_PasswordShortRegister()
    {
        driver.Navigate().GoToUrl("https://localhost:44327/Home/Register");

        var usernameField = driver.FindElement(By.Name("login"));
        var passwordField = driver.FindElement(By.Name("password"));
        var emailField = driver.FindElement(By.Name("email"));
        var registerButton = driver.FindElement(By.CssSelector("input[type='submit']"));

        usernameField.SendKeys("test7");
        passwordField.SendKeys("123");
        emailField.SendKeys("test7@mail.ru");
        registerButton.Click();
            
        wait.Until(d => driver.SwitchTo().Alert() != null);

        IAlert alert = driver.SwitchTo().Alert();
        string alertText = alert.Text;

        Assert.IsTrue(alertText.Contains("Пароль не должен быть короче 6 символов"), "Ошибка входа не отображается корректно");
        alert.Accept();
    }
}
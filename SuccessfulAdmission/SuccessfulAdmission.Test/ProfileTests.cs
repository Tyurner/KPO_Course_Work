using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;

namespace SuccessfulAdmission.Test;

public class ProfileTests
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
    public void ProfileTest_SuccessfulUpdateProfile()
    {
        driver.Navigate().GoToUrl("https://localhost:44327/Home/Enter");

        var usernameField = driver.FindElement(By.Name("login"));
        var passwordField = driver.FindElement(By.Name("password"));
        var loginButton = driver.FindElement(By.CssSelector("input[type='submit']"));

        usernameField.SendKeys("user1");
        passwordField.SendKeys("123456");
        loginButton.Click();
            
        wait.Until(d => d.Url == "https://localhost:44327/");
        driver.Navigate().GoToUrl("https://localhost:44327/Home/Profile");
        var emailField = driver.FindElement(By.Name("email"));
        usernameField = driver.FindElement(By.Name("login"));
        passwordField = driver.FindElement(By.Name("password"));
        var isTwoFactorCheckbox = driver.FindElement(By.Name("isTwoFactor"));
        var submitButton = driver.FindElement(By.CssSelector("input[type='submit']"));
        
        string newEmail = "user123@example.com";
        string newLogin = "user123";
        string newPassword = "1234567";

        emailField.Clear();
        emailField.SendKeys(newEmail);
        usernameField.Clear();
        usernameField.SendKeys(newLogin);
        passwordField.Clear();
        passwordField.SendKeys(newPassword);
        
        if (!isTwoFactorCheckbox.Selected)
        {
            isTwoFactorCheckbox.Click();
        }
        submitButton.Click();
        try
        {
            wait.Until(d => d.Url.Contains("/Profile"));
            emailField = driver.FindElement(By.Name("email"));
            usernameField = driver.FindElement(By.Name("login"));
            Assert.AreEqual(newEmail, emailField.GetAttribute("value"), "Email не был обновлен.");
            Assert.AreEqual(newLogin, usernameField.GetAttribute("value"), "Логин не был обновлен.");
        }
        catch (WebDriverTimeoutException)
        {
            Assert.Fail("Ошибка: Редирект или обновление данных не произошло вовремя.");
        }
    }
    
    [Test]
    public void ProfileTest_EmptyUpdateProfile()
    {
        driver.Navigate().GoToUrl("https://localhost:44327/Home/Enter");

        var usernameField = driver.FindElement(By.Name("login"));
        var passwordField = driver.FindElement(By.Name("password"));
        var loginButton = driver.FindElement(By.CssSelector("input[type='submit']"));

        usernameField.SendKeys("user2");
        passwordField.SendKeys("123456");
        loginButton.Click();
            
        wait.Until(d => d.Url == "https://localhost:44327/");
        driver.Navigate().GoToUrl("https://localhost:44327/Home/Profile");
        var emailField = driver.FindElement(By.Name("email"));
        usernameField = driver.FindElement(By.Name("login"));
        passwordField = driver.FindElement(By.Name("password"));
        var submitButton = driver.FindElement(By.CssSelector("input[type='submit']"));
        
        string newEmail = "";
        string newLogin = "";
        string newPassword = "";

        emailField.Clear();
        emailField.SendKeys(newEmail);
        usernameField.Clear();
        usernameField.SendKeys(newLogin);
        passwordField.Clear();
        passwordField.SendKeys(newPassword);
        
        submitButton.Click();
        wait.Until(d => driver.SwitchTo().Alert() != null);

        IAlert alert = driver.SwitchTo().Alert();
        string alertText = alert.Text;

        Assert.IsTrue(alertText.Contains("Введите почту, логин, пароль"), "Ошибка входа не отображается корректно");
        alert.Accept();
    }
    
    [Test]
    public void ProfileTest_LoginExistUpdateProfile()
    {
        driver.Navigate().GoToUrl("https://localhost:44327/Home/Enter");

        var usernameField = driver.FindElement(By.Name("login"));
        var passwordField = driver.FindElement(By.Name("password"));
        var loginButton = driver.FindElement(By.CssSelector("input[type='submit']"));

        usernameField.SendKeys("user2");
        passwordField.SendKeys("123456");
        loginButton.Click();
            
        wait.Until(d => d.Url == "https://localhost:44327/");
        driver.Navigate().GoToUrl("https://localhost:44327/Home/Profile");
        var emailField = driver.FindElement(By.Name("email"));
        usernameField = driver.FindElement(By.Name("login"));
        passwordField = driver.FindElement(By.Name("password"));
        var submitButton = driver.FindElement(By.CssSelector("input[type='submit']"));
        
        string newEmail = "test3@mail.ru";
        string newLogin = "admin";
        string newPassword = "123456";

        emailField.Clear();
        emailField.SendKeys(newEmail);
        usernameField.Clear();
        usernameField.SendKeys(newLogin);
        passwordField.Clear();
        passwordField.SendKeys(newPassword);
        
        submitButton.Click();
        wait.Until(d => driver.SwitchTo().Alert() != null);

        IAlert alert = driver.SwitchTo().Alert();
        string alertText = alert.Text;

        Assert.IsTrue(alertText.Contains("Пользователь с таким логином уже существует"), "Ошибка входа не отображается корректно");
        alert.Accept();
    }
    
    [Test]
    public void ProfileTest_EmailExistUpdateProfile()
    {
        driver.Navigate().GoToUrl("https://localhost:44327/Home/Enter");

        var usernameField = driver.FindElement(By.Name("login"));
        var passwordField = driver.FindElement(By.Name("password"));
        var loginButton = driver.FindElement(By.CssSelector("input[type='submit']"));

        usernameField.SendKeys("user2");
        passwordField.SendKeys("123456");
        loginButton.Click();
            
        wait.Until(d => d.Url == "https://localhost:44327/");
        driver.Navigate().GoToUrl("https://localhost:44327/Home/Profile");
        var emailField = driver.FindElement(By.Name("email"));
        usernameField = driver.FindElement(By.Name("login"));
        passwordField = driver.FindElement(By.Name("password"));
        var submitButton = driver.FindElement(By.CssSelector("input[type='submit']"));
        
        string newEmail = "admin@example.com";
        string newLogin = "test4";
        string newPassword = "123456";

        emailField.Clear();
        emailField.SendKeys(newEmail);
        usernameField.Clear();
        usernameField.SendKeys(newLogin);
        passwordField.Clear();
        passwordField.SendKeys(newPassword);
        
        submitButton.Click();
        wait.Until(d => driver.SwitchTo().Alert() != null);

        IAlert alert = driver.SwitchTo().Alert();
        string alertText = alert.Text;

        Assert.IsTrue(alertText.Contains("Пользователь с такой почтой уже существует"), "Ошибка входа не отображается корректно");
        alert.Accept();
    }
    
    [Test]
    public void ProfileTest_EmailInvalidUpdateProfile()
    {
        driver.Navigate().GoToUrl("https://localhost:44327/Home/Enter");

        var usernameField = driver.FindElement(By.Name("login"));
        var passwordField = driver.FindElement(By.Name("password"));
        var loginButton = driver.FindElement(By.CssSelector("input[type='submit']"));

        usernameField.SendKeys("user2");
        passwordField.SendKeys("123456");
        loginButton.Click();
            
        wait.Until(d => d.Url == "https://localhost:44327/");
        driver.Navigate().GoToUrl("https://localhost:44327/Home/Profile");
        var emailField = driver.FindElement(By.Name("email"));
        usernameField = driver.FindElement(By.Name("login"));
        passwordField = driver.FindElement(By.Name("password"));
        var submitButton = driver.FindElement(By.CssSelector("input[type='submit']"));
        
        string newEmail = "test5";
        string newLogin = "test5";
        string newPassword = "123456";

        emailField.Clear();
        emailField.SendKeys(newEmail);
        usernameField.Clear();
        usernameField.SendKeys(newLogin);
        passwordField.Clear();
        passwordField.SendKeys(newPassword);
        
        submitButton.Click();
        wait.Until(d => driver.SwitchTo().Alert() != null);

        IAlert alert = driver.SwitchTo().Alert();
        string alertText = alert.Text;

        Assert.IsTrue(alertText.Contains("Введите корректную почту"), "Ошибка входа не отображается корректно");
        alert.Accept();
    }
    
    [Test]
    public void ProfileTest_LoginShortUpdateProfile()
    {
        driver.Navigate().GoToUrl("https://localhost:44327/Home/Enter");

        var usernameField = driver.FindElement(By.Name("login"));
        var passwordField = driver.FindElement(By.Name("password"));
        var loginButton = driver.FindElement(By.CssSelector("input[type='submit']"));

        usernameField.SendKeys("user2");
        passwordField.SendKeys("123456");
        loginButton.Click();
            
        wait.Until(d => d.Url == "https://localhost:44327/");
        driver.Navigate().GoToUrl("https://localhost:44327/Home/Profile");
        var emailField = driver.FindElement(By.Name("email"));
        usernameField = driver.FindElement(By.Name("login"));
        passwordField = driver.FindElement(By.Name("password"));
        var submitButton = driver.FindElement(By.CssSelector("input[type='submit']"));
        
        string newEmail = "test6@mail.ru";
        string newLogin = "test";
        string newPassword = "123456";

        emailField.Clear();
        emailField.SendKeys(newEmail);
        usernameField.Clear();
        usernameField.SendKeys(newLogin);
        passwordField.Clear();
        passwordField.SendKeys(newPassword);
        
        submitButton.Click();
        wait.Until(d => driver.SwitchTo().Alert() != null);

        IAlert alert = driver.SwitchTo().Alert();
        string alertText = alert.Text;

        Assert.IsTrue(alertText.Contains("Логин не должен быть короче 5 символов"), "Ошибка входа не отображается корректно");
        alert.Accept();
    }
    
    [Test]
    public void ProfileTest_PasswordShortUpdateProfile()
    {
        driver.Navigate().GoToUrl("https://localhost:44327/Home/Enter");

        var usernameField = driver.FindElement(By.Name("login"));
        var passwordField = driver.FindElement(By.Name("password"));
        var loginButton = driver.FindElement(By.CssSelector("input[type='submit']"));

        usernameField.SendKeys("user2");
        passwordField.SendKeys("123456");
        loginButton.Click();
            
        wait.Until(d => d.Url == "https://localhost:44327/");
        driver.Navigate().GoToUrl("https://localhost:44327/Home/Profile");
        var emailField = driver.FindElement(By.Name("email"));
        usernameField = driver.FindElement(By.Name("login"));
        passwordField = driver.FindElement(By.Name("password"));
        var submitButton = driver.FindElement(By.CssSelector("input[type='submit']"));
        
        string newEmail = "test7@mail.ru";
        string newLogin = "test7";
        string newPassword = "123";

        emailField.Clear();
        emailField.SendKeys(newEmail);
        usernameField.Clear();
        usernameField.SendKeys(newLogin);
        passwordField.Clear();
        passwordField.SendKeys(newPassword);
        
        submitButton.Click();
        wait.Until(d => driver.SwitchTo().Alert() != null);

        IAlert alert = driver.SwitchTo().Alert();
        string alertText = alert.Text;

        Assert.IsTrue(alertText.Contains("Пароль не должен быть короче 6 символов"), "Ошибка входа не отображается корректно");
        alert.Accept();
    }
}
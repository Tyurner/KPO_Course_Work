using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;

namespace SuccessfulAdmission.Test
{
    public class LoginTests
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
        public void LoginTest_SuccessfulLogin()
        {
            driver.Navigate().GoToUrl("https://localhost:44327/Home/Enter");

            var usernameField = driver.FindElement(By.Name("login"));
            var passwordField = driver.FindElement(By.Name("password"));
            var loginButton = driver.FindElement(By.CssSelector("input[type='submit']"));

            usernameField.SendKeys("user1");
            passwordField.SendKeys("123456");
            loginButton.Click();
            
            wait.Until(d => d.Url == "https://localhost:44327/");

            Assert.AreEqual("https://localhost:44327/", driver.Url);
        }

        [Test]
        public void LoginTest_InvalidLogin()
        {
            driver.Navigate().GoToUrl("https://localhost:44327/Home/Enter");

            var usernameField = driver.FindElement(By.Name("login"));
            var passwordField = driver.FindElement(By.Name("password"));
            var loginButton = driver.FindElement(By.CssSelector("input[type='submit']"));

            usernameField.SendKeys("invalid-username");
            passwordField.SendKeys("invalid-password");
            loginButton.Click();
            
            wait.Until(d => driver.SwitchTo().Alert() != null);

            IAlert alert = driver.SwitchTo().Alert();
            string alertText = alert.Text;

            Assert.IsTrue(alertText.Contains("Неверный логин/пароль"), "Ошибка входа не отображается корректно");
            alert.Accept();
        }
        
        [Test]
        public void LoginTest_EmptyLogin()
        {
            driver.Navigate().GoToUrl("https://localhost:44327/Home/Enter");

            var usernameField = driver.FindElement(By.Name("login"));
            var passwordField = driver.FindElement(By.Name("password"));
            var loginButton = driver.FindElement(By.CssSelector("input[type='submit']"));

            usernameField.SendKeys("");
            passwordField.SendKeys("123456");
            loginButton.Click();
            
            wait.Until(d => driver.SwitchTo().Alert() != null);

            IAlert alert = driver.SwitchTo().Alert();
            string alertText = alert.Text;

            Assert.IsTrue(alertText.Contains("Введите логин и пароль"), "Ошибка входа не отображается корректно");
            alert.Accept();
        }
        
        [Test]
        public void LoginTest_TwoFactorLogin()
        {
            driver.Navigate().GoToUrl("https://localhost:44327/Home/Enter");

            var usernameField = driver.FindElement(By.Name("login"));
            var passwordField = driver.FindElement(By.Name("password"));
            var loginButton = driver.FindElement(By.CssSelector("input[type='submit']"));

            usernameField.SendKeys("admin");
            passwordField.SendKeys("123456");
            loginButton.Click();
            
            wait.Until(d => d.Url == "https://localhost:44327/Home/Enter2");

            Assert.AreEqual("https://localhost:44327/Home/Enter2", driver.Url);
        }
        
        [Test]
        public void LoginTest_TwoFactorEmptyLogin()
        {
            driver.Navigate().GoToUrl("https://localhost:44327/Home/Enter");

            var usernameField = driver.FindElement(By.Name("login"));
            var passwordField = driver.FindElement(By.Name("password"));
            var loginButton = driver.FindElement(By.CssSelector("input[type='submit']"));

            usernameField.SendKeys("admin");
            passwordField.SendKeys("123456");
            loginButton.Click();
            
            wait.Until(d => d.Url == "https://localhost:44327/Home/Enter2");
            
            var codeField = driver.FindElement(By.Name("code"));
            loginButton = driver.FindElement(By.CssSelector("input[type='submit']"));
            codeField.SendKeys("");
            loginButton.Click();

            wait.Until(d => driver.SwitchTo().Alert() != null);

            IAlert alert = driver.SwitchTo().Alert();
            string alertText = alert.Text;

            Assert.IsTrue(alertText.Contains("Введите код"), "Ошибка входа не отображается корректно");
            alert.Accept();
        }
        
        [Test]
        public void LoginTest_TwoFactorInvalidLogin()
        {
            driver.Navigate().GoToUrl("https://localhost:44327/Home/Enter");

            var usernameField = driver.FindElement(By.Name("login"));
            var passwordField = driver.FindElement(By.Name("password"));
            var loginButton = driver.FindElement(By.CssSelector("input[type='submit']"));

            usernameField.SendKeys("admin");
            passwordField.SendKeys("123456");
            loginButton.Click();
            
            wait.Until(d => d.Url == "https://localhost:44327/Home/Enter2");
            
            var codeField = driver.FindElement(By.Name("code"));
            loginButton = driver.FindElement(By.CssSelector("input[type='submit']"));
            codeField.SendKeys("22");
            loginButton.Click();

            wait.Until(d => driver.SwitchTo().Alert() != null);

            IAlert alert = driver.SwitchTo().Alert();
            string alertText = alert.Text;

            Assert.IsTrue(alertText.Contains("Неверный код"), "Ошибка входа не отображается корректно");
            alert.Accept();
        }
    }
}
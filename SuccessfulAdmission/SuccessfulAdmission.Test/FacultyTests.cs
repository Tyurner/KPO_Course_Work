using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;

namespace SuccessfulAdmission.Test;

public class FacultyTests
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
    public void FacultyTest_SuccessfulAddFaculty()
    {
        driver.Navigate().GoToUrl("https://localhost:44327/Home/Enter");

        var usernameField = driver.FindElement(By.Name("login"));
        var passwordField = driver.FindElement(By.Name("password"));
        var loginButton = driver.FindElement(By.CssSelector("input[type='submit']"));

        usernameField.SendKeys("admin");
        passwordField.SendKeys("123456");
        loginButton.Click();

        wait.Until(d => d.Url == "https://localhost:44327/");
        driver.Navigate().GoToUrl("https://localhost:44327/Home/FacultyCreate");

        var nameField = driver.FindElement(By.Id("name"));
        var descField = driver.FindElement(By.Id("desc"));
        var submitButton = driver.FindElement(By.CssSelector("input[type='submit']"));

        nameField.SendKeys("Факультет искусственного интеллекта");
        descField.SendKeys("Описание факультета искусственного интеллекта");

        submitButton.Click();

        var currentUrl = driver.Url;
        Assert.IsTrue(currentUrl.Contains("/Home/Faculties"));

        var facultyRows = driver.FindElements(By.CssSelector("#facultyTable tbody tr"));
        bool facultyFound = facultyRows.Any(row => row.Text.Contains("Факультет искусственного интеллекта"));
        Assert.IsTrue(facultyFound, "Новый факультет не отображается в таблице");
    }

    [Test]
    public void FacultyTest_SuccessfulEditFaculty()
    {
        driver.Navigate().GoToUrl("https://localhost:44327/Home/Enter");

        var usernameField = driver.FindElement(By.Name("login"));
        var passwordField = driver.FindElement(By.Name("password"));
        var loginButton = driver.FindElement(By.CssSelector("input[type='submit']"));

        usernameField.SendKeys("admin");
        passwordField.SendKeys("123456");
        loginButton.Click();

        wait.Until(d => d.Url == "https://localhost:44327/");
        driver.Navigate().GoToUrl("https://localhost:44327/Home/FacultySetting?id=17");
        
        var nameField = driver.FindElement(By.Id("name"));
        var descField = driver.FindElement(By.Id("desc"));
        var submitButton = driver.FindElement(By.CssSelector("input[type='submit']"));
        
        nameField.Clear();
        nameField.SendKeys("Факультет данных");
        descField.Clear();
        descField.SendKeys("Описание факультета данных");
        
        submitButton.Click();
        
        var currentUrl = driver.Url;
        Assert.IsTrue(currentUrl.Contains("/Home/Faculties"));
        
        var facultyRows = driver.FindElements(By.CssSelector("#facultyTable tbody tr"));
        bool facultyFound = facultyRows.Any(row => row.Text.Contains("Факультет данных"));
        Assert.IsTrue(facultyFound, "Отредактированный факультет не отображается в таблице");
        
        bool oldFacultyFound = facultyRows.Any(row => row.Text.Contains("Факультет искусственного интеллекта"));
        Assert.IsFalse(oldFacultyFound, "Старое имя факультета все еще отображается в таблице");
    }

    [Test]
    public void FacultyTest_SuccessfulDeleteFaculty()
    {
        driver.Navigate().GoToUrl("https://localhost:44327/Home/Enter");

        var usernameField = driver.FindElement(By.Name("login"));
        var passwordField = driver.FindElement(By.Name("password"));
        var loginButton = driver.FindElement(By.CssSelector("input[type='submit']"));

        usernameField.SendKeys("admin");
        passwordField.SendKeys("123456");
        loginButton.Click();

        wait.Until(d => d.Url == "https://localhost:44327/");
        driver.Navigate().GoToUrl("https://localhost:44327/Home/FacultySetting?id=17");
        
        var deleteButton = driver.FindElement(By.CssSelector("input[type='button'][value='Удалить']"));
        deleteButton.Click();
        
        var currentUrl = driver.Url;
        Assert.IsTrue(currentUrl.Contains("/Home/Faculties"));
        
        var facultyRows = driver.FindElements(By.CssSelector("#facultyTable tbody tr"));
        bool facultyFound = facultyRows.Any(row => row.Text.Contains("Факультет данных"));
        Assert.IsFalse(facultyFound, "Удаленный факультет все еще отображается в таблице");
    }

    [Test]
    public void FacultyTest_SuccessfulSearchFaculty()
    {
        driver.Navigate().GoToUrl("https://localhost:44327/Home/Enter");

        var usernameField = driver.FindElement(By.Name("login"));
        var passwordField = driver.FindElement(By.Name("password"));
        var loginButton = driver.FindElement(By.CssSelector("input[type='submit']"));

        usernameField.SendKeys("admin");
        passwordField.SendKeys("123456");
        loginButton.Click();

        wait.Until(d => d.Url == "https://localhost:44327/");
        driver.Navigate().GoToUrl("https://localhost:44327/Home/Faculties");
        
        var searchInput = driver.FindElement(By.Id("searchInput"));
        var searchButton = driver.FindElement(By.CssSelector("button.btn.btn-primary"));
        
        searchInput.SendKeys("Факультет информатики");
        
        searchButton.Click();
        
        var resultRows = driver.FindElements(By.CssSelector("#facultyTable tbody tr"));
        Assert.IsTrue(resultRows.Any(row => row.Text.Contains("Факультет информатики")));
    }

    [Test]
    public void FacultyTest_SuccessfulSortFaculty()
    {
        driver.Navigate().GoToUrl("https://localhost:44327/Home/Enter");

        var usernameField = driver.FindElement(By.Name("login"));
        var passwordField = driver.FindElement(By.Name("password"));
        var loginButton = driver.FindElement(By.CssSelector("input[type='submit']"));

        usernameField.SendKeys("admin");
        passwordField.SendKeys("123456");
        loginButton.Click();

        wait.Until(d => d.Url == "https://localhost:44327/");
        driver.Navigate().GoToUrl("https://localhost:44327/Home/Faculties");
        
        var nameColumnHeader = driver.FindElement(By.CssSelector("th.text-info[onclick='sortTable(0)']"));
        
        nameColumnHeader.Click();
        
        Thread.Sleep(1000);
        
        var facultyRowsAsc = driver.FindElements(By.CssSelector("#facultyTable tbody tr"));
        var namesAsc = facultyRowsAsc.Select(row => row.FindElement(By.CssSelector("td")).Text).ToList();

        var namesAscSorted = namesAsc.OrderBy(name => name).ToList();
        CollectionAssert.AreEqual(namesAsc, namesAscSorted, "Сортировка по возрастанию не работает");
        
        nameColumnHeader.Click();
        
        Thread.Sleep(1000);
        
        var facultyRowsDesc = driver.FindElements(By.CssSelector("#facultyTable tbody tr"));
        var namesDesc = facultyRowsDesc.Select(row => row.FindElement(By.CssSelector("td")).Text).ToList();
        
        var namesDescSorted = namesDesc.OrderByDescending(name => name).ToList();
        CollectionAssert.AreEqual(namesDesc, namesDescSorted, "Сортировка по убыванию не работает");
    }

    [Test]
    public void FacultyTest_NoRights()
    {
        driver.Navigate().GoToUrl("https://localhost:44327/Home/Enter");

        var usernameField = driver.FindElement(By.Name("login"));
        var passwordField = driver.FindElement(By.Name("password"));
        var loginButton = driver.FindElement(By.CssSelector("input[type='submit']"));

        usernameField.SendKeys("user1");
        passwordField.SendKeys("123456");
        loginButton.Click();

        wait.Until(d => d.Url == "https://localhost:44327/");
        driver.Navigate().GoToUrl("https://localhost:44327/Home/FacultySetting?id=1");
        
        var deleteButton = driver.FindElement(By.CssSelector("input[type='button'][value='Удалить']"));
        deleteButton.Click();
        wait.Until(d => driver.SwitchTo().Alert() != null);

        IAlert alert = driver.SwitchTo().Alert();
        string alertText = alert.Text;

        Assert.IsTrue(alertText.Contains("Недостаточно прав для данного действия"), "Ошибка не отображается корректно");
        alert.Accept();
    }
}
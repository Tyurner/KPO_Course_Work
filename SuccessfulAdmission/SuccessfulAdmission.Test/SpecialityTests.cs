using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;

namespace SuccessfulAdmission.Test;

public class SpecialityTests
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
    public void SpecialityTest_SuccessfulAddSpeciality()
    {
        driver.Navigate().GoToUrl("https://localhost:44327/Home/Enter");

        var usernameField = driver.FindElement(By.Name("login"));
        var passwordField = driver.FindElement(By.Name("password"));
        var loginButton = driver.FindElement(By.CssSelector("input[type='submit']"));

        usernameField.SendKeys("admin");
        passwordField.SendKeys("123456");
        loginButton.Click();

        wait.Until(d => d.Url == "https://localhost:44327/");
        driver.Navigate().GoToUrl("https://localhost:44327/Home/SpecialityCreate");

        var nameField = driver.FindElement(By.Id("name"));
        var descField = driver.FindElement(By.Id("desc"));
        var countField = driver.FindElement(By.Id("countStr"));
        var facultySelect = driver.FindElement(By.Id("facultyId"));
        var submitButton = driver.FindElement(By.CssSelector("input[type='submit']"));
        
        nameField.SendKeys("Специальность в области данных");
        descField.SendKeys("Описание специальности в области данных");
        countField.SendKeys("4");
        
        var facultyOption = facultySelect.FindElement(By.XPath("//option[text()='Факультет информатики']"));
        facultyOption.Click();
        
        submitButton.Click();
        
        var currentUrl = driver.Url;
        Assert.IsTrue(currentUrl.Contains("/Home/Specialities"));
        
        var specialtyRows = driver.FindElements(By.CssSelector("#specialityTable tbody tr"));
        bool specialtyFound = specialtyRows.Any(row => row.Text.Contains("Специальность в области данных"));
        Assert.IsTrue(specialtyFound, "Новая специальность не отображается в таблице");
    }
    
    [Test]
    public void SpecialityTest_SuccessfulEditSpeciality()
    {
        driver.Navigate().GoToUrl("https://localhost:44327/Home/Enter");

        var usernameField = driver.FindElement(By.Name("login"));
        var passwordField = driver.FindElement(By.Name("password"));
        var loginButton = driver.FindElement(By.CssSelector("input[type='submit']"));

        usernameField.SendKeys("admin");
        passwordField.SendKeys("123456");
        loginButton.Click();

        wait.Until(d => d.Url == "https://localhost:44327/");
        driver.Navigate().GoToUrl("https://localhost:44327/Home/SpecialitySetting?id=31");
        
        var nameField = driver.FindElement(By.Id("name"));
        var descField = driver.FindElement(By.Id("desc"));
        var countField = driver.FindElement(By.Id("countStr"));
        var facultySelect = driver.FindElement(By.Id("facultyId"));
        var submitButton = driver.FindElement(By.CssSelector("input[type='submit']"));
        
        nameField.Clear();
        nameField.SendKeys("Специальность в области искусственного интеллекта");
        descField.Clear();
        descField.SendKeys("Описание специальности в области искусственного интеллекта");
        countField.Clear();
        countField.SendKeys("5");
        
        submitButton.Click();
        
        var currentUrl = driver.Url;
        Assert.IsTrue(currentUrl.Contains("/Home/Specialities"));
        
        var specialtyRows = driver.FindElements(By.CssSelector("#specialityTable tbody tr"));
        bool specialtyFound = specialtyRows.Any(row => row.Text.Contains("Специальность в области искусственного интеллекта"));
        Assert.IsTrue(specialtyFound, "Отредактированная специальность не отображается в таблице");
        
        bool oldSpecialtyFound = specialtyRows.Any(row => row.Text.Contains("Специальность в области данных"));
        Assert.IsFalse(oldSpecialtyFound, "Старая специальность все еще отображается в таблице");
    }
    
    [Test]
    public void SpecialityTest_SuccessfulDeleteSpeciality()
    {
        driver.Navigate().GoToUrl("https://localhost:44327/Home/Enter");

        var usernameField = driver.FindElement(By.Name("login"));
        var passwordField = driver.FindElement(By.Name("password"));
        var loginButton = driver.FindElement(By.CssSelector("input[type='submit']"));

        usernameField.SendKeys("admin");
        passwordField.SendKeys("123456");
        loginButton.Click();

        wait.Until(d => d.Url == "https://localhost:44327/");
        driver.Navigate().GoToUrl("https://localhost:44327/Home/SpecialitySetting?id=31");
        
        var deleteButton = driver.FindElement(By.CssSelector("input[type='button'][value='Удалить']"));
        deleteButton.Click();
        
        var currentUrl = driver.Url;
        Assert.IsTrue(currentUrl.Contains("/Home/Specialities"));
        
        var specialtyRows = driver.FindElements(By.CssSelector("#specialityTable tbody tr"));
        bool specialtyFound = specialtyRows.Any(row => row.Text.Contains("Специальность в области искусственного интеллекта"));
        Assert.IsFalse(specialtyFound, "Удаленная специальность все еще отображается в таблице");
    }
    
    [Test]
    public void SpecialityTest_SuccessfulSearchSpeciality()
    {
        driver.Navigate().GoToUrl("https://localhost:44327/Home/Enter");

        var usernameField = driver.FindElement(By.Name("login"));
        var passwordField = driver.FindElement(By.Name("password"));
        var loginButton = driver.FindElement(By.CssSelector("input[type='submit']"));

        usernameField.SendKeys("admin");
        passwordField.SendKeys("123456");
        loginButton.Click();

        wait.Until(d => d.Url == "https://localhost:44327/");
        driver.Navigate().GoToUrl("https://localhost:44327/Home/Specialities");
        
        var searchInput = driver.FindElement(By.Id("searchInput"));
        var searchButton = driver.FindElement(By.CssSelector("button.btn.btn-primary"));
        
        searchInput.SendKeys("Программная инженерия");
        
        searchButton.Click();
        
        var resultRows = driver.FindElements(By.CssSelector("#specialityTable tbody tr"));
        Assert.IsTrue(resultRows.Any(row => row.Text.Contains("Программная инженерия")));
    }
    
    [Test]
    public void SpecialityTest_SuccessfulSortSpeciality()
    {
        driver.Navigate().GoToUrl("https://localhost:44327/Home/Enter");

        var usernameField = driver.FindElement(By.Name("login"));
        var passwordField = driver.FindElement(By.Name("password"));
        var loginButton = driver.FindElement(By.CssSelector("input[type='submit']"));

        usernameField.SendKeys("admin");
        passwordField.SendKeys("123456");
        loginButton.Click();

        wait.Until(d => d.Url == "https://localhost:44327/");
        driver.Navigate().GoToUrl("https://localhost:44327/Home/Specialities");
        
        var nameColumnHeader = driver.FindElement(By.CssSelector("th.text-info[onclick='sortTable(0)']"));
        
        nameColumnHeader.Click();
        
        Thread.Sleep(1000);
        
        var specialtyRowsAsc = driver.FindElements(By.CssSelector("#specialityTable tbody tr"));
        var namesAsc = specialtyRowsAsc.Select(row => row.FindElement(By.CssSelector("td")).Text).ToList();

        var namesAscSorted = namesAsc.OrderBy(name => name).ToList();
        CollectionAssert.AreEqual(namesAsc, namesAscSorted, "Сортировка по возрастанию не работает");
        
        nameColumnHeader.Click();
        
        Thread.Sleep(1000);
        
        var specialtyRowsDesc = driver.FindElements(By.CssSelector("#specialityTable tbody tr"));
        var namesDesc = specialtyRowsDesc.Select(row => row.FindElement(By.CssSelector("td")).Text).ToList();
        
        var namesDescSorted = namesDesc.OrderByDescending(name => name).ToList();
        CollectionAssert.AreEqual(namesDesc, namesDescSorted, "Сортировка по убыванию не работает");
    }
    
    [Test]
    public void SpecialityTest_NoRights()
    {
        driver.Navigate().GoToUrl("https://localhost:44327/Home/Enter");

        var usernameField = driver.FindElement(By.Name("login"));
        var passwordField = driver.FindElement(By.Name("password"));
        var loginButton = driver.FindElement(By.CssSelector("input[type='submit']"));

        usernameField.SendKeys("user1");
        passwordField.SendKeys("123456");
        loginButton.Click();

        wait.Until(d => d.Url == "https://localhost:44327/");
        driver.Navigate().GoToUrl("https://localhost:44327/Home/SpecialitySetting?id=1");
        
        var deleteButton = driver.FindElement(By.CssSelector("input[type='button'][value='Удалить']"));
        deleteButton.Click();
        wait.Until(d => driver.SwitchTo().Alert() != null);

        IAlert alert = driver.SwitchTo().Alert();
        string alertText = alert.Text;

        Assert.IsTrue(alertText.Contains("Недостаточно прав для данного действия"), "Ошибка не отображается корректно");
        alert.Accept();
    }
}

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Reqnroll;
using System;

namespace ReqnrollTestProject;

[Binding]
public class WebDriverHooks
{
    private readonly ScenarioContext _scenarioContext;

    public WebDriverHooks(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    [BeforeScenario("@ui")]
    public void SetupWebDriver()
    {
        var options = new ChromeOptions();
        var driver = new ChromeDriver(options);
        driver.Manage().Window.Maximize();
        _scenarioContext["WebDriver"] = driver;
    }

    [AfterScenario("@ui")]
    public void TearDownWebDriver()
    {
        if (_scenarioContext.TryGetValue("WebDriver", out IWebDriver? driver) && driver != null)
        {
            driver.Quit();
        }
    }

}  

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace ReqnrollTestProject.Pages
{
    public class Homepage
    {
        private readonly IWebDriver _driver;

        public Homepage(IWebDriver driver)
        {
            _driver = driver;
        }

        public void NavigateToHomepage()
        {
            _driver.Navigate().GoToUrl("https://ensekautomationcandidatetest.azurewebsites.net/");
            
        }

        public void ClickFindOutMore()
        {
            WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));

            IWebElement findOutMoreButton = wait.Until(driver =>
                driver.FindElement(By.CssSelector("p > a.btn.btn-primary.btn-lg"))
            );

            findOutMoreButton.Click();
        }

        public string GetCurrentUrl() => _driver.Url;
    }
}

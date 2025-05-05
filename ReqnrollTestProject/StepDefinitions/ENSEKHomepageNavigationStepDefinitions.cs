using System;
using Reqnroll;
using ReqnrollTestProject.Pages;
using OpenQA.Selenium;


namespace ReqnrollTestProject.StepDefinitions
{
    [Binding]
    public class ENSEKHomepageNavigationStepDefinitions

    {
        private readonly Homepage _homepage;
        public ENSEKHomepageNavigationStepDefinitions(ScenarioContext scenarioContext)
        {
            var driver = (IWebDriver)scenarioContext["WebDriver"];
            _homepage = new Homepage(driver);
        }

        [Given("ENSEK user is on the homepage")]
        public void GivenENSEKUserIsOnTheHomepage()
        {

            _homepage.NavigateToHomepage();
        }

        [When("user chooses Find out more")]
        public void WhenUserChoosesFindOutMore()
        {
            _homepage.ClickFindOutMore();
        }

        [Then("user should be redirected to the external homepage")]
        public void ThenUserShouldBeRedirectedToTheExternalHomepage()
        {
            var url = _homepage.GetCurrentUrl();
            if (!url.StartsWith("https://ensek.com"))
            {
                throw new Exception($"Unexpected URL: {url}");
            }
        }
    }
}

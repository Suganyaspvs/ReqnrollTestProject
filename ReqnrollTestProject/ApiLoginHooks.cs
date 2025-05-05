using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Reqnroll;
using RestSharp;

[Binding]
public class ApiLoginHooks
{
    private readonly ScenarioContext _scenarioContext;
    private const string BaseUrl = "https://qacandidatetest.ensek.io";

    public ApiLoginHooks(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    [BeforeScenario("@api")]
    public void AutoLogin()
    {
        var client = new RestClient(BaseUrl);
        var request = new RestRequest("/ENSEK/login", Method.Post);
        request.AddHeader("Content-Type", "application/json");
        request.AddJsonBody(new { username = "test", password = "testing" });

        var response = client.Execute(request);
        var token = JObject.Parse(response.Content)["access_token"]?.ToString();

        if (string.IsNullOrWhiteSpace(token))
            throw new Exception("Failed to acquire API token during hook login.");

        _scenarioContext["ApiToken"] = token;
    }
}


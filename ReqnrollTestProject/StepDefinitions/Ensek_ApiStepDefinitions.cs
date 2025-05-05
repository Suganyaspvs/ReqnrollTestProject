using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using NUnit.Framework;
using Reqnroll;
using System.Collections.Generic;
using System.Linq;
using RestSharp;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

[Binding]
public class EnsekApiSteps
{
    private readonly HttpClient _client = new();
    private HttpResponseMessage _response;
    private JsonElement _json;
    private readonly ScenarioContext _scenarioContext;
    private RestClient _restClient;
    private RestRequest _request;
    private RestResponse _restResponse;
    private string _token;
    private List<string> _orderIds = new();
    private const string BaseUrl = "https://qacandidatetest.ensek.io";
    private string _orderId;
    private RestResponse _createOrderResponse;
    private RestResponse _deleteResponse;

    public EnsekApiSteps(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
        _restClient = new RestClient(BaseUrl);
    }

    private void LoadTokenFromContext()
    {
        if (_scenarioContext.TryGetValue("ApiToken", out object tokenObj))
        {
            _token = tokenObj.ToString();
        }
        else
        {
            throw new Exception("API token not found in ScenarioContext. Make sure to call the login step first.");
        }
    }

    [Given("I have valid login credentials")]
    public void GivenIHaveValidLoginCredentials()
    {
        _request = new RestRequest("/ENSEK/login", Method.Post);
        _request.AddHeader("Content-Type", "application/json");
        _request.AddJsonBody(new { username = "test", password = "testing" });
    }

    [When("I call the login API")]
    public void WhenICallTheLoginAPI()
    {
        _restResponse = _restClient.Execute(_request);
        var token = JObject.Parse(_restResponse.Content)["access_token"]?.ToString();
        _token = token;
        _scenarioContext["ApiToken"] = token;
    }

    [Then("I should receive a valid token")]
    public void ThenIShouldReceiveAValidToken()
    {
        Assert.That(_token, Is.Not.Null.And.Length.GreaterThan(10));
    }

    [Given("I am authenticated")]
    [Given("I am authenticated to the ENSEK API")]
    public void GivenIAmAuthenticated()
    {
        LoadTokenFromContext();
    }

    [When("I buy (.*) units of energy with ID (.*)")]
    public void WhenIBuyUnitsOfEnergyWithID(int quantity, int fuelId)
    {
        LoadTokenFromContext();
        var request = new RestRequest($"/ENSEK/buy/{fuelId}/{quantity}", Method.Put);
        request.AddHeader("Authorization", $"Bearer {_token}");
        _restResponse = _restClient.Execute(request);

    }

    [Then("the purchase should be successful")]
    public void ThenThePurchaseShouldBeSuccessful()
    {
        Assert.That(_restResponse.IsSuccessful, Is.True, "Expected the purchase to succeed but it failed.");
        Assert.That(_restResponse.Content, Does.Contain("orderid").IgnoreCase, "Response does not contain a valid order ID.");
    }

    [Then("the purchase should not be successful")]
    public void ThenThePurchaseShouldNotBeSuccessful()
    {
        Assert.That(_restResponse.IsSuccessful, Is.False, "Unable to purchase");
    }

    [When("I send a POST request to \"(.*)\"")]
    public async Task WhenISendAPostRequest(string endpoint)
    {
        _response = await _client.PostAsync(BaseUrl + endpoint, null);
    }

    [Then("the response status code should be 200")]
    public void ThenTheResponseCodeShouldBe200()
    {
        Assert.AreEqual(200, (int)_response.StatusCode);
    }


    [When("I send a GET request to \"(.*)\"")]
    public async Task WhenISendAGetRequest(string endpoint)
    {
        _response = await _client.GetAsync(BaseUrl + endpoint);
        var body = await _response.Content.ReadAsStringAsync();
        _json = JsonSerializer.Deserialize<JsonElement>(body);
    }

    [Then("each order I placed should appear in the response")]
    public void ThenEachOrderShouldAppear()
    {
        var allIds = _json.EnumerateArray().Select(o => o.TryGetProperty("id", out var idVal) ? idVal.GetString() : "").ToList();
        foreach (var expectedId in _orderIds)
        {
            Assert.Contains(expectedId, allIds);
        }
    }

    [Then("I count how many orders were created before today")]
    public void ThenCountOrdersBeforeToday()
    {
        var now = DateTime.UtcNow;
        int count = _json.EnumerateArray().Count(order =>
            DateTime.TryParse(order.GetProperty("time").GetString(), out var orderDate) && orderDate < now.Date);
        TestContext.WriteLine($"Orders before today: {count}");
        Assert.GreaterOrEqual(count, 0);
    }


    [Given("I have a valid order ID from a new purchase")]
    public void GivenIHaveAValidOrderIdFromANewPurchase()
    {
        LoadTokenFromContext();
        int testFuelId = 1;
        int quantity = 1;

        var request = new RestRequest($"/ENSEK/buy/{testFuelId}/{quantity}", Method.Put);
        request.AddHeader("Authorization", $"Bearer {_token}");

        _createOrderResponse = _restClient.Execute(request);
        Assert.That(_createOrderResponse.IsSuccessful, "Failed to place order");

        Console.WriteLine("Order creation response: " + _createOrderResponse.Content);

        // ✅ Parse the JSON response and extract the message
        var json = JsonDocument.Parse(_createOrderResponse.Content);
        var message = json.RootElement.GetProperty("message").GetString();

        // ✅ Match the correct text: "order id" with a space
        var match = Regex.Match(message, @"order id is ([a-f0-9\-]+)", RegexOptions.IgnoreCase);
        Assert.That(match.Success, Is.True, "Order ID not found in message");

        _orderId = match.Groups[1].Value;
    }

    [When("I delete the order")]
    public void WhenIDeleteTheOrder()
    {
        var request = new RestRequest($"/ENSEK/orders/{_orderId}", Method.Delete);
        request.AddHeader("Authorization", $"Bearer {_token}");

        _deleteResponse = _restClient.Execute(request);
    }
    [Then("the order should be deleted successfully")]
    public void ThenTheOrderShouldBeDeletedSuccessfully()
    {
        Assert.That(_deleteResponse.IsSuccessful, Is.True, "Delete request was not successful.");
        Assert.That((int)_deleteResponse.StatusCode, Is.EqualTo(200).Or.EqualTo(204), "Expected a success status code.");
    }

}
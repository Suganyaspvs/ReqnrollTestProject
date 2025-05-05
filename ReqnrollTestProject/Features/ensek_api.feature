@api @ensek

Feature: ENSEK API Testing
  
  As a tester
  I want to validate the core functionality of the ENSEK API
  So that I can confirm the API behaves correctly under normal conditions

Feature: Login to ENSEK API
@smoke
Scenario: Successfully login and get token
	Given I have valid login credentials
	When I call the login API
	Then I should receive a valid token

@smoke
Scenario: Buy energy using a valid  ID
	When I buy 12 units of energy with ID 1
	And I buy 12 units of energy with ID 2
	And I buy 12 units of energy with ID 3
	And I buy 12 units of energy with ID 4
	Then the purchase should be successful

@smoke
Scenario: Verify that each order is listed in /orders
	When I send a GET request to "/ENSEK/orders"
	Then each order I placed should appear in the response

@smoke
Scenario: Count orders created before today
	When I send a GET request to "/ENSEK/orders"
	Then I count how many orders were created before today

@negative 	
Scenario: Buying energy with invalid id
	When I buy 12 units of energy with ID 5
	Then the purchase should not be successful

Scenario: Check if user is able to buy 0 units using a valid energy ID
	When I buy 0 units of energy with ID 4
	Then the purchase should be successful

@smoke
Scenario: Reset the test data
	When I send a POST request to "/ENSEK/reset"
	Then the response status code should be 200

@negative 
Scenario: Check if user is able to buy negative units using a valid energy ID
	When I buy -3 units of energy with ID 1
	Then the purchase should not be successful

Scenario: Delete a valid order
  Given I have a valid order ID from a new purchase
  When I delete the order
  Then the order should be deleted successfully
@ui

Feature: ENSEK Homepage Navigation

  Scenario: Redirects to external homepage

    Given ENSEK user is on the homepage
    When user chooses Find out more
    Then user should be redirected to the external homepage



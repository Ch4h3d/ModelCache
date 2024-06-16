param (
	$configuration = "Debug",
	$nugetSource = "",
	$apiKey = "",
	[switch]$showWarnings
)

Import-Module OlReliable-Module
$projectNames = @("ModelCache") # TuDu

Publish-Nugget -configuration $configuration -nugetSource $nugetSource -apiKey $apiKey -showWarnings $showWarnings
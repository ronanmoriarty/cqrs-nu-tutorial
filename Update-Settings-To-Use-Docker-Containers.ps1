. .\src\CQRS\PowerShell\Docker.ps1
. .\src\CQRS\PowerShell\FileOperations.ps1

function GetWaiterWebsiteUrl()
{
    $waiterWebsiteContainerId = GetContainerRunningWithImageName "cqrs-nu-tutorial_cafe-waiter-web"
    $waiterWebsiteIpAddress = GetIpAddress $waiterWebsiteContainerId
    return "http://$waiterWebsiteIpAddress"
}

function GetWaiterWebsitePassword()
{
    return GetEnvironmentVariableFromEnvFile "waiterWebsitePassword"
}

function GetCommandServicePassword()
{
    return GetEnvironmentVariableFromEnvFile "commandServicePassword"
}

function GetEventProjectingServicePassword()
{
    return GetEnvironmentVariableFromEnvFile "eventProjectingServicePassword"
}

function GetKeyValuePairs()
{
    $rabbitMqServerAddress = GetRabbitMqAddress
    $writeModelSqlServerAddress = GetWriteModelSqlServerAddress
    $readModelSqlServerAddress = GetReadModelSqlServerAddress
    $waiterWebsitePassword = GetWaiterWebsitePassword
    $commandServicePassword = GetCommandServicePassword
    $eventProjectingServicePassword = GetEventProjectingServicePassword
    $waiterWebsiteUrl = GetWaiterWebsiteUrl

    $keyValuePairs = @{}
    $keyValuePairs.Add("`$rabbitMqServerAddress", $rabbitMqServerAddress)
    $keyValuePairs.Add("`$rabbitMqUsername", "guest")
    $keyValuePairs.Add("`$rabbitMqPassword", "guest")
    $keyValuePairs.Add("`$writeModelSqlServerAddress", $writeModelSqlServerAddress)
    $keyValuePairs.Add("`$readModelSqlServerAddress", $readModelSqlServerAddress)
    $keyValuePairs.Add("`$waiterWebsitePassword", $waiterWebsitePassword)
    $keyValuePairs.Add("`$commandServicePassword", $commandServicePassword)
    $keyValuePairs.Add("`$eventProjectingServicePassword", $eventProjectingServicePassword)
    $keyValuePairs.Add("`$waiterWebsiteUrl", $waiterWebsiteUrl)
    return $keyValuePairs
}

$keyValuePairs = GetKeyValuePairs

$keyValuePairs.Keys | ForEach-Object {
    Write-Output "$($_): $($keyValuePairs[$_])"
}

SwapPlaceholdersInExampleFilesToCreateNewDockerJsonFiles .\src\Cafe\ appSettings.override.json.example appSettings.override.json $keyValuePairs

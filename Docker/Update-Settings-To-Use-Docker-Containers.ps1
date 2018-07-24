[CmdletBinding()]
param (
    [Parameter(Mandatory=$True)]
    [string] $userName,
    [Parameter(Mandatory=$True)]
    [SecureString] $password
)

function GetContainerRunningWithImageName($imageName){
    return docker container list --filter ancestor=$imageName --format "{{.ID}}"
}

function GetIpAddress($containerId){
    return docker inspect -f '{{range .NetworkSettings.Networks}}{{.IPAddress}}{{end}}' $containerId
}

function GetFullPath($relativePath){
    return [System.IO.Path]::GetFullPath([System.IO.Path]::Combine($PSScriptRoot, $relativePath))
}

function ConvertToPlainText([SecureString]$secureString){
    $BSTR = [System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($secureString)
    return [System.Runtime.InteropServices.Marshal]::PtrToStringAuto($BSTR)
}

function GetCQRSDALSettings($writeModelSqlServerAddress, $username, [SecureString] $secureStringPassword){
    $password = ConvertToPlainText $secureStringPassword
    return @"
{
    "connectionString": "Server=$writeModelSqlServerAddress;Database=CQRSTutorial.Cafe.Waiter.WriteModel;User Id=$username;Password=$password;"
}
"@
}

function GetWaiterWebsiteSettings($rabbitMqServerAddress, $readModelSqlServerAddress, $username, [SecureString] $secureStringPassword){
    $password = ConvertToPlainText $secureStringPassword
    return @"
{
    "rabbitmq": {
        "uri": "rabbitmq://$rabbitMqServerAddress",
        "username": "guest",
        "password": "guest"
    },
    "connectionString": "Server=$readModelSqlServerAddress;Database=CQRSTutorial.Cafe.Waiter.ReadModel;User Id=$username;Password=$password;"
}
"@
}

function GetWaiterWebsiteTestSettings($readModelSqlServerAddress, $username, [SecureString] $secureStringPassword){
    $password = ConvertToPlainText $secureStringPassword
    return @"
{
    "connectionString": "Server=$readModelSqlServerAddress;Database=CQRSTutorial.Cafe.Waiter.ReadModel;User Id=$username;Password=$password;"
}
"@
}

function GetWaiterCommandServiceSettings($rabbitMqServerAddress, $writeModelSqlServerAddress, $username, [SecureString]$secureStringPassword){
    $password = ConvertToPlainText $secureStringPassword
    return @"
{
    "rabbitmq": {
        "uri": "rabbitmq://$rabbitMqServerAddress",
        "username": "guest",
        "password": "guest"
    },
    "connectionString": "Server=$writeModelSqlServerAddress;Database=CQRSTutorial.Cafe.Waiter.WriteModel;User Id=$username;Password=$password;"
}
"@
}

function GetWaiterCommandServiceTestSettings($writeModelSqlServerAddress, $username, [SecureString]$secureStringPassword){
    $password = ConvertToPlainText $secureStringPassword
    return @"
{
    "connectionString": "Server=$writeModelSqlServerAddress;Database=CQRSTutorial.Cafe.Waiter.WriteModel;User Id=$username;Password=$password;"
}
"@
}

function GetWaiterEventProjectingServiceSettings($rabbitMqServerAddress, $readModelSqlServerAddress, $username, [SecureString]$secureStringPassword){
    $password = ConvertToPlainText $secureStringPassword
    return @"
{
    "rabbitmq": {
        "uri": "rabbitmq://$rabbitMqServerAddress",
        "username": "guest",
        "password": "guest"
    },
    "connectionString": "Server=$readModelSqlServerAddress;Database=CQRSTutorial.Cafe.Waiter.ReadModel;User Id=$username;Password=$password;"
}
"@
}

function GetWaiterEventProjectingServiceTestSettings($readModelSqlServerAddress, $username, [SecureString]$secureStringPassword){
    $password = ConvertToPlainText $secureStringPassword
    return @"
{
    "connectionString": "Server=$readModelSqlServerAddress;Database=CQRSTutorial.Cafe.Waiter.ReadModel;User Id=$username;Password=$password;"
}
"@
}

function GetWaiterAcceptanceTestsSettings($readModelSqlServerAddress, $username, [SecureString] $secureStringPassword){
    $password = ConvertToPlainText $secureStringPassword
    return @"
{
    "connectionString": "Server=$readModelSqlServerAddress;Database=CQRSTutorial.Cafe.Waiter.ReadModel;User Id=$username;Password=$password;"
}
"@
}

function WriteToFile($path, $contents){
    Write-Output "Writing $path..."
    Write-Output "Text: $contents"
    $_.Text | Out-File -encoding ASCII $path
}

function GetRabbitMqAddress(){
    $rabbitMqContainerId = GetContainerRunningWithImageName "rabbitmq"
    $rabbitMqServerIpAddress = GetIpAddress $rabbitMqContainerId
    return $rabbitMqServerIpAddress
}

function GetWriteModelSqlServerAddress(){
    $writeModelSqlServerContainerId = GetContainerRunningWithImageName "cqrs-write-db-server"
    $writeModelSqlServerIpAddress = GetIpAddress $writeModelSqlServerContainerId
    return $writeModelSqlServerIpAddress
}

function GetReadModelSqlServerAddress(){
    $readModelSqlServerContainerId = GetContainerRunningWithImageName "cqrs-read-db-server"
    $readModelSqlServerIpAddress = GetIpAddress $readModelSqlServerContainerId
    return $readModelSqlServerIpAddress
}

$rabbitMqServerIpAddress = GetRabbitMqAddress
Write-Output "`$rabbitMqServerIpAddress:$rabbitMqServerIpAddress"
$writeModelSqlServerIpAddress = GetWriteModelSqlServerAddress
Write-Output "`$writeModelSqlServerIpAddress: $writeModelSqlServerIpAddress"
$readModelSqlServerIpAddress = GetReadModelSqlServerAddress
Write-Output "`$readModelSqlServerIpAddress: $readModelSqlServerIpAddress"

$configuration = "Debug"

$cqrsDALTests = @{
    FilePath = "..\src\CQRS\CQRSTutorial.DAL.Tests\bin\$configuration\netcoreapp2.0\appSettings.override.json"
    Text = GetCQRSDALSettings $writeModelSqlServerIpAddress $username $password
}

$waiterWebsite = @{
    FilePath = "..\src\Cafe\Cafe.Waiter.Web\appSettings.override.json"
    Text = GetWaiterWebsiteSettings $rabbitMqServerIpAddress $readModelSqlServerIpAddress $username $password
}

$waiterWebsiteTest = @{
    FilePath = "..\src\Cafe\Cafe.Waiter.Web.Tests\bin\$configuration\netcoreapp2.0\appSettings.override.json"
    Text = GetWaiterWebsiteTestSettings $readModelSqlServerIpAddress $username $password
}

$waiterCommandService = @{
    FilePath = "..\src\Cafe\Cafe.Waiter.Command.Service\bin\$configuration\netcoreapp2.0\appSettings.override.json";
    Text = GetWaiterCommandServiceSettings $rabbitMqServerIpAddress $writeModelSqlServerIpAddress $username $password
}

$waiterCommandServiceTest = @{
    FilePath = "..\src\Cafe\Cafe.Waiter.Command.Service.Tests\bin\$configuration\netcoreapp2.0\appSettings.override.json";
    Text = GetWaiterCommandServiceTestSettings $writeModelSqlServerIpAddress $username $password
}

$waiterEventProjectingService = @{
    FilePath = "..\src\Cafe\Cafe.Waiter.EventProjecting.Service\bin\$configuration\netcoreapp2.0\appSettings.override.json"
    Text = GetWaiterEventProjectingServiceSettings $rabbitMqServerIpAddress $readModelSqlServerIpAddress $username $password
}

$waiterEventProjectingServiceTest = @{
    FilePath = "..\src\Cafe\Cafe.Waiter.EventProjecting.Service.Tests\bin\$configuration\netcoreapp2.0\appSettings.override.json"
    Text = GetWaiterEventProjectingServiceTestSettings $readModelSqlServerIpAddress $username $password
}

$waiterAcceptanceTest = @{
    FilePath = "..\src\Cafe\Cafe.Waiter.AcceptanceTests\bin\$configuration\netcoreapp2.0\appSettings.override.json";
    Text = GetWaiterAcceptanceTestsSettings $readModelSqlServerIpAddress $username $password
}

$appSettings = @(
    $cqrsDALTests,
    $waiterWebsite,
    $waiterWebsiteTest,
    $waiterCommandService,
    $waiterCommandServiceTest,
    $waiterEventProjectingService,
    $waiterEventProjectingServiceTest
    $waiterAcceptanceTest
)

$appSettings | ForEach-Object {
    $path = GetFullPath $_.FilePath
    if(Test-Path $path) {
        Remove-Item $path
    }

    WriteToFile $path $_.Text
}

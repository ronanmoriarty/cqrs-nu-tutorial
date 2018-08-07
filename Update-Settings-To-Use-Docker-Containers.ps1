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

function GetCQRSDALSettings($writeModelConnectionString){
    return @"
{
    "connectionString": "$writeModelConnectionString"
}
"@
}

function GetWaiterWebsiteSettings($rabbitMqServerAddress, $waiterWebsiteConnectionString){
    return @"
{
    "rabbitmq": {
        "uri": "rabbitmq://$rabbitMqServerAddress",
        "username": "guest",
        "password": "guest"
    },
    "connectionString": "$waiterWebsiteConnectionString"
}
"@
}

function GetWaiterWebsiteTestSettings($waiterWebsiteConnectionString){
    return @"
{
    "connectionString": "$waiterWebsiteConnectionString"
}
"@
}

function GetWaiterCommandServiceSettings($rabbitMqServerAddress, $writeModelConnectionString){
    return @"
{
    "rabbitmq": {
        "uri": "rabbitmq://$rabbitMqServerAddress",
        "username": "guest",
        "password": "guest"
    },
    "connectionString": "$writeModelConnectionString"
}
"@
}

function GetWaiterCommandServiceTestSettings($rabbitMqServerAddress, $writeModelConnectionString){
    return @"
{
    "rabbitmq": {
        "uri": "rabbitmq://$rabbitMqServerAddress",
        "username": "guest",
        "password": "guest"
    },
    "connectionString": "$writeModelConnectionString"
}
"@
}

function GetWaiterEventProjectingServiceSettings($rabbitMqServerAddress, $eventProjectingServiceConnectionString){
    return @"
{
    "rabbitmq": {
        "uri": "rabbitmq://$rabbitMqServerAddress",
        "username": "guest",
        "password": "guest"
    },
    "connectionString": "$eventProjectingServiceConnectionString"
}
"@
}

function GetWaiterEventProjectingServiceTestSettings($eventProjectingServiceConnectionString){
    return @"
{
    "connectionString": "$eventProjectingServiceConnectionString"
}
"@
}

function GetWaiterAcceptanceTestsSettings($readModelConnectionString, $waiterWebsiteUrl){
    return @"
{
    "connectionString": "$readModelConnectionString",
    "cafe": {
      "waiter": {
        "web": {
          "url": "$waiterWebsiteUrl"
        }
      }
    }
}
"@
}

function WriteToFile($path, $contents){
    Write-Output "Writing $path..."
    Write-Output "Text: $contents"
    $contents | Out-File -encoding ASCII $path
}

function GetRabbitMqAddress(){
    $rabbitMqContainerId = GetContainerRunningWithImageName "cqrs-nu-tutorial_rabbitmq"
    $rabbitMqServerIpAddress = GetIpAddress $rabbitMqContainerId
    return $rabbitMqServerIpAddress
}

function GetWriteModelSqlServerAddress(){
    $writeModelSqlServerContainerId = GetContainerRunningWithImageName "cqrs-nu-tutorial_waiter-write-db-server"
    $writeModelSqlServerIpAddress = GetIpAddress $writeModelSqlServerContainerId
    return $writeModelSqlServerIpAddress
}

function GetReadModelSqlServerAddress(){
    $readModelSqlServerContainerId = GetContainerRunningWithImageName "cqrs-nu-tutorial_waiter-read-db-server"
    $readModelSqlServerIpAddress = GetIpAddress $readModelSqlServerContainerId
    return $readModelSqlServerIpAddress
}

function GetWaiterWebsiteUrl()
{
    $waiterWebsiteContainerId = GetContainerRunningWithImageName "cqrs-nu-tutorial_cafe-waiter-web"
    $waiterWebsiteIpAddress = GetIpAddress $waiterWebsiteContainerId
    return "http://$waiterWebsiteIpAddress"
}

function GetWaiterWebsiteConnectionString([string] $password)
{
    $readModelSqlServerAddress = GetReadModelSqlServerAddress
    return "Server=$readModelSqlServerAddress;Database=CQRSTutorial.Cafe.Waiter.ReadModel;User Id=WaiterWebsite;Password=$password;"
}

function GetCommandServiceConnectionString([string] $password)
{
    $writeModelSqlServerAddress = GetWriteModelSqlServerAddress
    return "Server=$writeModelSqlServerAddress;Database=CQRSTutorial.Cafe.Waiter.WriteModel;User Id=CommandService;Password=$password;"
}

function GetEventProjectingServiceConnectionString([string] $password)
{
    $readModelSqlServerAddress = GetReadModelSqlServerAddress
    return "Server=$readModelSqlServerAddress;Database=CQRSTutorial.Cafe.Waiter.ReadModel;User Id=EventProjectingService;Password=$password;"
}

function GetWaiterWebsitePassword()
{
    return [regex]::Match((Get-Content .env),"waiterWebsitePassword='([^=]*)'").captures.groups[1].value
}

function GetCommandServicePassword()
{
    return [regex]::Match((Get-Content .env),"commandServicePassword='([^=]*)'").captures.groups[1].value
}

function GetEventProjectingServicePassword()
{
    return [regex]::Match((Get-Content .env),"eventProjectingServicePassword='([^=]*)'").captures.groups[1].value
}

$rabbitMqServerIpAddress = GetRabbitMqAddress
$waiterWebsitePassword = GetWaiterWebsitePassword
$commandServicePassword = GetCommandServicePassword
$eventProjectingServicePassword = GetEventProjectingServicePassword
Write-Output "`$rabbitMqServerIpAddress:$rabbitMqServerIpAddress"
$waiterWebsiteConnectionString = GetWaiterWebsiteConnectionString $waiterWebsitePassword
Write-Output "`$waiterWebsiteConnectionString: $waiterWebsiteConnectionString"
$commandServiceConnectionString = GetCommandServiceConnectionString $commandServicePassword
Write-Output "`$commandServiceConnectionString: $commandServiceConnectionString"
$eventProjectingServiceConnectionString = GetEventProjectingServiceConnectionString $eventProjectingServicePassword
Write-Output "`$eventProjectingServiceConnectionString: $eventProjectingServiceConnectionString"
$waiterWebsiteUrl = GetWaiterWebsiteUrl
Write-Output "`$waiterWebsiteUrl: $waiterWebsiteUrl"

$configuration = "Debug"

$cafeDALTests = @{
    FilePath = ".\src\Cafe\Cafe.DAL.Tests\bin\$configuration\netcoreapp2.0\appSettings.override.json"
    Text = GetCQRSDALSettings $commandServiceConnectionString
}

$waiterWebsite = @{
    FilePath = ".\src\Cafe\Cafe.Waiter.Web\appSettings.override.json"
    Text = GetWaiterWebsiteSettings $rabbitMqServerIpAddress $waiterWebsiteConnectionString
}

$waiterWebsiteTest = @{
    FilePath = ".\src\Cafe\Cafe.Waiter.Web.Tests\bin\$configuration\netcoreapp2.1\appSettings.override.json"
    Text = GetWaiterWebsiteTestSettings $eventProjectingServiceConnectionString
}

$waiterCommandService = @{
    FilePath = ".\src\Cafe\Cafe.Waiter.Command.Service\bin\$configuration\netcoreapp2.1\appSettings.override.json";
    Text = GetWaiterCommandServiceSettings $rabbitMqServerIpAddress $commandServiceConnectionString
}

$waiterCommandServiceTest = @{
    FilePath = ".\src\Cafe\Cafe.Waiter.Command.Service.Tests\bin\$configuration\netcoreapp2.1\appSettings.override.json";
    Text = GetWaiterCommandServiceTestSettings $rabbitMqServerIpAddress $commandServiceConnectionString
}

$waiterEventProjectingService = @{
    FilePath = ".\src\Cafe\Cafe.Waiter.EventProjecting.Service\bin\$configuration\netcoreapp2.1\appSettings.override.json"
    Text = GetWaiterEventProjectingServiceSettings $rabbitMqServerIpAddress $eventProjectingServiceConnectionString
}

$waiterEventProjectingServiceTest = @{
    FilePath = ".\src\Cafe\Cafe.Waiter.EventProjecting.Service.Tests\bin\$configuration\netcoreapp2.1\appSettings.override.json"
    Text = GetWaiterEventProjectingServiceTestSettings $eventProjectingServiceConnectionString
}

$waiterAcceptanceTest = @{
    FilePath = ".\src\Cafe\Cafe.Waiter.AcceptanceTests\bin\$configuration\netcoreapp2.1\appSettings.override.json";
    Text = GetWaiterAcceptanceTestsSettings $eventProjectingServiceConnectionString $waiterWebsiteUrl
}

$appSettings = @(
    $cafeDALTests,
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
$rabbitMqContainerId = docker container list --filter ancestor=rabbitmq --format "{{.ID}}"
$rabbitMqServerIpAddress = docker inspect -f '{{range .NetworkSettings.Networks}}{{.IPAddress}}{{end}}' $rabbitMqContainerId
Write-Output "`$rabbitMqServerIpAddress:$rabbitMqServerIpAddress"

$appSettingsFiles = @(
    "..\src\Cafe\Cafe.Waiter.Web\appsettings.override.json",
    "..\src\Cafe\Cafe.Waiter.Command.Service\appsettings.override.json",
    "..\src\Cafe\Cafe.Waiter.EventProjecting.Service\appsettings.override.json"
)

$appSettingsFiles | ForEach-Object {
    $path = [System.IO.Path]::GetFullPath([System.IO.Path]::Combine($PSScriptRoot, $_))
    if(Test-Path $path) {
        Remove-Item $path
    }

    $text = @"
{
    "rabbitmq": {
        "uri": "$rabbitMqServerIpAddress",
        "username": "guest",
        "password": "guest"
    }
}
"@

    $text | Out-File -encoding ASCII $path
    Write-Output "Created $path"
}

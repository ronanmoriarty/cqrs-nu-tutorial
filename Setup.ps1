[CmdletBinding()]
param (
    [Parameter(Mandatory=$True)]
    [SecureString] $saPassword,
    [Parameter(Mandatory=$True)]
    [SecureString] $waiterWebsitePassword,
    [Parameter(Mandatory=$True)]
    [SecureString] $commandServicePassword,
    [Parameter(Mandatory=$True)]
    [SecureString] $eventProjectingServicePassword
)

function ConvertToPlainText([SecureString]$secureString){
    $BSTR = [System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($secureString)
    return [System.Runtime.InteropServices.Marshal]::PtrToStringAuto($BSTR)
}

function GetFullPath($relativePath){
    return [System.IO.Path]::GetFullPath([System.IO.Path]::Combine($PSScriptRoot, $relativePath))
}

function CreateEnvFile()
{
    Write-Output "sa_password=$saPasswordPlainText" | Out-File -encoding ASCII .env
    Write-Output "waiterWebsitePassword='$waiterWebsitePasswordPlainText'" | Out-File -encoding ASCII -Append .env
    Write-Output "commandServicePassword='$commandServicePasswordPlainText'" | Out-File -encoding ASCII -Append .env
    Write-Output "eventProjectingServicePassword='$eventProjectingServicePasswordPlainText'" | Out-File -encoding ASCII -Append .env
    Write-Output "Created $(GetFullPath .env)"
    Write-Output "sa_password=$saPasswordPlainText" | Out-File -encoding ASCII .\src\CQRS\.env
    Write-Output "commandServicePassword='$commandServicePasswordPlainText'" | Out-File -encoding ASCII -Append .\src\CQRS\.env
    Write-Output "Created $(GetFullPath .\src\CQRS\.env)"
}

$saPasswordPlainText = ConvertToPlainText $saPassword
$waiterWebsitePasswordPlainText = ConvertToPlainText $waiterWebsitePassword
$commandServicePasswordPlainText = ConvertToPlainText $commandServicePassword
$eventProjectingServicePasswordPlainText = ConvertToPlainText $eventProjectingServicePassword

CreateEnvFile

function GetExampleFileWithPlaceholdersReplaced($filePath)
{
    $temp = (Get-Content $filePath).Replace("`$rabbitMqPassword", "guest")
    $temp = $temp.Replace("`$waiterWebsitePassword", "$waiterWebsitePasswordPlainText")
    $temp = $temp.Replace("`$commandServicePassword", "$commandServicePasswordPlainText")
    return $temp.Replace("`$eventProjectingServicePassword", "$eventProjectingServicePasswordPlainText")
}

function SwapPlaceholdersInExampleFilesToCreateNewDockerJsonFiles()
{
    Get-ChildItem -Path .\src\Cafe\ -Filter *.example -Recurse | ForEach-Object {
        $exampleFile = $_.FullName
        $dockerJsonPath = $exampleFile.Replace(".example", "")
        if(Test-Path $dockerJsonPath)
        {
            Remove-Item $dockerJsonPath
        }

        (GetExampleFileWithPlaceholdersReplaced $exampleFile) | Set-Content $dockerJsonPath
        Write-Output "Created $dockerJsonPath"
    }
}

SwapPlaceholdersInExampleFilesToCreateNewDockerJsonFiles
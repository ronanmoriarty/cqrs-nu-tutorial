Invoke-Expression(Invoke-WebRequest 'https://chocolatey.org/install.ps1' -UseBasicParsing | Select-Object -Expand Content)
#escape=`
FROM microsoft/mssql-server-windows-express
WORKDIR /app
ENV sa_password=_
ENV eventProjectingServicePassword=_
ENV waiterWebsitePassword=_
COPY .\ReadModel\Scripts .\scripts
COPY .\PowerShell .\powershell
EXPOSE 1433
CMD $parameters = @(\"eventProjectingServicePassword=$env:eventProjectingServicePassword\",\"waiterWebsitePassword=$env:waiterWebsitePassword\"); .\powershell\setup.ps1 -sa_password $env:sa_password -Database "Cafe.Waiter.ReadModel" -DatabaseFolder "C:\app\databases" -DatabaseScriptFolder "C:\app\scripts" -Parameters $env:parameters
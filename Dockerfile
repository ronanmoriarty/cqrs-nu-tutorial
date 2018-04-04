#escape=`
FROM microsoft/mssql-server-windows-express
WORKDIR /app
COPY .\src\Scripts\Cafe.Waiter.WriteModel .\scripts
COPY .\src\PowerShell .\powershell
SHELL [ "powershell" ]
RUN powershell\setupDatabase.ps1 'CQRSTutorial.Cafe.Waiter.WriteModel' 'C:\app\databases'
ENTRYPOINT [ "powershell" ]
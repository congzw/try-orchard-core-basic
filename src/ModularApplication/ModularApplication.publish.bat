call dotnet restore -r win-x64
call dotnet publish -p:IsPublish=true -c Release -r win-x64 --self-contained false --no-restore --output bin/publish/ModularApplication/win-x64

pause
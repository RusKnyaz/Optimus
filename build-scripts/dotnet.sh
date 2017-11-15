version="2.0.$BUILD_NUMBER-dev";
fileversion="2.0.$BUILD_NUMBER.1";
/usr/bin/dotnet build source/Knyaz.Optimus.sln /p:FrameworkPathOverride=/usr/lib/mono/4.5/ /p:Version=$version /p:FileVersion=$fileversion -c Debug -v n
/usr/bin/dotnet test source/Knyaz.Optimus.sln --no-build /p:FrameworkPathOverride=/usr/lib/mono/4.5/ -f netcoreapp2.0 -r test-results --logger "trx;LogFileName=Optimus.netcore.trx"
/usr/bin/dotnet build source/Knyaz.Optimus.sln /p:FrameworkPathOverride=/usr/lib/mono/4.5/ /p:Version=$version /p:FileVersion=$fileversion -c Release -v n
/usr/bin/dotnet pack source/Knyaz.Optimus/Knyaz.Optimus.csproj -c Release /p:Version=$version --no-build /p:FrameworkPathOverride=/usr/lib/mono/4.5/

version="2.0.$BUILD_NUMBER-dev";
fileversion="2.0.$BUILD_NUMBER.1";
echo $version
/user/bin/dotnet build source/Knyaz.Optimus.sln /p:Version=$version /p:FileVersion=$fileversion
/user/bin/dotnet test source/Knyaz.Optimus.sln
/user/bin/dotnet pack source/Knyaz.Optimus/Knyaz.Optimus.csproj -c Release /p:Version=$version


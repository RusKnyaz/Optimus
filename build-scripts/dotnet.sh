version="2.0.$BUILD_NUMBER-dev";
fileversion="2.0.$BUILD_NUMBER.1";
echo $version
dotnet build source/Knyaz.Optimus.sln /p:Version=$version /p:FileVersion=$fileversion
dotnet test source/Knyaz.Optimus.sln
dotnet pack source/Knyaz.Optimus/Knyaz.Optimus.csproj -c Release /p:Version=$version


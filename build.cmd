set suff=%0

dotnet build -c Release source/Knyaz.Optimus/Knyaz.Optimus.csproj /p:VersionSuffix=%suff%

dotnet build -c Release source/Knyaz.Optimus.Tests/Knyaz.Optimus.Tests.csproj /p:VersionSuffix=%suff%

dotnet pack -c Release source/Knyaz.Optimus/Knyaz.Optimus.csproj /p:VersionSuffix=%suff% --no-build
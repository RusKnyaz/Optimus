set suff=%1

dotnet build -c Release source/Knyaz.Optimus/Knyaz.Optimus.csproj /p:VersionSuffix=%suff%

dotnet build -c Release source/Knyaz.Optimus.Tests/Knyaz.Optimus.Tests.csproj /p:VersionSuffix=%suff%

dotnet build -c Release source/Knyaz.Optimus.Scripting.Jurassic/Knyaz.Optimus.Scripting.Jurassic.csproj /p:VersionSuffix=%suff%

dotnet pack -c Release source/Knyaz.Optimus/Knyaz.Optimus.csproj /p:VersionSuffix=%suff% --no-build

dotnet pack -c Release source/Knyaz.Optimus.Scripting.Jurassic/Knyaz.Optimus.Scripting.Jurassic.csproj /p:VersionSuffix=%suff% --no-build

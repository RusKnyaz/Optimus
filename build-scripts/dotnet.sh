versionPref=${BRANCH:1}
versionDev="${BRANCH:0:1}"
if [ "$versionDev" == "d" ]; then
	version="$versionPref.$BUILD_NUMBER-dev"
    fileversion="$versionPref.$BUILD_NUMBER.1"
else
	version="$versionPref.$BUILD_NUMBER"
    fileversion="$versionPref.$BUILD_NUMBER.0"
fi
echo "Starting build of version: $version, fileVersion: $fileversion"
echo "Step 1. Clean"
/bin/rm -f source/Knyaz.Optimus.Tests/test-results/*
/bin/rm -rf source/Knyaz.Optimus/bin/*
/bin/rm -rf source/Knyaz.Optimus/obj/*
/bin/rm -rf source/Knyaz.Optimus.Tests/bin/*
/bin/rm -rf source/Knyaz.Optimus.Tests/obj/*
echo "Step 2. Restore packages"
/usr/bin/dotnet restore source/Knyaz.Optimus/Knyaz.Optimus.csproj
/usr/bin/dotnet restore source/Knyaz.Optimus.Tests/Knyaz.Optimus.Tests.csproj
echo "Step 3. Build Knyaz.Optimus"
/usr/bin/dotnet build source/Knyaz.Optimus/Knyaz.Optimus.csproj /p:FrameworkPathOverride=/usr/lib/mono/4.5/ /p:Version=$version /p:FileVersion=$fileversion -c Release -v n
echo "Step 4. Build Knyaz.Optimus.Tests"
/usr/bin/dotnet build source/Knyaz.Optimus.Tests/Knyaz.Optimus.Tests.csproj /p:FrameworkPathOverride=/usr/lib/mono/4.5/ /p:Version=$version /p:FileVersion=$fileversion -c Release -v n
echo "Step 5. Run tests"
/usr/bin/dotnet test source/Knyaz.Optimus.Tests/Knyaz.Optimus.Tests.csproj --no-build /p:FrameworkPathOverride=/usr/lib/mono/4.5/ -f netcoreapp2.0 -r test-results --logger "trx;LogFileName=Optimus.netcore.trx"
if [ "$versionDev" == "r" ]; then
	echo "Step 6. Pack nupkg"
	/usr/bin/dotnet pack source/Knyaz.Optimus/Knyaz.Optimus.csproj -c Release /p:Version=$version --no-build /p:FrameworkPathOverride=/usr/lib/mono/4.5/
fi

versionPref=${$BRANCH:1}
versionDev="${$BRANCH:0:1}"
if [ "$versionDev" == "d" ]; then
	version="$versionPref.$BUILD_NUMBER-dev";
    fileversion="versionPref.$BUILD_NUMBER.1";
else
	version="$versionPref.$BUILD_NUMBER";
    fileversion="versionPref.$BUILD_NUMBER.0";
fi
echo "Starting build of version: $version, fileVersion: $fileVersion"
echo "Step 1. Clean"
/bin/rm -f source/Knyaz.Optimus/test-results/*
/bin/rm -f source/Knyaz.Optimus.Tests/test-results/*
/bin/rm -f -r source/Knyaz.Optimus/bin/*
echo "Step 2. Build debug version"
/usr/bin/dotnet build source/Knyaz.Optimus.sln /p:FrameworkPathOverride=/usr/lib/mono/4.5/ /p:Version=$version /p:FileVersion=$fileversion -c Debug -v n
echo "Step 3. Run tests"
/usr/bin/dotnet test source/Knyaz.Optimus.sln --no-build /p:FrameworkPathOverride=/usr/lib/mono/4.5/ -f netcoreapp2.0 -r test-results --logger "trx;LogFileName=Optimus.netcore.trx"
echo "Step 4. Build release version"
/usr/bin/dotnet build source/Knyaz.Optimus.sln /p:FrameworkPathOverride=/usr/lib/mono/4.5/ /p:Version=$version /p:FileVersion=$fileversion -c Release -v n
echo "Step 5. Pack nupkg"
/usr/bin/dotnet pack source/Knyaz.Optimus/Knyaz.Optimus.csproj -c Release /p:Version=$version --no-build /p:FrameworkPathOverride=/usr/lib/mono/4.5/

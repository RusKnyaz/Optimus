version="2.0.$BUILD_NUMBER-dev";
fileversion="2.0.$BUILD_NUMBER.1";
echo "Step 1. Clean"
/bin/rm source/Knyaz.Optimus/test-results/*
/bin/rm source/Knyaz.Optimus.Tests/test-results/*
echo "Step 2. Build debug version"
/usr/bin/dotnet build source/Knyaz.Optimus.sln /p:FrameworkPathOverride=/usr/lib/mono/4.5/ /p:Version=$version /p:FileVersion=$fileversion -c Debug -v n
echo "Step 3. Run tests"
/usr/bin/dotnet test source/Knyaz.Optimus.sln --no-build /p:FrameworkPathOverride=/usr/lib/mono/4.5/ -f netcoreapp2.0 -r test-results --logger "trx;LogFileName=Optimus.netcore.trx"
echo "Step 4. Build release version"
/usr/bin/dotnet build source/Knyaz.Optimus.sln /p:FrameworkPathOverride=/usr/lib/mono/4.5/ /p:Version=$version /p:FileVersion=$fileversion -c Release -v n
echo "Step 5. Pack nupkg"
/usr/bin/dotnet pack source/Knyaz.Optimus/Knyaz.Optimus.csproj -c Release /p:Version=$version --no-build /p:FrameworkPathOverride=/usr/lib/mono/4.5/

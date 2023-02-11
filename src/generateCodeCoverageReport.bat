dotnet tool install dotnet-reportgenerator-globaltool --tool-path tools
RMDIR /Q/S .\TestResults\
dotnet test --settings coverlet.runsettings --results-directory:"./TestResults/"
.\tools\reportgenerator.exe -reports:.\TestResults\**\coverage.opencover.xml -targetdir:.\CodeCoverageReport\
start .\CodeCoverageReport\index.htm
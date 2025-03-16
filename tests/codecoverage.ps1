#Requires -Version 7.5

<#
Prerequisite tools:
- `dotnet-coverage`
- `dotnet-reportgenerator`

Tool install commands:
> dotnet tool install dotnet-coverage -g
> dotnet tool install dotnet-reportgenerator-globaltool -g
#>

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
$PSNativeCommandUseErrorActionPreference = $true

# Build solution with Release configuration
dotnet build ../ -c Release

# Define session id (It's used as temprary directory name that generated at %Temp%)
$sessionId = 'ZLinq'

# Start code coverage collector session with background server mode.
dotnet coverage collect --settings ./codecoverage.runsettings --session-id $sessionId --server-mode --background --nologo

try
{
  # Run unit tests with code coverage data collection.
  dotnet coverage connect $sessionId --nologo "dotnet test ../ -c Release --no-build"
}
finally
{
  # Shutdown codecoverage session
  dotnet coverage shutdown $sessionId --nologo
}

# Run dotnet ReportGenerator
$targetDir = 'TestResults/CoverageReports'
reportgenerator -reports:TestResults/coverage.cobertura.xml -targetdir:$targetDir -reporttypes:Html

# Show generated HTML report file path as clickable link. 
$reportPath = Resolve-Path './TestResults/CoverageReports/index.html'
Write-Host "CodeCoverage report file generated: `e]8;;${reportPath}`e\${reportPath}`e]8;;`e\"

# PostBuild.ps1
param (
    [string]$TargetDir
)

$buildFolder = $TargetDir

# Use the target directory where the output files are located
$buildFolder = $TargetDir
Write-Host "DEBUG: Build folder is set to '$buildFolder'"

# Define the path for the .exe file
$exeFile = Join-Path -Path $buildFolder -ChildPath 'Launcher.exe'
Write-Host "DEBUG: Target .exe file path is '$exeFile'"

# Check if the target exe file exists
if (Test-Path $exeFile)
{
    Write-Host "DEBUG: Found .exe file at '$exeFile'. Retrieving version information..."

    # Retrieve product version
    $version = (Get-Item $exeFile).VersionInfo.FileVersion
    Write-Host "DEBUG: Product version retrieved: $version"

    # Create the ZIP file path
    $zipFileName = Join-Path -Path $buildFolder -ChildPath ((Get-Item $exeFile).VersionInfo.ProductName + '_V' + $version + '.zip')
    Write-Host "DEBUG: Target ZIP file path is '$zipFileName'"

    # Compress the build folder contents into a ZIP file
    Compress-Archive -Path (Join-Path -Path $buildFolder -ChildPath '*') -DestinationPath $zipFileName -Force
    Write-Host "ZIP file created: $zipFileName"
}
else
{
    Write-Host "No .exe file found in the target directory: $exeFile"
}
# PostBuild.ps1
param (
    [string]$TargetDir
)

$buildFolder = $TargetDir
$binFolder = Join-Path $buildFolder "assemblies"

# Ensure 'bin' folder exists; if not, create it
if (!(Test-Path -Path $binFolder)) {
    New-Item -ItemType Directory -Path $binFolder | Out-Null
}

# Move all files in the top-level of the build folder, except ObtSDK.dll and Launcher.exe, to the 'bin' folder
Get-ChildItem -Path $buildFolder -File | ForEach-Object {
    if ($_.Name -ne "ObtSDK.dll" -and $_.Name -ne "Launcher.exe") {
        Move-Item -Path $_.FullName -Destination $binFolder -Force
    }
}

Write-Host "All files in the top-level of the build folder, except 'ObtSDK.dll' and 'Launcher.exe', have been moved to the 'bin' folder."
@echo off
setlocal enabledelayedexpansion

:: Get the directory where the .cmd file is located
set "ScriptDir=%~dp0"

:: Read the config.ini file to get the path (assuming the line is "path=C:\path\to\MuMuManager.exe")
for /f "tokens=2 delims==" %%A in ('findstr /b "path=" "%ScriptDir%config.ini"') do set "MuMuPath=%%A"

:: Check if MuMuPath was set correctly
if not defined MuMuPath (
    echo Error: path not found in config.ini.
    exit /b
)

:: Loop from 0 to 100 and run the command
for /L %%i in (0,1,100) do (
    "%MuMuPath%" adb -v %%i connect
)

endlocal
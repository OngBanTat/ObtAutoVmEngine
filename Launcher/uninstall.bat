@echo off

:: Shut down the adb server
adb\adb.exe kill-server

:: Go back to the parent directory
cd ..

:: Remove the directory where the script was located
rmdir /s /q "%~dp0"

:: Exit
exit
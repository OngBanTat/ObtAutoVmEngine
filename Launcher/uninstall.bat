@echo off

:: Shut down the adb server
adb\adb.exe kill-server

:: Remove all files and subdirectories in the current directory
:: including the script itself
rmdir /s /q "%cd%"

:: Exit
exit
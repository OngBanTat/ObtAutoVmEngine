@echo off

echo Shutting down ADB server...
adb\adb.exe kill-server

echo Removing current folder and its contents...
cd ..
rmdir /s /q "%~dp0"

echo Uninstallation complete.
pause

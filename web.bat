@echo off
echo ========================================
echo    FlexHub Web App Launcher
echo ========================================
echo.
echo Starting FlexHub Web Application...
echo.
echo Once started, open your browser to:
echo    http://localhost:5280
echo.
echo Press Ctrl+C to stop the server
echo ========================================
echo.

cd /d "%~dp0FlexHub.Web"
dotnet run

pause

@echo off
REM Modern .NET 8 WPF Application Launcher
REM This batch file launches the ModernDesktopApp

echo.
echo ========================================
echo  Modern .NET 8 WPF Application
echo ========================================
echo.
echo Starting application...
echo.

REM Run the application
dotnet run --project ModernDesktopApp.csproj

REM Keep window open if there's an error
if errorlevel 1 (
    echo.
    echo ========================================
    echo  Application exited with an error
    echo ========================================
    echo.
    pause
)
@echo off
REM FlexHub - Modern .NET 8 WPF Application Launcher

echo.
echo ========================================
echo  FlexHub - Modern .NET 8 WPF App
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
@echo off
REM FlexHub - Modern .NET 8 WPF Application Launcher

echo.
echo ========================================
echo  FlexHub - Modern .NET 8 WPF App
echo ========================================
echo.

cd /d "%~dp0"

echo Cleaning build cache...
if exist bin rd /s /q bin
if exist obj rd /s /q obj
dotnet clean ModernDesktopApp.csproj --nologo -v q 2>nul

echo Building application...
dotnet build ModernDesktopApp.csproj --nologo

if errorlevel 1 (
    echo.
    echo ========================================
    echo  Build failed! See errors above.
    echo ========================================
    echo.
    pause
    exit /b 1
)

echo.
echo Starting application...
echo.

REM Run the application
dotnet run --project ModernDesktopApp.csproj --no-build

REM Keep window open if there's an error
if errorlevel 1 (
    echo.
    echo ========================================
    echo  Application exited with an error
    echo ========================================
    echo.
    pause
)
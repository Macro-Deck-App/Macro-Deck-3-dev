@echo off
REM Development script for Windows that watches md-ui library and serves configurator

echo Starting development environment...
echo.

REM Build md-ui once first
echo Building md-ui library...
call npm run build-md-ui

if %ERRORLEVEL% NEQ 0 (
    echo Initial md-ui build failed!
    exit /b 1
)

echo Initial md-ui build complete
echo.

REM Start watching md-ui in the background
echo Starting md-ui watch mode...
start "MD-UI Watch" /B npm run watch-md-ui

REM Give it a moment to start
timeout /t 2 /nobreak >nul

REM Start the configurator dev server
echo Starting configurator dev server on port 8192...
npm run ng serve configurator -- --port 8192

REM When configurator stops, the watch will also stop
echo.
echo Development environment stopped

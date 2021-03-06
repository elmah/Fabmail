@echo off
setlocal
pushd "%~dp0"
call :build Debug && call :build Release
popd
goto :EOF

:build
if "%PROCESSOR_ARCHITECTURE%"=="x86" set MSBUILD=%ProgramFiles%
if defined ProgramFiles(x86) set MSBUILD=%ProgramFiles(x86)%
set MSBUILD=%MSBUILD%\MSBuild\14.0\bin\msbuild
if not exist "%MSBUILD%" (
    echo>&2 Microsoft Build Tools 2015 does not appear to be installed on this
    echo>&2 machine, which is required to build the solution. You can install
    echo>&2 it from the URL below and then try building again:
    echo>&2 https://www.microsoft.com/en-us/download/details.aspx?id=48159
    exit /b 1
)
"%MSBUILD%" /p:Configuration=%1 /v:m %2 %3 %4 %5 %6 %7 %8 %9
goto :EOF

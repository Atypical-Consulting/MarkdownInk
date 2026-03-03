@echo off
echo === MarkdownInk setup ===
echo.

cd /d "%~dp0"

echo [1/4] Restoring packages...
dotnet restore MarkdownInk.slnx
if %ERRORLEVEL% neq 0 (
    echo ERROR: restore failed
    exit /b 1
)
echo.

echo [2/4] Building...
dotnet build MarkdownInk.slnx --configuration Release --no-restore
if %ERRORLEVEL% neq 0 (
    echo ERROR: build failed
    exit /b 1
)
echo.

echo [3/4] Packing as global tool...
if exist nupkg rmdir /s /q nupkg
dotnet pack src\MarkdownInk\MarkdownInk.csproj --configuration Release --no-build --output nupkg
if %ERRORLEVEL% neq 0 (
    echo ERROR: pack failed
    exit /b 1
)
echo.

echo [4/4] Installing globally...
dotnet tool uninstall --global mdink 2>nul
dotnet tool install --global --add-source ./nupkg MarkdownInk
if %ERRORLEVEL% neq 0 (
    echo ERROR: install failed
    exit /b 1
)
echo.

echo === Setup complete! ===
echo You can now run: mdink sample.md
echo.

mdink "%~dp0sample.md"

@echo off
setlocal
if "%GTA_V_DIR%"=="" (
  echo Defina a variavel GTA_V_DIR apontando para a pasta do GTA V.
  echo Exemplo: set GTA_V_DIR=C:\Program Files\Rockstar Games\Grand Theft Auto V
  exit /b 1
)

dotnet build RealLifeAccessMod.csproj -c Release
if errorlevel 1 exit /b 1

copy /Y bin\RealLifeAccessMod.dll "%GTA_V_DIR%\scripts\RealLifeAccessMod.dll"
echo.
echo DLL copiada para %GTA_V_DIR%\scripts\
endlocal

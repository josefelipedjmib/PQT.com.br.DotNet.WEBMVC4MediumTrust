@echo off
setlocal

REM Nome do módulo que você quer criar
set /p MODULO=Digite o nome do módulo:

REM Caminho completo do script PowerShell
set "SCRIPT_PATH=%~dp0Criar-Modulo.ps1"

REM Executa o PowerShell com política de execução ignorada
powershell.exe -ExecutionPolicy Bypass -File "%SCRIPT_PATH%" -NomeModulo "%MODULO%"

endlocal
pause

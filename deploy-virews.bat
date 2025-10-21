@echo off
setlocal

REM Caminho base da publicação - ajuste conforme necessário
set "PUBLISH_PATH=.\Web.MVC\bin\app.publish"

REM Caminhos de origem e destino
set "SOURCE=%PUBLISH_PATH%\bin\Areas"
set "DEST=%PUBLISH_PATH%\Areas"

echo 🔧 Verificando se existe: %SOURCE%
if exist "%SOURCE%" (
    echo ✅ Movendo arquivos de %SOURCE% para %DEST%

    REM Cria a pasta de destino se não existir
    if not exist "%DEST%" (
        mkdir "%DEST%"
    )

    REM Move os arquivos e subpastas
    robocopy "%SOURCE%" "%DEST%" /MOVE /E

    echo ✅ Views movidas com sucesso.
) else (
    echo ⚠️ Pasta bin\Areas não encontrada em %SOURCE%
)

endlocal
pause
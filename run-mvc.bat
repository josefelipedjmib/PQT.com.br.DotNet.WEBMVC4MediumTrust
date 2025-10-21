@echo off
setlocal

echo 🔧 Compilando projeto Web.MVC...
msbuild.exe WEBMVC4MediumTrust.sln /p:Configuration=Debug
if %ERRORLEVEL% NEQ 0 (
    echo ❌ Erro na compilação. Abortando execução.
    exit /b %ERRORLEVEL%
)

echo 🚀 Iniciando IIS Express na porta 8080...
iisexpress.exe /path:"%~dp0Web.MVC" /port:8080

endlocal
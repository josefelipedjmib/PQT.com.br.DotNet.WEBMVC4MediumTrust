param (
    [string]$NomeModulo,
    [string]$CaminhoTemplateZip = ".\Modulo.Template.zip",
    [string]$Destino = ".\"
)

# Caminho final do novo módulo
$ModuloPath = Join-Path $Destino "Modulo.$NomeModulo"

# 1. Extrair o ZIP
Expand-Archive -Path $CaminhoTemplateZip -DestinationPath $ModuloPath -Force

# 2. Renomear pastas internas
Rename-Item -Path "$ModuloPath\Areas\__AREA__" -NewName $NomeModulo
Rename-Item -Path "$ModuloPath\Areas\$NomeModulo\Views\__AREA__" -NewName $NomeModulo
Rename-Item -Path "$ModuloPath\Areas\$NomeModulo\Controllers\__AREA__Controller.cs" -NewName "$NomeModulo`Controller.cs"
Rename-Item -Path "$ModuloPath\Areas\$NomeModulo\__AREA__AreaRegistration.cs" -NewName "$NomeModulo`AreaRegistration.cs"

# 3. Substituir conteúdo dos arquivos
Get-ChildItem -Path $ModuloPath -Recurse -Include *.cs, *.cshtml, *.config, *.csproj | ForEach-Object {
    (Get-Content $_.FullName) -replace '__AREA__', $NomeModulo | Set-Content $_.FullName
}

# 4. Renomear o projeto
Rename-Item -Path "$ModuloPath\Modulo.__AREA__.csproj" -NewName "Modulo.$NomeModulo.csproj"
Rename-Item -Path "$ModuloPath\Modulo.__AREA__.csproj.user" -NewName "Modulo.$NomeModulo.csproj.user"

Write-Host "✅ Módulo 'Modulo.$NomeModulo' criado com sucesso em $ModuloPath"

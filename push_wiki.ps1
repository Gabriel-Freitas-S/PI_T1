# Script para sincronizar todos os arquivos da pasta wiki/ diretamente com o GitHub Wiki
# Uso: .\push_wiki.ps1

$ErrorActionPreference = "Stop"

$repoWikiUrl = "https://github.com/Gabriel-Freitas-S/PI_T1.wiki.git"
$tempDir = Join-Path $env:TEMP "pi_t1_wiki_push"

Write-Host "==> Preparando sincronização com GitHub Wiki..." -ForegroundColor Cyan

if (Test-Path $tempDir) {
    Remove-Item $tempDir -Recurse -Force
}

New-Item -ItemType Directory -Path $tempDir | Out-Null
Copy-Item -Path "wiki\*" -Destination $tempDir -Recurse

Push-Location $tempDir
try {
    git init -b master | Out-Null
    git config user.name "Gabriel-Freitas-S"
    git config user.email "gabriel@example.com"
    git add .
    git commit -m "docs: publicacao completa da GitHub Wiki (10 capitulos)" | Out-Null
    git remote add origin $repoWikiUrl

    Write-Host "==> Enviando páginas para o GitHub Wiki..." -ForegroundColor Cyan
    $env:GITHUB_TOKEN = ""
    git push -u origin master --force
    Write-Host "==> Wiki publicada com sucesso em: https://github.com/Gabriel-Freitas-S/PI_T1/wiki" -ForegroundColor Green
}
finally {
    Pop-Location
}

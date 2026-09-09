# Script para desenvolvimento e visualização da documentação Astro Starlight
# Uso: 
#   .\run_docs.ps1          (Inicia servidor de desenvolvimento com hot-reload)
#   .\run_docs.ps1 -Build   (Compila versão de produção em docs/dist)
#   .\run_docs.ps1 -Preview (Inicia preview da versão compilada)

param(
    [switch]$Build,
    [switch]$Preview
)

$ErrorActionPreference = "Stop"
$docsDir = Join-Path $PSScriptRoot "docs"

if (-not (Test-Path $docsDir)) {
    Write-Error "Diretório 'docs' não encontrado em $docsDir"
    exit 1
}

Push-Location $docsDir
try {
    if ($Build) {
        Write-Host "==> Compilando documentação Starlight em modo de produção..." -ForegroundColor Cyan
        npm run build
        Write-Host "==> Build concluído com sucesso em docs/dist!" -ForegroundColor Green
    }
    elseif ($Preview) {
        Write-Host "==> Iniciando servidor de pré-visualização local da pasta dist..." -ForegroundColor Cyan
        npm run preview
    }
    else {
        Write-Host "==> Iniciando servidor de desenvolvimento Astro Starlight com hot-reload..." -ForegroundColor Cyan
        npm run dev
    }
}
finally {
    Pop-Location
}

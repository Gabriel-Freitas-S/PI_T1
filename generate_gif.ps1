# ==============================================================================
# SCRIPT DE GERACAO AUTOMATICA DO GIF DA LOCOMOTIVA (WPF + FFMPEG)
# ==============================================================================
# Uso: .\generate_gif.ps1
#
# Parametros opcionais:
#   .\generate_gif.ps1 -Duration 15.0 -Fps 25 -Width 880 -OutputFile "locomotiva.gif"
# ==============================================================================

param(
    [double]$Duration = 15.0,
    [int]$Fps = 20,
    [int]$Width = 800,
    [string]$OutputFile = "locomotiva.gif"
)

$ErrorActionPreference = "Stop"

Write-Host "[1/4] Localizando FFmpeg no sistema..." -ForegroundColor Cyan

# 1. Busca pelo executavel do FFmpeg
$ffmpegCmd = "ffmpeg"
$ffmpegExe = $null

if (Get-Command $ffmpegCmd -ErrorAction SilentlyContinue) {
    $ffmpegExe = (Get-Command $ffmpegCmd).Source
} else {
    $candidatos = @(
        "$env:USERPROFILE\scoop\shims\ffmpeg.exe",
        "$env:USERPROFILE\scoop\apps\ffmpeg\current\bin\ffmpeg.exe",
        "C:\ProgramData\chocolatey\bin\ffmpeg.exe",
        "C:\ffmpeg\bin\ffmpeg.exe",
        "C:\Program Files\ffmpeg\bin\ffmpeg.exe"
    )
    foreach ($c in $candidatos) {
        if (Test-Path $c) {
            $ffmpegExe = $c
            break
        }
    }
}

if (-not $ffmpegExe) {
    Write-Error "FFmpeg nao foi encontrado automaticamente. Certifique-se de reiniciar o terminal ou adicionar o FFmpeg ao PATH."
    exit 1
}

Write-Host "   -> FFmpeg encontrado: $ffmpegExe" -ForegroundColor Green

# 2. Renderizacao deterministica de quadros offline via WPF (.NET 10)
$tempFramesDir = Join-Path $env:TEMP "pi_t1_gif_frames"
if (Test-Path $tempFramesDir) {
    Remove-Item $tempFramesDir -Recurse -Force
}
New-Item -ItemType Directory -Path $tempFramesDir | Out-Null

$totalQuadros = [math]::Round($Duration * $Fps)
Write-Host "[2/4] Capturando $totalQuadros quadros ($Duration s a $Fps FPS)..." -ForegroundColor Cyan

Get-Process -Name PI_T1 -ErrorAction SilentlyContinue | Stop-Process -Force
& dotnet run -- --record-frames $Duration $Fps $tempFramesDir

$quadrosGerados = (Get-ChildItem -Path $tempFramesDir -Filter "frame_*.png").Count
if ($quadrosGerados -eq 0) {
    Write-Error "Nenhum quadro foi gerado pelo processo do WPF."
    exit 1
}
Write-Host "   -> $quadrosGerados quadros capturados com sucesso em $tempFramesDir" -ForegroundColor Green

# 3. Codificacao de alta qualidade com FFmpeg (Palettegen + Paletteuse)
Write-Host "[3/4] Codificando GIF otimizado ($Width px com paleta adaptativa 128 cores)..." -ForegroundColor Cyan

$filterComplex = "fps=" + $Fps + ",scale=" + $Width + ":-1:flags=lanczos,split[s0][s1];[s0]palettegen=max_colors=128:stats_mode=diff[p];[s1][p]paletteuse=dither=bayer:bayer_scale=3"
$inputPattern = Join-Path $tempFramesDir "frame_%04d.png"

& "$ffmpegExe" -y -framerate $Fps -i "$inputPattern" -vf "$filterComplex" -loop 0 "$OutputFile"

if (-not (Test-Path $OutputFile)) {
    Write-Error "Falha ao gerar o arquivo GIF com o FFmpeg."
    exit 1
}

# 4. Sincronizacao com a pasta wiki/ e limpeza
Write-Host "[4/4] Sincronizando com a pasta wiki/ e finalizando..." -ForegroundColor Cyan
if (Test-Path "wiki") {
    Copy-Item -Path $OutputFile -Destination "wiki\$OutputFile" -Force
    Write-Host "   -> Copiado para wiki\$OutputFile" -ForegroundColor Green
}

Remove-Item $tempFramesDir -Recurse -Force

$tamanhoMb = [math]::Round(((Get-Item $OutputFile).Length / 1MB), 2)
Write-Host ""
Write-Host "SUCESSO! GIF gerado com perfeicao:" -ForegroundColor Green
Write-Host "   Arquivo : $OutputFile" -ForegroundColor White
Write-Host "   Tamanho : $tamanhoMb MB" -ForegroundColor White
Write-Host "   Duracao : $Duration s (Loop Continuo)" -ForegroundColor White
Write-Host "   Taxa    : $Fps FPS, Largura: $Width px" -ForegroundColor White
Write-Host ""

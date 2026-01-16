@echo off
cd /d "%~dp0"

echo ================================
echo Iniciando MediaMTX...
echo ================================

REM Inicia o MediaMTX na pasta atual
start "MediaMTX" "%~dp0mediamtx.exe"

REM Aguarda alguns segundos
timeout /t 3 /nobreak > nul

echo ================================
echo Iniciando RTSP Python...
echo ================================

REM Inicia o script Python na mesma pasta
start "RTSP Python" python "%~dp0rtsp.py"

echo ================================
echo Tudo iniciado com sucesso
echo ================================

pause

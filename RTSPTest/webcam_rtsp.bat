@echo off
title Webcam RTSP - Low Latency

echo Iniciando MediaMTX...
start "" mediamtx.exe
timeout /t 2 > nul

echo Iniciando stream...

set CAMERA_NAME=XWF-1080P
set RTSP_URL=rtsp://localhost:8554/camera1

ffmpeg ^
-f dshow -rtbufsize 50M -i video="%CAMERA_NAME%" ^
-vf "scale=1280:720,fps=15" ^
-c:v libx264 ^
-preset ultrafast ^
-tune zerolatency ^
-pix_fmt yuv420p ^
-g 30 ^
-keyint_min 30 ^
-sc_threshold 0 ^
-b:v 2000k ^
-maxrate 2000k ^
-bufsize 1000k ^
-fflags nobuffer ^
-flags low_delay ^
-f rtsp ^
-rtsp_transport udp ^
%RTSP_URL%

pause

@echo off
chcp 1254

openfiles >nul 2>&1 || (
    echo Bu dosya yonetici olarak calistirilmalidir.
    pause
    exit /b 1
)

set SERVICE_NAME=HamsteraiWebApi

sc query %SERVICE_NAME% >nul 2>&1
if %ERRORLEVEL% == 0 (
    echo %SERVICE_NAME% yeniden baslatiliyor...
    sc stop %SERVICE_NAME%
    sc start %SERVICE_NAME%
    echo %SERVICE_NAME% basariyla yeniden baslatildi.
) else (
    echo %SERVICE_NAME% bulunamadi. Lutfen servisi yukleyin.
)
pause
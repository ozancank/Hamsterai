@echo off
chcp 1254

openfiles >nul 2>&1 || (
    echo Bu dosya yonetici olarak calistirilmalidir.
    pause
    exit /b 1
)

setlocal enabledelayedexpansion

set servicePath=#ServicePath#
set file="%servicePath%appsettings.json"

if not exist %file% (
    echo %file% bulunamadi.
    timeout /t 5 >nul
    exit
)

call :UpdateConnectionString "HamsteraiConnectionString"
call :UpdateConnectionString "MikroConnectionString"

if %ERRORLEVEL% NEQ 0 (
    echo SQL Server baglantisi basarisiz. Program kapaniyor...
    pause
    timeout /t 5 >nul
    exit
)

set SERVICE_NAME=HamsteraiWebApi
set SERVICE_DISPLAY_NAME=HamsteraiWebApi
set SERVICE_PATH="%servicePath%\HamsteraiWebApi.exe"

sc query %SERVICE_NAME% >nul 2>&1
if %ERRORLEVEL% == 0 (
    echo %SERVICE_NAME% kaldiriliyor...
    sc stop %SERVICE_NAME%
    sc delete %SERVICE_NAME%
)

echo %SERVICE_NAME% servis kuruluyor...
sc create %SERVICE_NAME% binPath= %SERVICE_PATH% DisplayName= %SERVICE_DISPLAY_NAME%
sc config %SERVICE_NAME% start= delayed-auto
sc start %SERVICE_NAME%

del /s /q /f *.tmp
timeout /t 5 >nul
exit


:UpdateConnectionString
echo %~1
echo ----------------------
set "CONN_NAME=%~1"
set "EXISTING_CONN_STRING="
for /f "tokens=*" %%i in ('findstr /r /c:"\"%CONN_NAME%\"" %file%') do set "EXISTING_CONN_STRING=%%i"

for /f "tokens=2 delims==;" %%a in ('echo %EXISTING_CONN_STRING%') do set "DATA_SOURCE=%%a"
for /f "tokens=4 delims==;" %%a in ('echo %EXISTING_CONN_STRING%') do set "INITIAL_CATALOG=%%a"
for /f "tokens=6 delims==;" %%a in ('echo %EXISTING_CONN_STRING%') do set "USER_ID=%%a"
for /f "tokens=8 delims==;" %%a in ('echo %EXISTING_CONN_STRING%') do set "PASSWORD=%%a"

set /p DATA_SOURCE=Data Source [%DATA_SOURCE%]:
if "%DATA_SOURCE%"=="" set DATA_SOURCE=%DATA_SOURCE%
set DATA_SOURCE=%DATA_SOURCE:\\=\%

set /p USER_ID=User Id [%USER_ID%]:
if "%USER_ID%"=="" set USER_ID=%USER_ID%

set /p PASSWORD=Password [%PASSWORD%]:
if "%PASSWORD%"=="" set PASSWORD=%PASSWORD%

echo !CONN_NAME!
if !CONN_NAME! == HamsteraiConnectionString (
    sqlcmd -S %DATA_SOURCE% -U %USER_ID% -P %PASSWORD% -Q "SELECT 1;" -b >nul 2>&1
    
    for /f "usebackq tokens=*" %%i in (%file%) do (
        set "line=%%i"
        echo !line! | findstr /r /c:"\"%CONN_NAME%\"" >nul && (
            set "line="%CONN_NAME%": "Data Source=%DATA_SOURCE:\=\\%;Initial Catalog=Hamsterai;User Id=%USER_ID%;password=%PASSWORD%;Trusted_Connection=False;TrustServerCertificate=True;","      
        )       
        echo !line! >> appsettings.tmp
    )
    move /y appsettings.tmp %file%
    exit /b %ERRORLEVEL%
) else (
    echo (
        set DBSRC=%DATA_SOURCE:\=\\%
        sqlcmd -S !DBSRC! -U %USER_ID% -P %PASSWORD% -Q "SELECT name FROM sys.databases where database_id > 4 order by name;" -h-1 -W > databases.txt
        
        set COUNT=0
        for /f "tokens=*" %%i in (databases.txt) do (
            set /a COUNT+=1
            echo !COUNT! - %%i
            set DB_!COUNT!=%%i
        )
    )
    
    del /f /q databases.txt    
    set /p DB_CHOICE=%CONN_NAME% icin hangi veritabanini kullanmak istersiniz (numara girin): 
    
    for /f "tokens=*" %%i in ("!DB_%DB_CHOICE%!") do set SELECTED_DB=%%i
    
    for /f "usebackq tokens=*" %%i in (%file%) do (
        set "line=%%i"
        echo !line! | findstr /r /c:"\"%CONN_NAME%\"" >nul && (
            set "line="%CONN_NAME%": "Data Source=%DATA_SOURCE:\=\\%;Initial Catalog=%SELECTED_DB%;User Id=%USER_ID%;password=%PASSWORD%;Trusted_Connection=False;MultipleActiveResultSets=true;TrustServerCertificate=True;""
        )
        echo !line! >> appsettings.tmp
    )
 
    move /y appsettings.tmp %file%
    exit /b %ERRORLEVEL%
)
goto :EOF
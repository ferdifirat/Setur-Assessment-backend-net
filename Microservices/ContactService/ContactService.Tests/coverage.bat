@echo off
setlocal

echo.
echo [1/4] Testler çalıştırılıyor ve coverage toplanıyor...
dotnet test --collect:"XPlat Code Coverage"
if %errorlevel% neq 0 (
    echo Test çalıştırılırken hata oluştu.
    pause
    exit /b %errorlevel%
)

echo.
echo [2/4] ReportGenerator aracı yükleniyor (eğer yüklü değilse)...
dotnet tool install --global dotnet-reportgenerator-globaltool --ignore-failed-sources

echo.
echo [3/4] HTML raporu oluşturuluyor...
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:"coverage-report" -reporttypes:Html

if exist coverage-report\index.html (
    echo.
    echo [4/4] Rapor tarayıcıda açılıyor...
    start coverage-report\index.html
) else (
    echo Rapor dosyası bulunamadı.
)

echo.
echo İşlem tamamlandı.
pause
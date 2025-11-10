# ?? COMPREHENSIVE UNIT TEST - BUILD & RUN SCRIPT

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  BUILD & RUN ALL UNIT TESTS" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Set location to test project
$testProject = "RestaurantManagement.Tests\RestaurantManagement.Tests.csproj"

if (-Not (Test-Path $testProject)) {
    Write-Host "? Test project not found at: $testProject" -ForegroundColor Red
    exit 1
}

Write-Host "?? Restoring packages..." -ForegroundColor Yellow
dotnet restore $testProject

if ($LASTEXITCODE -ne 0) {
    Write-Host "? Package restore failed!" -ForegroundColor Red
    exit $LASTEXITCODE
}

Write-Host ""
Write-Host "?? Building test project..." -ForegroundColor Yellow
dotnet build $testProject --no-restore

if ($LASTEXITCODE -ne 0) {
    Write-Host "? Build failed!" -ForegroundColor Red
    exit $LASTEXITCODE
}

Write-Host ""
Write-Host "? Build successful!" -ForegroundColor Green
Write-Host ""

Write-Host "?? Running all unit tests..." -ForegroundColor Yellow
Write-Host ""

dotnet test $testProject --no-build --verbosity normal --logger "console;verbosity=detailed"

if ($LASTEXITCODE -ne 0) {
    Write-Host ""
    Write-Host "? Some tests FAILED!" -ForegroundColor Red
    Write-Host ""
    exit $LASTEXITCODE
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "  ? ALL TESTS PASSED!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""

# Count test files
$testFiles = Get-ChildItem -Path "RestaurantManagement.Tests\Unit" -Filter "*Tests.cs" -Recurse
$testCount = $testFiles.Count

Write-Host "?? Test Summary:" -ForegroundColor Cyan
Write-Host "  - Test Files: $testCount" -ForegroundColor White
Write-Host "  - Layers Tested: Services, Controllers" -ForegroundColor White
Write-Host ""

Write-Host "?? Testing Complete!" -ForegroundColor Green

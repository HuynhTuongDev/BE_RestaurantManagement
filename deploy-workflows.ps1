#!/usr/bin/env pwsh
# Deploy GitHub Actions Workflows

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  GitHub Actions CI/CD Setup" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check if workflows directory exists
$workflowDir = ".github/workflows"
if (-Not (Test-Path $workflowDir)) {
    Write-Host "? Workflows directory not found!" -ForegroundColor Red
    exit 1
}

# List all workflow files
$workflows = Get-ChildItem -Path $workflowDir -Filter "*.yml"
Write-Host "?? Found $($workflows.Count) workflow files:" -ForegroundColor Yellow
$workflows | ForEach-Object { Write-Host "  - $($_.Name)" -ForegroundColor White }

Write-Host ""
Write-Host "?? Preparing to push to GitHub..." -ForegroundColor Yellow
Write-Host ""

# Check git status
$gitStatus = git status --porcelain
if ($gitStatus) {
    Write-Host "?? Changes detected:" -ForegroundColor Cyan
    git status --short
    
    Write-Host ""
    Write-Host "? Staging changes..." -ForegroundColor Green
    git add .github/workflows/
    git add CI_CD_SETUP_GUIDE.md
    git add deploy-workflows.ps1
    
    Write-Host ""
    Write-Host "?? Creating commit..." -ForegroundColor Green
    git commit -m "chore: add GitHub Actions CI/CD pipelines for automated testing"
    
    Write-Host ""
    Write-Host "?? Pushing to GitHub..." -ForegroundColor Green
    git push origin HEAD
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host ""
        Write-Host "========================================" -ForegroundColor Green
        Write-Host "  ? WORKFLOWS DEPLOYED SUCCESSFULLY!" -ForegroundColor Green
        Write-Host "========================================" -ForegroundColor Green
        Write-Host ""
        Write-Host "?? View your pipelines:" -ForegroundColor Cyan
        Write-Host "  ?? https://github.com/HuynhTuongDev/BE_RestaurantManagement/actions" -ForegroundColor White
        Write-Host ""
        Write-Host "? Next steps:" -ForegroundColor Cyan
        Write-Host "  1. Create a PR or push to develop branch" -ForegroundColor White
        Write-Host "  2. Watch tests run automatically" -ForegroundColor White
        Write-Host "  3. View coverage reports in artifacts" -ForegroundColor White
        Write-Host ""
    } else {
        Write-Host ""
        Write-Host "? Git push failed!" -ForegroundColor Red
        exit 1
    }
} else {
    Write-Host "??  No changes to commit" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "? Workflows already deployed!" -ForegroundColor Green
}

Write-Host "?? Documentation:" -ForegroundColor Cyan
Write-Host "  ?? See CI_CD_SETUP_GUIDE.md for details" -ForegroundColor White

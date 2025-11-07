# Cleanup Script for Testing Folder
# This script removes the remaining Testing folder that couldn't be deleted due to locked files

Write-Host "CoinPay Testing Folder Cleanup Script" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

# Check if Testing folder exists
if (Test-Path "Testing") {
    Write-Host "Found Testing folder. Attempting cleanup..." -ForegroundColor Yellow

    # Kill any remaining playwright processes
    Write-Host "Stopping playwright processes..." -ForegroundColor Yellow
    Get-Process | Where-Object {$_.Name -like "*playwright*" -or $_.Name -like "*node*" -and $_.CommandLine -like "*playwright*"} | Stop-Process -Force -ErrorAction SilentlyContinue

    # Wait a moment
    Start-Sleep -Seconds 2

    # Try to remove the Testing folder
    try {
        Remove-Item -Path "Testing" -Recurse -Force -ErrorAction Stop
        Write-Host "✓ Testing folder removed successfully!" -ForegroundColor Green
    }
    catch {
        Write-Host "✗ Could not remove Testing folder automatically." -ForegroundColor Red
        Write-Host "  Error: $($_.Exception.Message)" -ForegroundColor Red
        Write-Host ""
        Write-Host "Manual cleanup steps:" -ForegroundColor Yellow
        Write-Host "1. Close any applications that might be using files in Testing folder" -ForegroundColor Gray
        Write-Host "2. Restart your computer if needed" -ForegroundColor Gray
        Write-Host "3. Delete the Testing folder manually" -ForegroundColor Gray
        Write-Host "4. Or run this script again" -ForegroundColor Gray
    }
}
else {
    Write-Host "✓ Testing folder not found - cleanup already complete!" -ForegroundColor Green
}

Write-Host ""
Write-Host "All testing infrastructure has been moved to CoinPay.Tests/" -ForegroundColor Cyan
Write-Host "See CoinPay.Tests/README.md for the complete documentation" -ForegroundColor Cyan

# Monster Hosting üçün Publish Script
# PowerShell-də run et: .\publish.ps1

Write-Host "Building and publishing for Monster hosting..." -ForegroundColor Green

# Clean previous publish
if (Test-Path "./publish") {
    Remove-Item -Recurse -Force "./publish"
    Write-Host "Cleaned previous publish folder" -ForegroundColor Yellow
}

# Publish
dotnet publish -c Release -o ./publish

if ($LASTEXITCODE -eq 0) {
    Write-Host "`nPublish completed successfully!" -ForegroundColor Green
    Write-Host "Publish folder: $PWD\publish" -ForegroundColor Cyan
    Write-Host "`nNext steps:" -ForegroundColor Yellow
    Write-Host "1. Update appsettings.Production.json with Monster SQL connection string" -ForegroundColor White
    Write-Host "2. Upload all files from 'publish' folder to Monster hosting via FTP" -ForegroundColor White
    Write-Host "3. Run database migrations on Monster server" -ForegroundColor White
    Write-Host "4. Test: https://yourdomain.com/swagger" -ForegroundColor White
} else {
    Write-Host "`nPublish failed! Check errors above." -ForegroundColor Red
    exit 1
}


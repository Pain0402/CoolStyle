Write-Host "Updating Fashion Ecommerce (Keeping Data)..." -ForegroundColor Cyan

# 1. Build and Start (Smart Update)
# Docker Compose will check for changes, rebuild the image, and recreate only the necessary containers.
# Data volumes (SQL, Redis) are PRESERVED.
Write-Host "Building and Updating Containers..." -ForegroundColor Yellow
docker compose -f docker-compose.prod.yml up -d --build

# 2. Cleanup dangling images (Optional, to save disk space)
Write-Host "Cleaning up old images..." -ForegroundColor Gray
docker image prune -f

# 3. Validation Message
Write-Host "---------------------------------------------------" -ForegroundColor Green
Write-Host "Update Successful!" -ForegroundColor Green
Write-Host "---------------------------------------------------"

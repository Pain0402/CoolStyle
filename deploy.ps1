Write-Host "Starting Deployment for Fashion Ecommerce (Production)..." -ForegroundColor Cyan

# 1. Stop existing containers and remove volumes (Clean Slate)
Write-Host "Stopping existing containers..." -ForegroundColor Yellow
docker compose -f docker-compose.prod.yml down -v

# 2. Build and Start
Write-Host "Building and Starting Containers..." -ForegroundColor Yellow
docker compose -f docker-compose.prod.yml up -d --build

# 3. Validation Message
Write-Host "---------------------------------------------------" -ForegroundColor Green
Write-Host "Deployment Successful!" -ForegroundColor Green
Write-Host "---------------------------------------------------"
Write-Host "Storefront: http://localhost" 
Write-Host "Backend API: http://localhost:8080/swagger" 
Write-Host "Log Server:  http://localhost:5341" 
Write-Host "---------------------------------------------------"

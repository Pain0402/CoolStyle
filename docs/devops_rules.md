# DevOps & Deployment Rules

## 1. Containerization Strategy
- **Docker**: Every service (API, UI, Database, Cache) must have a `Dockerfile`.
- **Docker Compose**: Use `docker-compose.yml` for local development orchestration.
- **Optimization**: Multi-stage builds to keep production images small (e.g., use Alpine Linux base).

## 2. CI/CD Pipeline (GitHub Actions or Azure DevOps)
### 2.1. Continuous Integration (CI)
- Trigger: Pull Request or Push to `main`.
- Steps:
  1. Build Backend & Frontend.
  2. Run Unit Tests (Block if fail).
  3. Lint Code (ESLint, StyleCop).

### 2.2. Continuous Deployment (CD)
- Environment: Staging then Production.
- Strategy: Blue/Green or Rolling Update (zero downtime).
- Process:
  1. Build Docker Images.
  2. Push to Container Registry (Docker Hub / ACR).
  3. Deploy to Server.

## 3. Infrastructure Management
- **Reverse Proxy**: Nginx.
  - Handle SSL termination (LetsEncrypt).
  - Compression (Gzip/Brotli).
  - Static file caching.
- **Monitoring**:
  - **Logs**: Centralized logging (Seq / ELK).
  - **Metrics**: Prometheus + Grafana (Server stats, API latency).
  - **Uptime**: UptimeRobot or health check endpoints (`/health`).

## 4. Security Ops
- **Secrets**: NEVER commit secrets to Git. Use `.env` files locally and Secret Manager in CI/CD.
- **Backups**: Automated nightly backups of SQL Server to external storage (S3).

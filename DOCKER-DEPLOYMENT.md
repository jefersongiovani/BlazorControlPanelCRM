# Docker Deployment Guide for BlazorControlPanel

This guide explains how to deploy the BlazorControlPanel application using Docker with nginx as a static file server.

## 🏗️ Architecture

The application uses a multi-stage Docker build:
1. **Build Stage**: Uses .NET 9 SDK to build and publish the Blazor WebAssembly application
2. **Production Stage**: Uses nginx:alpine to serve the static files with optimized configuration

## 📋 Prerequisites

- Docker installed on your system
- Docker Compose (optional, for easier management)

## 🚀 Quick Start

### Option 1: Using the Build Script (Recommended)

```bash
# Make the script executable (if not already)
chmod +x build-docker.sh

# Run the build script
./build-docker.sh
```

### Option 2: Manual Docker Commands

```bash
# Build the Docker image
docker build -t blazor-control-panel:latest .

# Run the container
docker run -d \
  --name blazor-control-panel \
  -p 8080:80 \
  --restart unless-stopped \
  blazor-control-panel:latest
```

### Option 3: Using Docker Compose

```bash
# Start the application
docker-compose up -d

# View logs
docker-compose logs -f

# Stop the application
docker-compose down
```

## 🌐 Accessing the Application

Once deployed, the application will be available at:
- **Main Application**: http://localhost:8080
- **Health Check**: http://localhost:8080/health

## 🔧 Configuration

### Environment Variables

The container supports the following environment variables:

- `NGINX_HOST`: Server hostname (default: localhost)
- `NGINX_PORT`: Server port (default: 80)

### Port Configuration

By default, the application runs on port 80 inside the container and is mapped to port 8080 on the host. You can change this by modifying the port mapping:

```bash
# Run on different port (e.g., 3000)
docker run -d --name blazor-control-panel -p 3000:80 blazor-control-panel:latest
```

## 📊 Performance Optimizations

The nginx configuration includes several optimizations:

### Compression
- **Gzip compression** enabled for all text-based files
- **Brotli compression** support (if available)
- Optimized compression levels for different file types

### Caching Strategy
- **Static assets**: 1 year cache with immutable flag
- **Framework files**: 1 year cache with immutable flag
- **Application files**: No cache to ensure updates are loaded
- **WASM files**: Proper MIME type and long-term caching

### Security Headers
- X-Frame-Options: SAMEORIGIN
- X-Content-Type-Options: nosniff
- X-XSS-Protection: enabled
- Referrer-Policy: strict-origin-when-cross-origin

## 🔍 Monitoring and Health Checks

### Health Check Endpoint
The application includes a health check endpoint at `/health` that returns:
- **Status**: 200 OK
- **Response**: "healthy"

### Docker Health Check
The container includes built-in health checks that run every 30 seconds:
```bash
# Check container health
docker ps
# Look for "healthy" status
```

### Viewing Logs
```bash
# View container logs
docker logs blazor-control-panel

# Follow logs in real-time
docker logs -f blazor-control-panel

# View nginx access logs
docker exec blazor-control-panel tail -f /var/log/nginx/access.log

# View nginx error logs
docker exec blazor-control-panel tail -f /var/log/nginx/error.log
```

## 🛠️ Troubleshooting

### Common Issues

1. **Port Already in Use**
   ```bash
   # Check what's using the port
   lsof -i :8080
   
   # Use a different port
   docker run -d --name blazor-control-panel -p 8081:80 blazor-control-panel:latest
   ```

2. **Container Won't Start**
   ```bash
   # Check container logs
   docker logs blazor-control-panel
   
   # Check container status
   docker ps -a
   ```

3. **Application Not Loading**
   ```bash
   # Test health endpoint
   curl http://localhost:8080/health
   
   # Check nginx configuration
   docker exec blazor-control-panel nginx -t
   ```

### Debugging Commands

```bash
# Enter container shell
docker exec -it blazor-control-panel sh

# Check nginx configuration
docker exec blazor-control-panel cat /etc/nginx/nginx.conf

# Check file permissions
docker exec blazor-control-panel ls -la /usr/share/nginx/html

# Restart nginx inside container
docker exec blazor-control-panel nginx -s reload
```

## 🔄 Updates and Maintenance

### Updating the Application

1. **Rebuild the image**:
   ```bash
   docker build -t blazor-control-panel:latest .
   ```

2. **Stop and remove old container**:
   ```bash
   docker stop blazor-control-panel
   docker rm blazor-control-panel
   ```

3. **Run new container**:
   ```bash
   docker run -d --name blazor-control-panel -p 8080:80 blazor-control-panel:latest
   ```

### Backup and Restore

Since this is a stateless application with local storage, no special backup procedures are needed. User data is stored in browser local storage.

## 🚀 Production Deployment

For production deployment, consider:

1. **Use a reverse proxy** (nginx, Traefik, or cloud load balancer)
2. **Enable HTTPS** with SSL certificates
3. **Set up monitoring** and alerting
4. **Configure log aggregation**
5. **Use container orchestration** (Kubernetes, Docker Swarm)

### Example Production nginx Configuration

```nginx
server {
    listen 443 ssl http2;
    server_name your-domain.com;
    
    ssl_certificate /path/to/certificate.crt;
    ssl_certificate_key /path/to/private.key;
    
    location / {
        proxy_pass http://localhost:8080;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

## 📞 Support

For issues related to Docker deployment, check:
1. Container logs: `docker logs blazor-control-panel`
2. Health check status: `curl http://localhost:8080/health`
3. nginx configuration: `docker exec blazor-control-panel nginx -t`

---

**BlazorControlPanel** - Enterprise CRM Solution with Docker Deployment

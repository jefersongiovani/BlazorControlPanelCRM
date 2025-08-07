#!/bin/bash

# Build script for BlazorControlPanel Docker image

set -e

echo "🚀 Building BlazorControlPanel Docker image..."

# Build the Docker image
docker build -t blazor-control-panel:latest .

echo "✅ Docker image built successfully!"

# Optional: Run the container
read -p "Do you want to run the container now? (y/n): " -n 1 -r
echo
if [[ $REPLY =~ ^[Yy]$ ]]; then
    echo "🏃 Starting container..."
    docker run -d \
        --name blazor-control-panel \
        -p 8080:80 \
        --restart unless-stopped \
        blazor-control-panel:latest
    
    echo "✅ Container started successfully!"
    echo "🌐 Application is available at: http://localhost:8080"
    echo "🔍 Health check: http://localhost:8080/health"
    
    # Show container logs
    echo "📋 Container logs:"
    docker logs blazor-control-panel
else
    echo "ℹ️  To run the container later, use:"
    echo "   docker run -d --name blazor-control-panel -p 8080:80 blazor-control-panel:latest"
fi

echo "🎉 Build process completed!"

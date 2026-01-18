# Build and Push Script for LinkVault

$RegistryUsername = Read-Host "Enter your Docker Hub Username (or press Enter to skip if using Minikube local load)"

if ($RegistryUsername) {
    $BackendImage = "$RegistryUsername/linkvault-backend:latest"
    $FrontendImage = "$RegistryUsername/linkvault-frontend:latest"
} else {
    $BackendImage = "linkvault/backend:latest"
    $FrontendImage = "linkvault/frontend:latest"
}

Write-Host "Building Backend Image: $BackendImage..."
docker build -t $BackendImage -f "src/LinkVault.HttpApi.Host/Dockerfile" .

Write-Host "Building Frontend Image: $FrontendImage..."
docker build -t $FrontendImage -f "angular/Dockerfile" "angular"

if ($RegistryUsername) {
    Write-Host "Pushing images to Docker Hub..."
    docker push $BackendImage
    docker push $FrontendImage
    Write-Host "Images pushed. Please update your values.yaml image.repository to '$RegistryUsername/linkvault-backend' and '$RegistryUsername/linkvault-frontend'."
} else {
    Write-Host "Checking for Minikube..."
    if (Get-Command minikube -ErrorAction SilentlyContinue) {
        Write-Host "Loading images into Minikube..."
        minikube image load $BackendImage
        minikube image load $FrontendImage
        Write-Host "Images loaded into Minikube."
    } else {
        Write-Host "Minikube not found. Images built locally."
    }
}

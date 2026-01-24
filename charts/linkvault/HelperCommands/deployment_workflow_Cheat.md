LinkVault Deployment Cheat Sheet
Use these commands to apply your changes to the Minikube cluster.

IMPORTANT

Always Run First: Point your PowerShell to Minikube's Docker daemon.

& minikube -p minikube docker-env --shell powershell | Invoke-Expression

1. Frontend Changes (Angular)
   If you modified files in angular/:

# 1. Navigate to angular directory

cd angular

# 2. Build the image (this runs 'npm run build:prod' inside Docker)

docker build -t mahmoudali2111/linkvault-frontend:latest .

# 3. Return to root (optional, if you have other commands)

cd ..

# 4. Restart the deployment to pick up the new image

kubectl rollout restart deployment/frontend -n linkvault 2. Backend Changes (.NET)
If you modified files in src/:

# 1. Build the image from the solution root

docker build -t mahmoudali2111/linkvault-backend:latest -f src/LinkVault.HttpApi.Host/Dockerfile .

# 2. Restart the deployment

kubectl rollout restart deployment/backend -n linkvault 3. Configuration Changes (
values.yaml
)
If you modified
charts/linkvault/values.yaml
or other Helm templates:

# Apply the changes

helm upgrade --install linkvault ./charts/linkvault -n linkvault 4. Full Reset (If things get stuck)
If you need to completely remove and reinstall the application:

helm uninstall linkvault -n linkvault
helm upgrade --install linkvault ./charts/linkvault -n linkvault

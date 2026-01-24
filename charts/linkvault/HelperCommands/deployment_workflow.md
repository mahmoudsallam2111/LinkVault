# LinkVault Deployment & Update Workflow

This guide details the steps to update your **LinkVault** application (Frontend or Backend) and deploy the changes to your local **Minikube** Kubernetes cluster.

## Prerequisites
Ensure a PowerShell terminal is open at the root of your project: `d:\LinkVault App\LinkVault`.

---

## 1. Frontend Updates (Angular)
If you made changes to the Angular code (e.g., `angular/src/**/*.ts` or `html`).

### Step 1.1: Rebuild the Application
Build the production bundle and create the Docker image.
```powershell
cd angular
# Build Angular App
npm run build 

# Build Docker Image
docker build -t linkvault/frontend:latest .
cd ..
```

### Step 1.2: Load Image into Minikube
**Critical**: Since Minikube runs in its own VM/container, it cannot see your local Docker images unless you load them.
```powershell
minikube image load linkvault/frontend:latest
```

### Step 1.3: Restart Deployment
Restart the pods to pick up the new image.
```powershell
kubectl rollout restart deployment/frontend -n linkvault
```

---

## 2. Backend Updates (.NET)
If you made changes to the C# code (e.g., `src/**/*.cs`).

### Step 2.1: Rebuild Docker Image
Build the backend image from the root directory.
```powershell
docker build -t mahmoudali2111/linkvault-backend:latest -f src/LinkVault.HttpApi.Host/Dockerfile .
```

### Step 2.2: Load Image into Minikube
```powershell
minikube image load mahmoudali2111/linkvault-backend:latest
```

### Step 2.3: Restart Deployment
```powershell
kubectl rollout restart deployment/backend -n linkvault
```

---

## 3. Configuration Updates (Helm/Ingress)
If you made changes to `charts/linkvault/values.yaml` or any template files (like `ingress.yaml`).

### Step 3.1: Upgrade Helm Release
Apply the changes to the cluster.
```powershell
helm upgrade linkvault ./charts/linkvault -n linkvault
```

---

## Troubleshooting

### Verify Pod Status
Check if pods are running or in error state.
```powershell
kubectl get pods -n linkvault
```

### Check Logs
If a pod is crashing or getting errors:
```powershell
# Frontend Logs
kubectl logs -n linkvault -l app.kubernetes.io/name=frontend

# Backend Logs
kubectl logs -n linkvault -l app.kubernetes.io/name=backend
```

### Access Issues?
Remember that `minikube tunnel` must be running in a separate window to access `http://linkvault.local`.

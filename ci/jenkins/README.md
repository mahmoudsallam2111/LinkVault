# Jenkins for LinkVault CI

A local Jenkins (in Docker Desktop) that replaces Azure Pipelines. It is fully
configured from this folder: admin user, the `dockerhub` credential and the
`linkvault` pipeline job, which runs the repo's `Jenkinsfile`.

## Flow

push to master or release/* on GitHub
→ Jenkins polls every 2 min and builds `linkvault-backend` and `linkvault-frontend`
→ pushes `:<build number>` and `:latest` to Docker Hub
→ Argo CD Image Updater commits the new tag to LinkVault.GitOps `values.yaml`
→ Argo CD syncs the cluster

## Start it

```powershell
cd LinkVault\ci\jenkins
copy .env.example .env     # then edit .env: admin password + Docker Hub token
docker compose up -d --build
```

Open http://localhost:8090 and sign in with the admin user from `.env`.
The first build number is `NEXT_BUILD_NUMBER` (47), which continues after the
tags Azure Pipelines already pushed to Docker Hub.

## Change things

- Pipeline steps: edit `Jenkinsfile` in the repo root.
- Job, credentials or users: edit `casc.yaml` or `.env`, then
  `docker compose up -d --build`.
- Plugins: edit `plugins.txt`, then rebuild the same way.

Builds run on the Jenkins container itself and use your Docker Desktop through
the mounted socket, so the images also appear in `docker images` on your PC.

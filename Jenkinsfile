// LinkVault CI (replaces azure-pipelines.yml)
//
// Builds the backend and frontend images and pushes them to Docker Hub with
// the Jenkins build number as the tag (plus "latest"). Argo CD Image Updater
// watches Docker Hub for new numeric tags and commits them to LinkVault.GitOps,
// and Argo CD then rolls the new version out to the cluster.
//
// Needs a Jenkins credential "dockerhub" (username + access token) and the
// docker CLI on the agent; ci/jenkins/ sets both up.

pipeline {
  agent any

  options {
    disableConcurrentBuilds()
    timestamps()
    buildDiscarder(logRotator(numToKeepStr: '20'))
  }

  environment {
    BACKEND_IMAGE  = 'mahmoudali2111/linkvault-backend'
    FRONTEND_IMAGE = 'mahmoudali2111/linkvault-frontend'
    IMAGE_TAG      = "${env.BUILD_NUMBER}"
  }

  stages {
    stage('Build backend image') {
      steps {
        sh 'docker build -t "$BACKEND_IMAGE:$IMAGE_TAG" -f src/LinkVault.HttpApi.Host/Dockerfile .'
      }
    }

    stage('Build frontend image') {
      steps {
        sh 'docker build -t "$FRONTEND_IMAGE:$IMAGE_TAG" angular'
      }
    }

    stage('Push to Docker Hub') {
      steps {
        withCredentials([usernamePassword(credentialsId: 'dockerhub',
                                          usernameVariable: 'DOCKERHUB_USER',
                                          passwordVariable: 'DOCKERHUB_TOKEN')]) {
          sh '''
            echo "$DOCKERHUB_TOKEN" | docker login -u "$DOCKERHUB_USER" --password-stdin
            for img in "$BACKEND_IMAGE" "$FRONTEND_IMAGE"; do
              docker tag  "$img:$IMAGE_TAG" "$img:latest"
              docker push "$img:$IMAGE_TAG"
              docker push "$img:latest"
            done
          '''
        }
      }
    }
  }

  post {
    always {
      sh 'docker logout || true'
    }
  }
}

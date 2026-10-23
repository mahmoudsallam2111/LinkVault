// LinkVault CI (replaces azure-pipelines.yml)
//
// Builds the backend and/or frontend images (only what changed) and pushes them
// to Docker Hub with the Jenkins build number as the tag (plus "latest").
// Argo CD Image Updater watches Docker Hub for new numeric tags and commits them
// to LinkVault.GitOps, and Argo CD then rolls the new version out to the cluster.
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
    stage('Detect changes') {
      steps {
        script {
          // Determine which files changed since the previous successful commit
          def baseCommit = env.GIT_PREVIOUS_SUCCESSFUL_COMMIT ?: env.GIT_PREVIOUS_COMMIT
          def changedFiles = ''
          if (baseCommit) {
            changedFiles = sh(
              script: "git diff --name-only ${baseCommit} ${env.GIT_COMMIT} || true",
              returnStdout: true
            ).trim()
          }

          if (!changedFiles) {
            echo "No previous build commit found (or manual re-run) — building both backend and frontend."
            env.BUILD_BACKEND  = 'true'
            env.BUILD_FRONTEND = 'true'
          } else {
            echo "Changed files since ${baseCommit}:\n${changedFiles}"
            def lines = changedFiles.split('\n')

            // Frontend changes: anything inside angular/ or pipeline config
            def frontendChanged = lines.any { it.startsWith('angular/') || it == 'Jenkinsfile' }
            // Backend changes: anything inside src/, common.props, NuGet.Config, solution files, or pipeline config
            def backendChanged  = lines.any {
              it.startsWith('src/') ||
              it == 'common.props' ||
              it == 'NuGet.Config' ||
              it == 'LinkVault.slnx' ||
              it == 'Jenkinsfile'
            }

            env.BUILD_BACKEND  = backendChanged.toString()
            env.BUILD_FRONTEND = frontendChanged.toString()
          }

          echo "Build plan -> Backend: ${env.BUILD_BACKEND}, Frontend: ${env.BUILD_FRONTEND}"
        }
      }
    }

    stage('Build backend image') {
      when {
        environment name: 'BUILD_BACKEND', value: 'true'
      }
      steps {
        sh 'docker build -t "$BACKEND_IMAGE:$IMAGE_TAG" -f src/LinkVault.HttpApi.Host/Dockerfile .'
      }
    }

    stage('Build frontend image') {
      when {
        environment name: 'BUILD_FRONTEND', value: 'true'
      }
      steps {
        sh 'docker build -t "$FRONTEND_IMAGE:$IMAGE_TAG" angular'
      }
    }

    stage('Push to Docker Hub') {
      when {
        anyOf {
          environment name: 'BUILD_BACKEND', value: 'true'
          environment name: 'BUILD_FRONTEND', value: 'true'
        }
      }
      steps {
        withCredentials([usernamePassword(credentialsId: 'dockerhub',
                                          usernameVariable: 'DOCKERHUB_USER',
                                          passwordVariable: 'DOCKERHUB_TOKEN')]) {
          sh '''
            echo "$DOCKERHUB_TOKEN" | docker login -u "$DOCKERHUB_USER" --password-stdin

            push_with_retry() {
              for attempt in 1 2 3 4; do
                docker push "$1" && return 0
                echo "Push of $1 failed (attempt $attempt of 4), retrying in 20s..."
                sleep 20
              done
              return 1
            }

            if [ "$BUILD_BACKEND" = "true" ]; then
              docker tag "$BACKEND_IMAGE:$IMAGE_TAG" "$BACKEND_IMAGE:latest"
              push_with_retry "$BACKEND_IMAGE:$IMAGE_TAG"
              push_with_retry "$BACKEND_IMAGE:latest"
            fi

            if [ "$BUILD_FRONTEND" = "true" ]; then
              docker tag "$FRONTEND_IMAGE:$IMAGE_TAG" "$FRONTEND_IMAGE:latest"
              push_with_retry "$FRONTEND_IMAGE:$IMAGE_TAG"
              push_with_retry "$FRONTEND_IMAGE:latest"
            fi
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
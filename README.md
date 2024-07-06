# EventManagementSystem

## Project Overview

This project demonstrates a .NET User Service application with a Redis sidecar, designed for deployment on Kubernetes. It showcases:

- A .NET Web API for user management
- Redis integration for caching and data persistence
- Kubernetes deployment with a sidecar container pattern
- Local development and testing using Kind (Kubernetes in Docker)

## Table of Contents

1. [Prerequisites](#prerequisites)
3. [Local Development Setup](#local-development-setup)
4. [Building the Application](#building-the-application)
5. [Kubernetes Deployment](#kubernetes-deployment)
6. [Accessing the Application](#accessing-the-application)
7. [Troubleshooting](#troubleshooting)

## Prerequisites

Ensure you have the following installed:

- [Docker](https://www.docker.com/get-started) or [Podman](https://podman.io/getting-started/installation)
- [kubectl](https://kubernetes.io/docs/tasks/tools/)
- [Kind](https://kind.sigs.k8s.io/docs/user/quick-start/#installation)
- [.NET SDK](https://dotnet.microsoft.com/download) (version 8.0)


## Local Development Setup

1. Clone the repository:git clone https://github.com/tito-datta/EventManagementSystem.git 

2. `cd EventManagementSystem`

3. Set up the Kind cluster:

4. `kind create cluster --name my-app-dev --config dev-cluster.yaml`

## Building the Application

1. Build the Docker image:
`docker build -t localhost/user-service:latest .`
Or with Podman:
`podman build -t localhost/user-service:latest .`

2. Load the image into Kind:
For Docker:
`kind load docker-image localhost/user-service:latest --name my-app-dev`

For Podman:
`podman save localhost/user-service:latest -o user-service.tar`
`kind load image-archive user-service.tar --name my-app-dev`

## Kubernetes Deployment

1. Apply the Kubernetes manifests:
`kubectl apply -f dev.yaml`

2. Verify the deployment:
`kubectl get pods`
`kubectl get services`

## Accessing the Application

1. Set up port forwarding:
`kubectl port-forward service/user-service 8080:8080`

2. The application is now accessible at `http://localhost:8080`

## Troubleshooting

If you encounter issues:

1. Check pod status:
`kubectl describe pod <pod-name>`

2. View logs:
`kubectl logs <pod-name> -c user-service`
`kubectl logs <pod-name> -c redis-sidecar`

3. Verify images in Kind:
`podman exec -it my-app-dev-control-plane crictl images`

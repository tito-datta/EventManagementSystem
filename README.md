# EventManagementSystem

## Project Overview

This project demonstrates a .NET User Service application with a Redis sidecar, designed for deployment on Kubernetes. It showcases:

- A .NET Web API for user & membership management
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

2. Set up the cluster

Kind:

    kind create cluster --name <cluster-name> --config dev-cluster.yaml

## Building the Application

1. Build the image

Docker:

    docker build -f /path/to/dockerfile -t <image-name:tag> .

Podman:

    podman build -f /path/to/dockerfile -t <image-name:tag> .

2. Load the image into Kind:

Docker:

    kind load docker-image <image-name:tag> --name <cluster-name>

Podman:

    podman save <image-name:tag> -o <file-name>.tar

    kind load image-archive <file-name>.tar --name <cluster-name>

## Kubernetes Deployment

Apply the Kubernetes manifests:

    kubectl apply -f dev.yaml

Verify the deployment:

    kubectl get pods -n <namespace>

    kubectl get services -n <namespace>

## Accessing the Application

Set up port forwarding:

    kubectl port-forward service/user-service 8080:8080 8001:8001

The application is now accessible at the 8080 locally. 

note: port 8001 hosts the Redis persistent db service 

## Troubleshooting

If you encounter issues:

Check pod status:

    kubectl describe pod <pod-name> -n <namespace>

View logs:

    kubectl logs <pod-name> -c <container-name> -n <namespace>

Verify images in Kind:
    
    podman exec -it <cluster-name>-control-plane crictl images


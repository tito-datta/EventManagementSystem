function Are-PodsReady {
    $pods = kubectl get pods -o json | ConvertFrom-Json
    foreach ($pod in $pods.items) {
        if ($pod.status.phase -eq "Pending") {
            return $false
        }
        foreach ($containerStatus in $pod.status.containerStatuses) {
            if ($containerStatus.state.waiting.reason -eq "ContainerCreating") {
                return $false
            }
        }
    }
    return $true
}

Write-Host "Waiting for all pods to be ready..."
while (-not (Are-PodsReady)) {
    Write-Host "Some pods are still being created. Waiting 5 seconds before checking again..."
    Start-Sleep -Seconds 5
}

Write-Host "All pods are now ready!"
kubectl get pods
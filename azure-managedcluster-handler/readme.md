# Overview

Creates an Azure managed AKS cluster based on injected inputs. AKS config data looks like this:

```
config:
  azure-native:location: WestUS2
  azure-aks-handler:arkdata:
    VnetResourceGroup: rg-ameere4c52133
    SubNetName: snet-tier2-aks
    VnetName: ameerd2b39280
    Aks:
      Name: aks01
      ResourceGroup: ark-rg-aks01
      ServicePrincipal: "ark-k8s-sp"
      VmSize: Standard_B4ms
      EnablePrivateCluster: true
      NetworkProfile:
        NetworkPlugin: "azure"
        NetworkPolicy: "calico"
        DockerBridgeCidr: "172.17.0.0/16"
```
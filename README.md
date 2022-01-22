# Overview

Based on existing TF, we need to run the folders in the following order:

| Number | Folder  Name | Description |
| - | - | - |
| 1 | azure-statestore | Creates resource group, storage accounts and containers for custom state storage if required. **Note** : We are defaulting now to Pulumi cloud|
| 2 | azure-base-resuroucegroups | Creates "er" resource group for express route and others as appropriate" |
| 3 | azure-hub | Creates an Azure Hub network for a hub-spoke topology|




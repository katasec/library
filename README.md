# Overview

Each folder in this repo will contain an independant pulumi program responsible for setting up some infrastructure. The Pulumi programs can be written in any supported language. The programs will be executed by an ark [worker](https://github.com/katasec/ark/tree/main/worker). The worker will :

- Download the Pulumi program to source it as a local program via the Pulumi automation API
- Inject any required input required by the pulumi program into it's `Pulumi.<stack>.yaml`.
- Execute the Pulumi program


| Number | Folder  Name | Description |
| - | - | - |
| 1 | azurecloudspace-handler | Creates a `cloudspace` and one ore more `environments` in the azure cloudspace. This is aninitial setup before deploying any infrastructure|
| 2 | azure-managedcluster-handler | Creates an AKS cluster in an existing cloudspace `environment`|




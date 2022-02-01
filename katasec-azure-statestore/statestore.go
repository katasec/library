package main

import (
	"github.com/pulumi/pulumi-azure-native/sdk/go/azure/resources"
	"github.com/pulumi/pulumi-azure-native/sdk/go/azure/storage"
	"github.com/pulumi/pulumi/sdk/v3/go/pulumi"
)

type createStateStoreRequest struct {
	companyShortName string
	stateStoreRGName string
	containers       []string
}

func NewStateStoreRequest() (request *createStateStoreRequest) {
	return &createStateStoreRequest{
		companyShortName: "ks",
		stateStoreRGName: "base",
		containers: []string{
			"mgmt",
			"dev",
			"uat",
			"prod",
			"iam",
			"adds",
			"exr",
		},
	}
}

// createBase Creates the base storage accounts and containers required
// as part of the Virtual Data Centre
func createStateStore(ctx *pulumi.Context) (err error) {

	request := NewStateStoreRequest()

	// Create Resource Group for State Store
	rgName, err := resources.NewResourceGroup(ctx, request.stateStoreRGName, &resources.ResourceGroupArgs{
		Location: pulumi.String(location),
	})
	if err != nil {
		return err
	}

	// Output Resource Group Details
	ctx.Export("BaseRG_ID", rgName.ID())
	ctx.Export("BaseRG_URN", rgName.URN())
	ctx.Export("BaseRG_Name", rgName.Name)

	// Create Storage Account for State Store
	stAccountName := "st" + request.companyShortName
	storageAccount, err := storage.NewStorageAccount(ctx, stAccountName, &storage.StorageAccountArgs{
		ResourceGroupName: rgName.Name,
		Sku: &storage.SkuArgs{
			Name: storage.SkuName_Standard_LRS,
		},
		Kind: storage.KindStorageV2,
	})
	if err != nil {
		return err
	}

	// Create containers for state store
	for _, name := range request.containers {
		_, err = storage.NewBlobContainer(ctx, name, &storage.BlobContainerArgs{
			AccountName:       storageAccount.Name,
			ResourceGroupName: rgName.Name,
		})
	}
	if err != nil {
		return err
	}

	return nil
}

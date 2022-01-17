package main

import (
	"github.com/pulumi/pulumi-azure-native/sdk/go/azure/resources"
	"github.com/pulumi/pulumi-azure-native/sdk/go/azure/storage"
	"github.com/pulumi/pulumi/sdk/v3/go/pulumi"
)

var (
	location = "SouthEastAsia"
)

func RunPulumi() {
	pulumi.Run(func(ctx *pulumi.Context) error {
		return createStuff(ctx)
	})
}

func main() {
	RunPulumi()
}

func createStuff(ctx *pulumi.Context) error {

	groupNames := []string{"group1", "group2", "group3"}

	for _, groupName := range groupNames {
		// Create an Azure Resource Group
		resourceGroup, err := createResourceGroup(ctx, groupName)
		if err != nil {
			return err
		}

		// Create an Storage Account in the resource group
		_, err = createStorageAccount(ctx, resourceGroup, groupName+"sa")
		if err != nil {
			return err
		}

		// // Capture the primary key of the Storage Account into context
		// ctx.Export("primaryStorageKey", pulumi.All(resourceGroup.Name, account.Name).ApplyT(
		// 	func(args []interface{}) (string, error) {
		// 		return exportStorageKeys(ctx, args)
		// 	},
		// ))

	}

	return nil

}

func createResourceGroup(ctx *pulumi.Context, name string) (*resources.ResourceGroup, error) {
	// Create an Azure Resource Group
	resourceGroup, err := resources.NewResourceGroup(ctx, name, &resources.ResourceGroupArgs{
		Location: pulumi.String(location),
	})
	if err != nil {
		return nil, err
	}

	return resourceGroup, nil
}

func createStorageAccount(ctx *pulumi.Context, resourceGroup *resources.ResourceGroup, groupName string) (*storage.StorageAccount, error) {
	account, err := storage.NewStorageAccount(ctx, groupName, &storage.StorageAccountArgs{
		ResourceGroupName: resourceGroup.Name,
		Sku: &storage.SkuArgs{
			Name: storage.SkuName_Standard_LRS,
		},
		Kind: storage.KindStorageV2,
	})

	// Capture the primary key of the Storage Account into context
	ctx.Export(groupName+"_primaryStorageKey", pulumi.All(resourceGroup.Name, account.Name).ApplyT(
		func(args []interface{}) (string, error) {
			return exportStorageKeys(ctx, args)
		},
	))

	return account, err
}

func exportStorageKeys(ctx *pulumi.Context, args []interface{}) (string, error) {
	resourceGroupName := args[0].(string)
	accountName := args[1].(string)
	accountKeys, err := storage.ListStorageAccountKeys(ctx, &storage.ListStorageAccountKeysArgs{
		ResourceGroupName: resourceGroupName,
		AccountName:       accountName,
	})
	if err != nil {
		return "", err
	}

	return accountKeys.Keys[0].Value, nil
}

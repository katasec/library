package main

import (
	"fmt"
	"os"

	"github.com/pulumi/pulumi-azure-native/sdk/go/azure/network"
	"github.com/pulumi/pulumi/sdk/v3/go/pulumi"
)

type CreateVNETHub struct {
	VNETName        string   `json:"VNETName"`
	AddressPrefixes []string `json:"AddressPrefixes"`
}

func createHub(ctx *pulumi.Context, request CreateVNETHub) (err error) {

	// Declare remote stack details
	remoteStack := "katasec/katasec-azure-statestore/dev"
	remoteURL := "https://github.com/katasec/library.git"
	remoteProjectPath := "katasec-azure-statestore"

	// Get resource group from remote stack
	_, resourceGroup, _ := getResourceGroup(remoteStack, remoteURL, remoteProjectPath)

	// Create VNET in resource group
	createVNETHub(ctx, resourceGroup, request.VNETName, request.AddressPrefixes)

	return nil
}

func createVNETHub(ctx *pulumi.Context, resourceGroup string, vnetName string, addressPrefix []string) (err error) {

	tags := make(map[string]string)
	tags["ks_resource_type"] = "network"
	tags["ks_resource_role"] = "hub"

	hubVNET, err := network.NewVirtualNetwork(ctx, "virtualNetwork", &network.VirtualNetworkArgs{
		AddressSpace: &network.AddressSpaceArgs{
			AddressPrefixes: pulumi.StringArray{
				pulumi.String(addressPrefix[0]),
			},
		},
		Location:           pulumi.String(location),
		ResourceGroupName:  pulumi.String(resourceGroup),
		VirtualNetworkName: pulumi.String(vnetName),
		Tags:               pulumi.ToStringMap(tags),
	})

	if err != nil {
		fmt.Printf("Failed to create vnet: %v\n", err)
		os.Exit(1)
	}

	// Export VNET details
	ctx.Export("HubVNET_ID", hubVNET.ID())
	ctx.Export("HubVNET_Name", hubVNET.Name)

	return nil
}

func getResourceGroup(remoteStack string, remoteURL string, remoteProjectPath string) (baseRGID string, baseRGName string, err error) {

	// Get outputs from remote stack
	outs := getStackOutputs(remoteStack, remoteURL, remoteProjectPath)

	// Get resource group ID
	baseRGID, ok := outs["BaseRG_ID"].Value.(string)
	if !ok {
		fmt.Println("failed to get ID output")
		os.Exit(1)
	}

	// Get resource group name
	baseRGName, ok = outs["BaseRG_Name"].Value.(string)
	if !ok {
		fmt.Println("failed to get NAME output")
		os.Exit(1)
	}

	return baseRGID, baseRGName, err
}

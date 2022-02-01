package main

import (
	"context"
	"fmt"
	"os"

	"github.com/pulumi/pulumi-azure-native/sdk/go/azure/network"
	"github.com/pulumi/pulumi/sdk/v3/go/auto"
	"github.com/pulumi/pulumi/sdk/v3/go/pulumi"
)

func createHub(ctx *pulumi.Context) (err error) {

	// Declare remote stack details
	remoteStack := "katasec/katasec-azure-statestore/dev"
	remoteURL := "https://github.com/katasec/library.git"
	remoteProjectPath := "katasec-azure-statestore"

	// Get stack outputs from remote stack
	_, resourceGroup, _ := getStackOutputs(remoteStack, remoteURL, remoteProjectPath)

	createVNET(ctx, resourceGroup, "hub", "10.0.0.0/16")

	return nil
}

func createVNET(ctx *pulumi.Context, resourceGroup string, vnetName string, addressPrefix string) (err error) {
	hubVNET, err := network.NewVirtualNetwork(ctx, "virtualNetwork", &network.VirtualNetworkArgs{
		AddressSpace: &network.AddressSpaceArgs{
			AddressPrefixes: pulumi.StringArray{
				pulumi.String(addressPrefix),
			},
		},
		Location:           pulumi.String(location),
		ResourceGroupName:  pulumi.String(resourceGroup),
		VirtualNetworkName: pulumi.String(vnetName),
	})

	if err != nil {
		fmt.Printf("Failed to create vnet: %v\n", err)
		os.Exit(1)
	}

	ctx.Export("HubVNET_ID", hubVNET.ID())
	ctx.Export("HubVNET_Name", hubVNET.Name)

	return nil
}

func getStackOutputs(remoteStack string, remoteURL string, remoteProjectPath string) (baseRGID string, baseRGName string, err error) {

	myctx := context.Background()

	myrepo := auto.GitRepo{
		URL:         remoteURL,
		ProjectPath: remoteProjectPath,
	}

	stack, _ := auto.UpsertStackRemoteSource(myctx, remoteStack, myrepo)

	_, err = stack.Refresh(myctx)
	if err != nil {
		fmt.Printf("failed to refresh stack: %v\n", err)
		os.Exit(1)
	} else {
		fmt.Print("Refreshed stack !")
	}

	outs, err := stack.Outputs(myctx)
	if err != nil {
		fmt.Printf("failed to get  outputs: %v\n", err)
		os.Exit(1)
	}

	baseRGID, ok := outs["BaseRG_ID"].Value.(string)
	if !ok {
		fmt.Println("failed to get ID output")
		os.Exit(1)
	}

	baseRGName, ok = outs["BaseRG_Name"].Value.(string)
	if !ok {
		fmt.Println("failed to get NAME output")
		os.Exit(1)
	}

	return baseRGID, baseRGName, err
}

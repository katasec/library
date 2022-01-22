package main

import (
	"github.com/pulumi/pulumi-azure-native/sdk/go/azure/resources"
	"github.com/pulumi/pulumi/sdk/v3/go/pulumi"
)

type createBaseResourceGroups struct {
	resourceGroups []string
	location       string
}

func NewBaseResourceGroupsRequest() (request *createBaseResourceGroups) {
	return &createBaseResourceGroups{

		// Add additional resource groups here as appropriate
		resourceGroups: []string{
			"er",
		},
		location: location,
	}
}

func createResourceGroups(ctx *pulumi.Context) (err error) {

	request := NewBaseResourceGroupsRequest()
	for _, name := range request.resourceGroups {
		_, err := resources.NewResourceGroup(ctx, name, &resources.ResourceGroupArgs{
			Location: pulumi.String(request.location),
		})

		if err != nil {
			return err
		}
	}
	return err
}

package main

import (
	"github.com/pulumi/pulumi/sdk/v3/go/pulumi"
)

var (
	location = "SouthEastAsia"
)

func RunPulumi() {

	request := CreateVNETHub{
		VNETName:        "hub",
		AddressPrefixes: []string{"10.0.0.0/16"},
	}

	pulumi.Run(func(ctx *pulumi.Context) error {
		return createHub(ctx, request)
	})
}

func main() {
	RunPulumi()
}

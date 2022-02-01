package main

import (
	"github.com/pulumi/pulumi/sdk/v3/go/pulumi"
)

var (
	location = "SouthEastAsia"
)

func RunPulumi() {
	pulumi.Run(func(ctx *pulumi.Context) error {
		return createResourceGroups(ctx)
	})
}

func main() {
	RunPulumi()

}

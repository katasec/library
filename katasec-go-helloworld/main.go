package main

import (
	"github.com/pulumi/pulumi/sdk/v3/go/pulumi"
)

var (
	location = "SouthEastAsia"
)

func main() {
	pulumi.Run(func(ctx *pulumi.Context) error {
		ctx.Export("Hello", "World")
		return nil
	})
}

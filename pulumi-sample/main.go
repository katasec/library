package main

import (
	"github.com/pulumi/pulumi/sdk/v3/go/pulumi"
	"github.com/pulumi/pulumi/sdk/v3/go/pulumi/config"
)

func main() {
	pulumi.Run(func(ctx *pulumi.Context) error {

		// Get Hello Messag as an input from Config
		conf := config.New(ctx, "")
		message := conf.Require("message")

		// Empty Pulumi program that exports one output
		ctx.Export("Output", pulumi.String(message))

		return nil

	})

}

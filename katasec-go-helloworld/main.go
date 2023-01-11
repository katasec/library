package main

import (
	requests "github.com/katasec/ark/requests"
	"github.com/pulumi/pulumi/sdk/v3/go/pulumi"
	"github.com/pulumi/pulumi/sdk/v3/go/pulumi/config"
)

var (
	location = "SouthEastAsia"
)

func main() {
	pulumi.Run(func(ctx *pulumi.Context) error {

		cfg := config.New(ctx, "")

		var data requests.AzureCloudspaceRequest
		cfg.RequireObject("arkdata", &data)

		ctx.Export("Hello", pulumi.String("World"))

		ctx.Export("ArkData_Hub", pulumi.String(data.Hub.Name))
		return nil
	})
}

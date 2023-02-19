package main

import (
	"github.com/katasec/ark/sdk/v0/messages"
	"github.com/pulumi/pulumi/sdk/v3/go/pulumi"
	"github.com/pulumi/pulumi/sdk/v3/go/pulumi/config"
)

func main() {
	pulumi.Run(Handler)
}

func readConfig(ctx *pulumi.Context) messages.AzureAks {
	var data messages.AzureAks
	cfg := config.New(ctx, "")
	cfg.RequireObject("arkdata", &data)
	return data
}

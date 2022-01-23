package main

import (
	"context"
	"fmt"

	"github.com/pulumi/pulumi/sdk/v3/go/auto"
	"github.com/pulumi/pulumi/sdk/v3/go/pulumi"
)

func createHub(ctx *pulumi.Context) (err error) {

	myctx := context.Background()

	myrepo := auto.GitRepo{
		URL:         "https://github.com/katasec/library.git",
		ProjectPath: "azure-base-resourcegroups",
	}

	stack, _ := auto.UpsertStackRemoteSource(myctx, "katasec/azure-base-resourcegroups/dev", myrepo)
	_, err = stack.Refresh(myctx)
	if err != nil {
		fmt.Print("Could not refresh stack !")
		return err
	} else {
		fmt.Print("Refreshed stack !")
	}

	//ctx.Export("ResourceCount", pulumi.String(stack.Name()))

	ctx.Export("TestThing", pulumi.String("Awesome Value"))
	return nil
}

package main

import (
	"context"
	"fmt"
	"os"

	"github.com/pulumi/pulumi/sdk/v3/go/auto"
	"github.com/pulumi/pulumi/sdk/v3/go/pulumi"
)

func createHub(ctx *pulumi.Context) (err error) {

	myctx := context.Background()

	myrepo := auto.GitRepo{
		URL:         "https://github.com/katasec/library.git",
		ProjectPath: "katasec-azure-statestore",
	}

	stack, _ := auto.UpsertStackRemoteSource(myctx, "katasec/katasec-azure-statestore/dev", myrepo)

	_, err = stack.Refresh(myctx)
	if err != nil {
		fmt.Print("Could not refresh stack !")
		return err
	} else {
		fmt.Print("Refreshed stack !")
	}

	outs, err := stack.Outputs(myctx)
	if err != nil {
		fmt.Printf("failed to get  outputs: %v\n", err)
		os.Exit(1)
	}

	baseRGID, ok := outs["ID"].Value.(string)
	if !ok {
		fmt.Println("failed to get ID output")
		os.Exit(1)
	}

	baseRGName, ok := outs["Name"].Value.(string)
	if !ok {
		fmt.Println("failed to get NAME output")
		os.Exit(1)
	}

	ctx.Export("ID", pulumi.String(baseRGID))
	ctx.Export("Name", pulumi.String(baseRGName))

	return nil
}

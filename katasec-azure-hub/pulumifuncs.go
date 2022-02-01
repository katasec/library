package main

import (
	"context"
	"fmt"
	"os"

	"github.com/pulumi/pulumi/sdk/v3/go/auto"
)

func getStackOutputs(remoteStack string, remoteURL string, remoteProjectPath string) (outs auto.OutputMap) {

	myctx := context.Background()

	myrepo := auto.GitRepo{
		URL:         remoteURL,
		ProjectPath: remoteProjectPath,
	}

	stack, err := auto.UpsertStackRemoteSource(myctx, remoteStack, myrepo)
	if err != nil {
		fmt.Printf("failed to select stack: %v\n", err)
		os.Exit(1)
	}
	_, err = stack.Refresh(myctx)
	if err != nil {
		fmt.Printf("failed to refresh stack: %v\n", err)
		os.Exit(1)
	} else {
		fmt.Print("Refreshed stack !")
	}

	outs, err = stack.Outputs(myctx)
	if err != nil {
		fmt.Printf("failed to get outputs: %v\n", err)
		os.Exit(1)
	}

	return outs
}

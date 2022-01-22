package main

import "github.com/pulumi/pulumi/sdk/v3/go/pulumi"

func createHub(ctx *pulumi.Context) (err error) {

	ctx.Export("TestThing", pulumi.String("Awesome Value"))

	if err != nil {
		return err
	}

	return err
}

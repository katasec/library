package main

import (
	"github.com/pulumi/pulumi/sdk/v3/go/pulumi"
	"github.com/pulumi/pulumi/sdk/v3/go/pulumi/config"
)

type ArkData struct {
	VnetResourceGroup string
	SubNetName        string
	VnetName          string

	Aks AksConfig
}

type AksConfig struct {
	Name                 string
	ResourceGroup        string
	ServicePrincipal     string
	VmSize               string
	DnsPrefix            string
	EnablePrivateCluster bool
	NetworkProfile       NetworkProfile
}

type NetworkProfile struct {
	NetworkPlugin    string
	NetworkPolicy    string
	DockerBridgeCidr string
}

func main() {
	pulumi.Run(Handler)
}

func readConfig(ctx *pulumi.Context) ArkData {
	var data ArkData
	cfg := config.New(ctx, "")
	cfg.RequireObject("arkdata", &data)
	return data
}

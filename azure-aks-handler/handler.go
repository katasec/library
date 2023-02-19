package main

import (
	"fmt"
	"log"

	"github.com/pulumi/pulumi-azure-native/sdk/go/azure/containerservice"
	network "github.com/pulumi/pulumi-azure-native/sdk/go/azure/network"
	"github.com/pulumi/pulumi-azure-native/sdk/go/azure/resources"
	"github.com/pulumi/pulumi-azuread/sdk/v5/go/azuread"
	"github.com/pulumi/pulumi/sdk/v3/go/pulumi"
)

func Handler(ctx *pulumi.Context) (err error) {

	//ctx.Export("message", pulumi.String("hello world"))
	launchK8s(ctx)

	return err
}

func launchK8s(ctx *pulumi.Context) {

	// Read config
	cfg := readConfig(ctx)

	//lookup subnet
	k8sSubnet := network.LookupSubnetOutput(ctx, network.LookupSubnetOutputArgs{
		ResourceGroupName:  pulumi.String(cfg.VnetResourceGroup),
		SubnetName:         pulumi.String(cfg.SubNetName),
		VirtualNetworkName: pulumi.String(cfg.VnetName),
	})

	// Create Resource Group
	myrg, err := resources.NewResourceGroup(ctx, cfg.Aks.ResourceGroup, &resources.ResourceGroupArgs{})
	ExitOnError(err)
	ctx.Export("aks_rg", myrg.Name)

	// Create service principal
	sp, spPwd, err := CreateAzureServicePrincipal(ctx, cfg.Aks.ServicePrincipal)
	ExitOnError(err)

	// Create Aks cluster
	managedCluster, err := containerservice.NewManagedCluster(ctx, cfg.Aks.Name, &containerservice.ManagedClusterArgs{
		ResourceGroupName: myrg.Name,
		Identity: &containerservice.ManagedClusterIdentityArgs{
			Type: containerservice.ResourceIdentityTypeSystemAssigned,
		},
		ServicePrincipalProfile: &containerservice.ManagedClusterServicePrincipalProfileArgs{
			ClientId: sp.ID(),
			Secret:   spPwd.Value,
		},
		AgentPoolProfiles: containerservice.ManagedClusterAgentPoolProfileArray{
			containerservice.ManagedClusterAgentPoolProfileArgs{
				AvailabilityZones: pulumi.StringArray{
					pulumi.String("1"),
					pulumi.String("2"),
					pulumi.String("3"),
				},
				Count:              pulumi.Int(1),
				EnableNodePublicIP: pulumi.Bool(false),
				Mode:               pulumi.String("System"),
				Name:               pulumi.String("agentpool"),
				OsType:             pulumi.String("Linux"),
				Type:               pulumi.String("VirtualMachineScaleSets"),
				VmSize:             pulumi.String(cfg.Aks.VmSize),
				VnetSubnetID:       k8sSubnet.Id(),
			},
		},
		DnsPrefix: pulumi.String("ark"),
		NetworkProfile: &containerservice.ContainerServiceNetworkProfileArgs{
			NetworkPlugin:    pulumi.String(cfg.Aks.NetworkProfile.NetworkPlugin),
			NetworkPolicy:    pulumi.String(cfg.Aks.NetworkProfile.NetworkPolicy),
			DockerBridgeCidr: pulumi.String(cfg.Aks.NetworkProfile.DockerBridgeCidr),
		},
		ApiServerAccessProfile: &containerservice.ManagedClusterAPIServerAccessProfileArgs{
			EnablePrivateCluster: pulumi.BoolPtr(true),
		},
	})
	ExitOnError(err)
	ctx.Export("aks_name", managedCluster.Name)

	// Output local config as debug
	ctx.Export("VnetResourceGroup", pulumi.String(cfg.VnetResourceGroup))
	ctx.Export("SubNetName", pulumi.String(cfg.SubNetName))
	ctx.Export("VnetName", pulumi.String(cfg.VnetName))
	ctx.Export("Aks.Name", pulumi.String(cfg.Aks.Name))
	ctx.Export("Aks.ResourceGroup", pulumi.String(cfg.Aks.ResourceGroup))
	ctx.Export("Aks.ServicePrincipal", pulumi.String(cfg.Aks.ServicePrincipal))
	ctx.Export("Aks.VmSize", pulumi.String(cfg.Aks.VmSize))
	ctx.Export("Aks.EnablePrivateCluster", pulumi.BoolPtr(cfg.Aks.EnablePrivateCluster))
	ctx.Export("Aks.NetworkProfile.NetworkPlugin", pulumi.String(cfg.Aks.NetworkProfile.NetworkPlugin))
	ctx.Export("Aks.NetworkProfile.NetworkPolicy", pulumi.String(cfg.Aks.NetworkProfile.NetworkPolicy))
	ctx.Export("Aks.NetworkProfile.DockerBridgeCidr", pulumi.String(cfg.Aks.NetworkProfile.DockerBridgeCidr))

}

func CreateAzureServicePrincipal(ctx *pulumi.Context, name string) (*azuread.ServicePrincipal, *azuread.ServicePrincipalPassword, error) {

	var err error
	// Create a new registered Azure AD app
	app, err := azuread.NewApplication(ctx, name+"-app", &azuread.ApplicationArgs{
		DisplayName: pulumi.String(name + "-app"),
	})
	if err != nil {
		log.Fatal(err.Error())
	}

	// Create a service principal in the App
	sp, err := azuread.NewServicePrincipal(ctx, name+"-sp", &azuread.ServicePrincipalArgs{
		ApplicationId: app.ApplicationId,
	})
	if err != nil {
		log.Fatal(err.Error())
	}

	// Create password for the service principal
	spPassword, err := azuread.NewServicePrincipalPassword(ctx, name+"-secret", &azuread.ServicePrincipalPasswordArgs{
		ServicePrincipalId: sp.ID(),
		EndDateRelative:    pulumi.StringPtr("8760h"),
		DisplayName:        pulumi.StringPtr(fmt.Sprintf("%v-password", name+"-secret")),
	})

	ctx.Export("spPassword", spPassword)
	if err != nil {
		log.Fatal(err.Error())
	}

	return sp, spPassword, err
}

func ExitOnError(err error) {
	if err != nil {
		panic(err)
	}
}

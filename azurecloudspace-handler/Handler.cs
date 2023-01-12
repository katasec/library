using System.Collections.Generic;
using Resources = Pulumi.AzureNative.Resources;
using Network = Pulumi.AzureNative.Network;
using System;
using ArkServer.Entities.Azure;

namespace AzureCloudspaceHandler;

public static partial class Handler
{
    public static string Location = "westus2";

    public static void CreateAzureSpace()
    {
        var (hubRg, hubVnet, hubFw) = CreateHub();
    }

    /// <summary>
    /// The Hub network is not directly visible to the end user. It is, however, implemented behind the scenes.
    /// </summary>
    public static Tuple<Resources.ResourceGroup, Network.VirtualNetwork, Network.AzureFirewall> CreateHub()
    {
        // Create hub resource group
        var hubRg = new Resources.ResourceGroup("rg-hub-", new()
        {
            Tags= { { "ark:managed", "true" }},
        });

        // Define subnets for hub
        List<Network.Inputs.SubnetArgs> hubSubnets = new ();
        foreach (var subnet in ConfigData.Hub.SubnetsInfo)
        {
            var s = new Network.Inputs.SubnetArgs
            {
                AddressPrefix = subnet.AddressPrefix,
                Name = subnet.Name
            };
            hubSubnets.Add(s);
        }

        // Create VNET with above subnets
        var virtualNetwork = new Network.VirtualNetwork(ConfigData.Hub.Name, new Network.VirtualNetworkArgs
        {
            ResourceGroupName = hubRg.Name,
            AddressSpace = new Network.Inputs.AddressSpaceArgs
            {
                AddressPrefixes = ConfigData.Hub.AddressPrefix,
            },
            Location = Location,

            VirtualNetworkName = ConfigData.Hub.Name,
            Subnets = hubSubnets
        });

        // Create Firewall
        var firewall = CreateFirewall(hubRg, virtualNetwork);

        return Tuple.Create(hubRg, virtualNetwork, firewall);
        //var subnet = new Network.Subnet()
    }
    /// <summary>
    /// Start creates the azure cloudspace which is essentially hub + one or more spokes
    /// </summary>
    /// <returns></returns>

    public static void AddSpokes(Resources.ResourceGroup hubRg, Network.VirtualNetwork hubVnet, Network.AzureFirewall firewall)
    {
        foreach (var spoke in ConfigData.Env)
        {
            AddASpoke(spoke, hubRg, hubVnet, firewall);
        }
    }

    public static void AddASpoke(VNetInfo spoke, Resources.ResourceGroup hubRg, Network.VirtualNetwork hubVnet, Network.AzureFirewall firewall)
    {
        // Create Spoke Resource Group
        var spokeRgName = $"rg-{spoke.Name}";
        var spokeRg = new Resources.ResourceGroup(spokeRgName);

        // Route to the firewall
        var routeName = $"rt-{spoke.Name}";
        var spokeRoute = CreateFirewallRoute(spokeRg,firewall, routeName);

    }

    public static Dictionary<string, object?> Start()
    {
        var exports = new Dictionary<string, object?>();

        CreateHub();

        exports.Add("Hello", "World");
        exports.Add("Checking", "Update");
        return exports;
    }

}

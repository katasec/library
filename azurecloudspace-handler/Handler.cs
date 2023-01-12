using System.Collections.Generic;
using Resources = Pulumi.AzureNative.Resources;
using Network = Pulumi.AzureNative.Network;
namespace AzureCloudspaceHandler;

public static partial class Handler
{
    public static string Location = "westus2";
    /// <summary>
    /// The Hub network is not directly visible to the end user. It is, however, implemented behind the scenes.
    /// </summary>
    public static void CreateHub()
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
        //var subnet = new Network.Subnet()
    }


    /// <summary>
    /// Start creates the azure cloudspace which is essentially hub + one or more spokes
    /// </summary>
    /// <returns></returns>
    public static Dictionary<string, object?> Start()
    {
        var exports = new Dictionary<string, object?>();

        CreateHub();

        exports.Add("Hello", "World");
        exports.Add("Checking", "Update");
        return exports;
    }
}



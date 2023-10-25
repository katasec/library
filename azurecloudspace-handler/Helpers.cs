using System;
using Pulumi;
using Network = Pulumi.AzureNative.Network;
using Resources = Pulumi.AzureNative.Resources;
using System.Collections.Generic;
using AzureCloudspaceHandler.Models;

namespace AzureCloudspaceHandler;

public static partial class Handler
{
    public static readonly AzureCloudspace ConfigData = new();

    /// <summary>
    /// The constructor reads pulumi config injected by the worker into this handler.
    /// </summary>
    static Handler()
    {
        Config cfg = new();
        var azureCloudspace = cfg.GetObject<AzureCloudspace>("arkdata");
        if (azureCloudspace == null)
        {
            Console.WriteLine("Config data from Ark is empty, got null");
            Environment.Exit(0);
        }
        else
        {
            ConfigData = azureCloudspace;
        }
    }

    public static Tuple<Network.FirewallPolicy,Network.FirewallPolicyRuleCollectionGroup> CreateFirewallPolicy(Resources.ResourceGroup rg)
    {
        // Create a Firewall Policy
        var firewallPolicy = new Network.FirewallPolicy("hub-policy-", new()
        {
            ResourceGroupName = rg.Name,
            Sku = new Network.Inputs.FirewallPolicySkuArgs
            {
                Tier = "Basic"
            },

        });

        // Add Policy Rule to Firewall Policy
        var firewallPolicyRuleCollectionGroup = new Network.FirewallPolicyRuleCollectionGroup("firewallPolicyRuleCollectionGroup", new()
        {
            Name = "apprules-",
            Priority = 101,
            Id = firewallPolicy.Id,
            FirewallPolicyName = firewallPolicy.Name,
            ResourceGroupName = rg.Name,
            RuleCollections =
            {
                new Network.Inputs.FirewallPolicyFilterRuleCollectionArgs
                {
                    Name = "Allowed_HTTPs",
                    Action = new Network.Inputs.FirewallPolicyFilterRuleCollectionActionArgs
                    {
                        Type = "Allow",
                    },
                    RuleCollectionType = "FirewallPolicyFilterRuleCollection",
                    Rules = new[]
                    {
                        new Network.Inputs.ApplicationRuleArgs
                        {
                            Description = "Allow Github Container Registry",
                            Name = "Allow_GHCR",
                            Protocols = new[]
                            {
                                new Network.Inputs.FirewallPolicyRuleApplicationProtocolArgs
                                {
                                    Port = 443,
                                    ProtocolType = "Https",
                                },
                            },
                            RuleType = "ApplicationRule",
                            SourceAddresses = new[]
                            {
                                "*"
                            },
                            TargetFqdns= new[]
                            {
                                "ghcr.io",
                                "mcr.microsoft.com"
                            }
                        },
                    }
                }
            }
        });


        return Tuple.Create(firewallPolicy,firewallPolicyRuleCollectionGroup);
    }

    public static Network.AzureFirewall CreateFirewall(Resources.ResourceGroup rg, Network.VirtualNetwork vnet)
    {
        // Create a Firewall Policy for assigment later
        var (fwPolicy, ruleCollectionGroup) = CreateFirewallPolicy(rg);


        // Create Firewall Management IP (used by Azure)
        var managementIp = new Network.PublicIPAddress("fw-mgmt-ip", new()
        {
            ResourceGroupName = rg.Name,
            PublicIPAllocationMethod = "Static",
            Sku = new Network.Inputs.PublicIPAddressSkuArgs
            {
                Name = "Standard",
                Tier = "Regional"
            }
        });

        // Create public IP for firewall inbound/outbound traffic
        var publicIp = new Network.PublicIPAddress("fw-ip", new()
        {
            ResourceGroupName = rg.Name,
            PublicIPAllocationMethod = "Static",
            Sku = new Network.Inputs.PublicIPAddressSkuArgs
            {
                Name = "Standard",
                Tier = "Regional"
            }
        });

        // Lookup firewall subnet Id
        Output<string?> fwSubnetId = Network.GetSubnet.Invoke(new Network.GetSubnetInvokeArgs
        {
            ResourceGroupName = rg.Name,
            SubnetName = "AzureFirewallSubnet",
            VirtualNetworkName = vnet.Name,
        }).Apply(x => x.Id);

        // Lookup mgmt  subnet Id
        Output<string?> mgmtfwSubnetId = Network.GetSubnet.Invoke(new Network.GetSubnetInvokeArgs
        {
            ResourceGroupName = rg.Name,
            SubnetName = "AzureFirewallManagementSubnet",
            VirtualNetworkName = vnet.Name
        }).Apply(x => x.Id);

        // Create Firewall
        var firewall = new Network.AzureFirewall("hubfirewall", new()
        {
            ResourceGroupName = rg.Name,
            Sku = new Network.Inputs.AzureFirewallSkuArgs
            {
                Tier = "Basic",
                Name = "AZFW_VNet"
            },
            IpConfigurations = new Network.Inputs.AzureFirewallIPConfigurationArgs
            {
                Name = "fwip-configuration",
                PublicIPAddress = new Network.Inputs.SubResourceArgs
                {
                    Id = publicIp.Id,
                },
                Subnet = new Network.Inputs.SubResourceArgs
                {
                    Id = fwSubnetId
                }
            },
            ManagementIpConfiguration = new Network.Inputs.AzureFirewallIPConfigurationArgs
            {
                Name = "mgmtip-configuration",
                PublicIPAddress = new Network.Inputs.SubResourceArgs
                {
                    Id = managementIp.Id,
                },
                Subnet = new Network.Inputs.SubResourceArgs
                {
                    Id = mgmtfwSubnetId
                }
            },
            FirewallPolicy = new Network.Inputs.SubResourceArgs
            {
                Id = fwPolicy.Id
            }
        }, new CustomResourceOptions
        {
            DependsOn= new InputList<Resource> {vnet, fwPolicy, ruleCollectionGroup}
        });



        return firewall;
    }

    public static Network.RouteTable CreateFirewallRoute(Resources.ResourceGroup rg, Network.AzureFirewall firewall, string rtTableName)
    {
        // Create Route Table
        var routeTable = new Network.RouteTable(rtTableName, new()
        {
            ResourceGroupName = rg.Name,
            RouteTableName = rtTableName
        });

        // Add a route to the firewall
        var route = new Network.Route($"{rtTableName}-firewall-route", new()
        {
            ResourceGroupName = rg.Name,
            AddressPrefix = "0.0.0.0/0",
            NextHopType = "VirtualAppliance",
            RouteTableName = rtTableName,
            RouteName = "firewall-route",
            NextHopIpAddress = firewall.IpConfigurations.First().Apply(x => x.PrivateIPAddress)
        },
        options: new()
        {
            DependsOn = new[] { routeTable }
        });
        return routeTable;
    }

    public static Network.VirtualNetwork CreateVNet(Resources.ResourceGroup spokeRg, VNetSpec spoke, Network.RouteTable routeTable)
    {

        // Define empty subnets list for spoke
        List<Network.Inputs.SubnetArgs> spokeSubnets = new();

        // Define required subnets with route to firewall
        foreach (var subnet in spoke.SubnetsInfo)
        {
            var s = new Network.Inputs.SubnetArgs
            {
                AddressPrefix = subnet.AddressPrefix,
                Name = subnet.Name,
                RouteTable = new Network.Inputs.RouteTableArgs
                {
                    Id = routeTable.Id
                }
            };
            spokeSubnets.Add(s);
        }

        // Create VNET with above subnets
        var virtualNetwork = new Network.VirtualNetwork(spoke.Name, new Network.VirtualNetworkArgs
        {
            ResourceGroupName = spokeRg.Name,
            AddressSpace = new Network.Inputs.AddressSpaceArgs
            {
                AddressPrefixes = spoke.AddressPrefix,
            },
            Location = Location,
            //VirtualNetworkName = ConfigData.Hub.Name,
            Subnets = spokeSubnets
        });

        return virtualNetwork;
    }

    public static void PeerNetworks(string pulumiUrn, Resources.ResourceGroup srcGroup, Network.VirtualNetwork srcNet, Network.VirtualNetwork dstNet)
    {
        var peeringName = Output.Format($"{srcNet.Name}-to-{dstNet.Name}");

        var network = new Network.VirtualNetworkPeering(pulumiUrn, new()
        {
            Name = peeringName,
            VirtualNetworkPeeringName = peeringName,
            ResourceGroupName = srcGroup.Name,
            VirtualNetworkName = srcNet.Name,

            AllowForwardedTraffic = true,
            AllowGatewayTransit = false,
            AllowVirtualNetworkAccess = true,
            RemoteVirtualNetwork = new Network.Inputs.SubResourceArgs
            {
                Id = dstNet.Id
            },
            UseRemoteGateways = false
        });
    }
}

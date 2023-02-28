using Pulumi;
using Pulumi.AzureNative.Resources;
using Pulumi.AzureNative.Storage;
using Pulumi.AzureNative.Storage.Inputs;
using System.Collections.Generic;
using AzureNative = Pulumi.AzureNative;

return await Pulumi.Deployment.RunAsync(() =>
{
    // Reference the resource group forthe network assets
   
    //var rg_network = new GetResourceGroupInvokeArgs { ResourceGroupName = "rg-network-assets" };
    var rg = "rg-network-assets";
    var vnet = "cybercrest-hub-net";
    var subnet = "snet2";
    var location = "eastus";
    var hostname = "securevm";
    var diskname = "securevm_disk1";
    var nic_name = "eth1";

    var subnet_args = AzureNative.Network.GetSubnet.Invoke( new()
    {
        SubnetName= subnet,
        VirtualNetworkName=vnet,
        ResourceGroupName=rg

    });

    var subnet_id = subnet_args.Apply(x => x.Id);

    var nic = new AzureNative.Network.NetworkInterface(nic_name, new()
        {
            EnableAcceleratedNetworking = true,
            IpConfigurations = new[]
            {
                new AzureNative.Network.Inputs.NetworkInterfaceIPConfigurationArgs
                {
                    Name = "ipconfig1",
                    /*PublicIPAddress = new AzureNative.Network.Inputs.PublicIPAddressArgs
                    {
                        Id = "/subscriptions/subid/resourceGroups/rg1/providers/Microsoft.Network/publicIPAddresses/test-ip",
                    },*/
                    Subnet = new AzureNative.Network.Inputs.SubnetArgs
                    {
                        Id = subnet_id,
                    },
                },
            },
            Location = location,
            NetworkInterfaceName = nic_name,
            ResourceGroupName = rg,
        });



        var vm = new AzureNative.Compute.VirtualMachine(hostname, new()
        {
            HardwareProfile = new AzureNative.Compute.Inputs.HardwareProfileArgs
            {
                VmSize = "Standard_D1_v2",
            },
            Location = location,
            NetworkProfile = new AzureNative.Compute.Inputs.NetworkProfileArgs
            {
                NetworkInterfaces = new[]
                {
                    new AzureNative.Compute.Inputs.NetworkInterfaceReferenceArgs
                    {
                        Id = nic.Id,
                        Primary = true,
                    },
                },
            },
            OsProfile = new AzureNative.Compute.Inputs.OSProfileArgs
            {
                AdminPassword = "Xmiles04$%",
                AdminUsername = "egale",
                ComputerName = hostname,
                WindowsConfiguration = new AzureNative.Compute.Inputs.WindowsConfigurationArgs
                {
                    EnableAutomaticUpdates = true,
                    PatchSettings = new AzureNative.Compute.Inputs.PatchSettingsArgs
                    {
                        PatchMode = "AutomaticByOS",
                    },
                    ProvisionVMAgent = true,
                },
            },
            ResourceGroupName = rg,
            StorageProfile = new AzureNative.Compute.Inputs.StorageProfileArgs
            {
                ImageReference = new AzureNative.Compute.Inputs.ImageReferenceArgs
                {
                    Offer = "WindowsServer",
                    Publisher = "MicrosoftWindowsServer",
                    Sku = "2016-Datacenter",
                    Version = "latest",
                },
                OsDisk = new AzureNative.Compute.Inputs.OSDiskArgs
                {
                    Caching = AzureNative.Compute.CachingTypes.ReadWrite,
                    CreateOption = "FromImage",                    
                    ManagedDisk = new AzureNative.Compute.Inputs.ManagedDiskParametersArgs
                    {
                        StorageAccountType = "Standard_LRS",
                    },
                    Name = diskname,
                    DeleteOption = "Delete"
                },
            },
            VmName = hostname,
        });


    var vm_ext_args = new AzureNative.Compute.VirtualMachineExtensionArgs
    {
        ResourceGroupName = rg,
        VmName = hostname,
        Type = "IaaSAntimalware",
        Publisher = "Microsoft.Azure.Security",
        TypeHandlerVersion = "1.3"
    };

    var vm_ext_av = new AzureNative.Compute.VirtualMachineExtension ("IaaSAntimalware", vm_ext_args, new CustomResourceOptions { DependsOn = {vm}});

});
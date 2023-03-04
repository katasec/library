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
    var rg1 = "rg-security-assets";
    var vnet = "cybercrest-hub-net";
    var kv_name = "egalvault";
    var subnet = "snet2";
    var location = "eastus";
    var hostname = "securevm";
    var diskname = "securevm_disk1";
    var nic_name = "eth1";
    //var disk-key= "disk-key";

    var subnet_args = AzureNative.Network.GetSubnet.Invoke( new()
    {
        SubnetName= subnet,
        VirtualNetworkName=vnet,
        ResourceGroupName=rg

    });

    var kvault_args = AzureNative.KeyVault.GetVault.Invoke ( new()
    {
        ResourceGroupName=rg1,
        VaultName =kv_name,

    });

    var keyvault_id = kvault_args.Apply(y => y.Id);
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

    var diskEncryptionSet = new AzureNative.Compute.DiskEncryptionSet("diskEncryptionSet", new()
    {
        ActiveKey = new AzureNative.Compute.Inputs.KeyForDiskEncryptionSetArgs
        {
            KeyUrl = "https://egalvault.vault.azure.net/keys/disk-key/53d6a218fa9141d2aafb7206b9a6c090",
            SourceVault = new AzureNative.Compute.Inputs.SourceVaultArgs
            {
                Id = keyvault_id,
            },
            
        },
        DiskEncryptionSetName = "myDiskEncryptionSet",
        EncryptionType = "EncryptionAtRestWithCustomerKey",
        Identity = new AzureNative.Compute.Inputs.EncryptionSetIdentityArgs
        {
            Type = "SystemAssigned",
        },
        Location = location,
        ResourceGroupName = rg1,
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
                        AssessmentMode = "AutomaticByPlatform",

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
                        
                        DiskEncryptionSet = new AzureNative.Compute.Inputs.DiskEncryptionSetParametersArgs
                        {
                            Id = diskEncryptionSet.Id,
                        },

                        StorageAccountType = "Standard_LRS",
                    },
                    Name = diskname,
                    DeleteOption = "Delete",
                },
            },
            VmName = hostname,
        });


    //This below is one way of creating an extension for a virtual machine by passing the DependsOn argument to the CustomResourceOptions and the vm resource
    // to create a wait on the Virtual machine to be created before the extension can be created in the machine.
    var vm_ext_args_av = new AzureNative.Compute.VirtualMachineExtensionArgs
    {
        ResourceGroupName = rg,
        VmName = hostname,
        Type = "IaaSAntimalware",
        Publisher = "Microsoft.Azure.Security",
        TypeHandlerVersion = "1.3"
    };

    var vm_ext_args_am = new AzureNative.Compute.VirtualMachineExtensionArgs
    {
        ResourceGroupName = rg,
        VmName = hostname,
        Type = "AzureMonitorWindowsAgent",
        Publisher = "Microsoft.Azure.Monitor",  
        TypeHandlerVersion = "1.12",
        EnableAutomaticUpgrade = true
    };

    var vm_ext_av = new AzureNative.Compute.VirtualMachineExtension ("IaaSAntimalware", vm_ext_args_av, new CustomResourceOptions { DependsOn = {vm}});
    var vm_ext_am = new AzureNative.Compute.VirtualMachineExtension ("AzureMonitorWindowsAgent", vm_ext_args_am, new CustomResourceOptions { DependsOn = {vm}});

});
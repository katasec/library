using System;
using System.Collections.Generic;
using Pulumi;
using ArkServer.Entities.Azure;
using Resources = Pulumi.AzureNative.Resources;

namespace AzureCloudspaceHandler;

public static class Handler
{

    static AzureCloudspace GetConfig()
    {
        var cfg = new Config();
        var configData = cfg.GetObject<AzureCloudspace>("arkdata");
        if (configData == null)
        {
            Console.WriteLine("Config data from Ark is empty, got null");
            Environment.Exit(0);
        }
        else
        {
            Console.WriteLine($"Hub name is: {configData.Hub.Name}");
        }

        return configData;
    }

    public static Dictionary<string, object?> Start()
    {
        var exports = new Dictionary<string, object?>();

        var cfg = GetConfig();

        var resourceGroup = new Resources.ResourceGroup(cfg.Name);


        exports.Add("Hello", "World");
        exports.Add("Checking", "Update");
        return exports;
    }
}



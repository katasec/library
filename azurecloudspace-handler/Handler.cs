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

        return configData;
    }

    /// <summary>
    /// Start creates the azure cloudspace which is essentially hub + one or more spokes
    /// </summary>
    /// <returns></returns>
    public static Dictionary<string, object?> Start()
    {
        var exports = new Dictionary<string, object?>();

        // Get config data with cloudspace details.
        var cfg = GetConfig();

        // Create hub resource group
        var hubRg = new Resources.ResourceGroup("rg-hub-", new()
        {
            Tags= { { "ark:managed", "true" }},
        });


        exports.Add("Hello", "World");
        exports.Add("Checking", "Update");
        return exports;
    }
}



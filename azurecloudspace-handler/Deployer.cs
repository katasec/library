using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulumi;
using ArkServer.Entities.Azure;

namespace AzureCloudspaceHandler;

public static class Deployer
{
    public static Dictionary<string, object?> Start()
    {
        var exports = new Dictionary<string, object?>();

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

        exports.Add("Hello", "World");
        return exports;
    }
}



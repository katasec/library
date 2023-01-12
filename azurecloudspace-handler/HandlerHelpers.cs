using ArkServer.Entities.Azure;
using System;
using Pulumi;

namespace AzureCloudspaceHandler;

public static partial class Handler
{
    public static readonly AzureCloudspace? ConfigData = new();

    /// <summary>
    /// The constructor reads pulumi config injected by the worker into this handler.
    /// </summary>
    static Handler()
    {
        var cfg = new Config();
        ConfigData = cfg.GetObject<AzureCloudspace>("arkdata");
        if (ConfigData == null)
        {
            Console.WriteLine("Config data from Ark is empty, got null");
            Environment.Exit(0);
        }
    }
}

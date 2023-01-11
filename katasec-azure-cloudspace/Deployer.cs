using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Resources = Pulumi.AzureNative.Resources;
using ArkServer.Entities.Azure;

namespace AzureCloudspace;

public static class Deployer
{
    public static Dictionary<string, object?> Start()
    {
        var config = new Pulumi.Config();

        var req = config.GetObject<ArkServer.Entities.Azure.AzureCloudspace>("pulumiconfig");
        var exports = new Dictionary<string, object?>();

        exports.Add("hub",req.Hub.Name);
        foreach(var env in req.Env)
        {
            exports.Add("env" + env.Name,req.Env[0].Name);
        }

        int i=0;
        foreach(var subnets in req.Hub.SubnetsInfo)
        {
            exports.Add($"subnet_{i++}",subnets.AddressPrefix);
        }

        return exports;
    }
}

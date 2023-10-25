// using System;
// using System.IO;
// using System.Text.Json;
// using AzureCloudspaceHandler.Models;
//
//
// var json = File.ReadAllText("./test.json");
// var stuff = JsonSerializer.Deserialize<AzureCloudspace>(json);

return await Pulumi.Deployment.RunAsync(() =>
{
    return AzureCloudspaceHandler.Handler.Start();
});
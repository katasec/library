
return await Pulumi.Deployment.RunAsync(() =>
{
    return AzureCloudspaceHandler.Deployer.Start();
});
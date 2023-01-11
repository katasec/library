
return await Pulumi.Deployment.RunAsync(() =>
{
    return AzureCloudspaceHandler.Handler.Start();
});
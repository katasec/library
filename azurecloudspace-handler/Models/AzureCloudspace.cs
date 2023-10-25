using System.Collections.Generic;
using System.Text.Json.Serialization;
using YamlDotNet.Serialization;

namespace AzureCloudspaceHandler.Models;


/// <summary>
/// An Azure Cloudspace has one hub and one or more 'Environments' or VNETs
/// </summary>

public class AzureCloudspace 
{
    /// <summary>
    /// Name of cloudspace. Defaulting to 'default'
    /// </summary>
    [JsonPropertyName("Name")]
    [YamlMember(Alias = "Name")]
    public string Name { get; set; } = "default";

    /// <summary>
    /// One hub per cloudspace
    /// </summary>
    [JsonPropertyName("Hub")]
    [YamlMember(Alias = "Hub")]
    public VNetSpec Hub { get; set; }

    /// <summary>
    /// One or more Environments or VNETs per cloudspace
    /// </summary>
    [JsonPropertyName("Spokes")]
    [YamlMember(Alias = "Spokes")]
    public HashSet<VNetSpec> Spokes { get; set; } = new HashSet<VNetSpec>();

    /// <summary>
    /// Creation status
    /// </summary>
    [JsonPropertyName("Status")]
    [YamlMember(Alias = "Status")]
    public string? Status { get; set; }
}
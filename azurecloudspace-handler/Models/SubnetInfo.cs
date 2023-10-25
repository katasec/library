
using System.Collections.Generic;
using System.Text.Json.Serialization;
using YamlDotNet.Serialization;

namespace AzureCloudspaceHandler.Models
{
    public class SubnetInfo
    {
        [JsonPropertyName("Name")]
        [YamlMember(Alias = "Name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("AddressPrefix")]
        [YamlMember(Alias = "AddressPrefix")]
        public string AddressPrefix { get; set; } = "";

        [JsonPropertyName("Description")]
        [YamlMember(Alias = "Description")]
        public string Description { get; set; } = "";

        [JsonPropertyName("Tags")]
        [YamlMember(Alias = "Tags")]
        public List<KeyValuePair<string, string>> Tags = new();
    }
}
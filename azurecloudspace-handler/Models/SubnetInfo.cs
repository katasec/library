
using System.Collections.Generic;
using System.Text.Json.Serialization;
using YamlDotNet.Serialization;

namespace AzureCloudspaceHandler.Models
{
    public class SubnetInfo
    {
        [JsonPropertyName("name")]
        [YamlMember(Alias = "Name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("addressprefix")]
        [YamlMember(Alias = "AddressPrefix")]
        public string AddressPrefix { get; set; } = "";

        [JsonPropertyName("description")]
        [YamlMember(Alias = "Description")]
        public string Description { get; set; } = "";

        [JsonPropertyName("tags")]
        [YamlMember(Alias = "Tags")]
        public List<KeyValuePair<string, string>> Tags = new();
    }
}
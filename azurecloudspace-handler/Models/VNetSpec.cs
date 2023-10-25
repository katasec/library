using System.Collections.Generic;
using System.Text.Json.Serialization;
using YamlDotNet.Serialization;

namespace AzureCloudspaceHandler.Models
{
    public class VNetSpec
    {
        [JsonPropertyName("Name")]
        [YamlMember(Alias = "Name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("AddressPrefix")]
        [YamlMember(Alias = "AddressPrefix")]
        public string AddressPrefix { get; set; } = "";

        [JsonPropertyName("SubnetsInfo")]
        [YamlMember(Alias = "SubnetsInfo")]
        public IEnumerable<SubnetInfo> SubnetsInfo { get; set; } = new List<SubnetInfo>();
    }
}
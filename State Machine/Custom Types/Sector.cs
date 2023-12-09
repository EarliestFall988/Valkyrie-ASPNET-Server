using System.Text.Json.Serialization;

namespace ValkyrieFSMCore
{
    /// <summary>
    /// A class representing a sector
    /// </summary>
    public class Sector
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = "";
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";
        [JsonPropertyName("departmentCode")]
        public string DepartmentCode { get; set; } = "";

        [JsonPropertyName("Projects")]
        public List<Project> Projects { get; set; } = new List<Project>();
    }
}



using System.Text.Json.Serialization;

namespace ValkyrieFSMCore
{
    /// <summary>
    /// The variable type
    /// </summary>
    public class Project
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = "";

        /// <summary>
        /// The name of the project
        /// </summary>
        [JsonPropertyName ("name")]
        public string Name { get; set; } = "";

        /// <summary>
        /// the description of the project
        /// </summary>
        [JsonPropertyName("startDate")]
        public string StartDate { get; set; } = "";

        /// <summary>
        /// the id of the sector that the project is associated with
        /// </summary>
        [JsonPropertyName("endDate")]
        public string EndDate { get; set; } = "";
    }
}



using System.Text.Json.Serialization;

namespace ValkyrieFSMCore
{
    /// <summary>
    /// The class representing a project
    /// </summary>
    public class Project
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = "";

        /// <summary>
        /// The name of the project
        /// </summary>
        [JsonPropertyName("name")]
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

        [JsonPropertyName("TotalManHours")]
        public float TotalManHours { get; set; } = 0;

        [JsonPropertyName("jobNumber")]
        public string JobNumber { get; set; } = "";


        [JsonPropertyName("materialCost")]
        public float MaterialCost { get; set; } = 0;

        [JsonPropertyName("subContractorCost")]
        public float SubContractorCost { get; set; } = 0;

        [JsonPropertyName("laborCost")]
        public float LaborCost { get; set; } = 0;

        [JsonPropertyName("otherCost")]
        public float OtherCost { get; set; } = 0;

        [JsonPropertyName("state")]
        public string State { get; set; } = "";

        [JsonPropertyName("status")]
        public string Status { get; set; } = "";
    }
}

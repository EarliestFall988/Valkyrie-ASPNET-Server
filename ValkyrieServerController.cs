using Avalon;

using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Valkyrie_Server
{
    /// <summary>
    /// Handles sending and receiving data from the Valkyrie server
    /// </summary>
    public class ValkyrieServerController
    {

        public static readonly Uri productionBaseUri = new Uri("https://valkyrie-nu.vercel.app/api/v1/getdata/", UriKind.Absolute);
        public static readonly Uri testBaseUri = new Uri("http://localhost:3000/api/v1/getdata/", UriKind.Absolute);

        public static readonly Uri testURI = new Uri("http://localhost:5000/api/sm/guess", UriKind.Absolute);

        /// <summary>
        /// the base URI for the Valkyrie server
        /// </summary>
        private Uri selectedURI = testURI;

        /// <summary>
        ///  The API key for the Valkyrie server
        /// </summary>
        public string ValkyrieAPIKey { get; init; } = "";


        /// <summary>
        /// Try get the instructions from the Valkyrie server
        /// </summary>
        /// <param name="instructionId">the instruction ID</param>
        /// <returns>the result of the operation and the content of the response</returns>
        public async Task<(bool result, string content)> TryGetInstructions(string instructionId)
        {

            //Debug.WriteLine("\n\n\t" + instructionId + "\n");

            using HttpClient client = new();

            var data = JsonSerializer.Serialize(new Content()
            {
                Key = ValkyrieAPIKey,
                InstructionId = instructionId
            });

            HttpContent content = new StringContent(data);


            //Debug.WriteLine("\n\n\t" + selectedURI + "\n");
            //Debug.WriteLine("\n\n\t" + data + "\n");

            var response = await client.PostAsync(selectedURI, content);
            client.DefaultRequestHeaders.Add("x-api-key", ValkyrieAPIKey);

            using StreamReader reader = new StreamReader(response.Content.ReadAsStream());
            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            string responseString = await reader.ReadToEndAsync();

            //Debug.WriteLine("\n\n\n\t" + response.StatusCode + "\n");

            if (!response.IsSuccessStatusCode)
            {
                return (false, "Error: " + response.StatusCode + "\n" + responseString);
            }


            if (string.IsNullOrEmpty(responseString))
            {
                return (false, "Error: Empty response from server");
            }
            return (true, responseString);
        }

        /// <summary>
        /// Update the function definitions for the given instruction ID
        /// </summary>
        /// <param name="instructionId"> the instruction ID</param>
        /// <returns> the result of the update</returns>
        public async Task<(string response, string statusCode)> UpdateInstructionFunctionDefinitions(string instructionId)
        {

            if (string.IsNullOrEmpty(instructionId))
            {
                return ("Error: No instruction ID provided", "0");
            }

            using HttpClient client = new();

            var functionJSON = DiscoverFunctionsHandler.GetFunctionDefinitionsJSON();

            HttpContent content = new StringContent(functionJSON);

            var response = await client.PostAsync(selectedURI, content);
            client.DefaultRequestHeaders.Add("x-api-key", ValkyrieAPIKey);
            client.DefaultRequestHeaders.Add("x-instruction-id", instructionId);

            using StreamReader reader = new StreamReader(response.Content.ReadAsStream());

            string responseString = await reader.ReadToEndAsync();

            if (!response.IsSuccessStatusCode)
            {
                return ("Error: " + response.StatusCode + "\n" + responseString, response.StatusCode.ToString());
            }

            if (string.IsNullOrEmpty(responseString))
            {
                return ("Error: Empty response from server", "0");
            }

            return (responseString, response.StatusCode.ToString());
        }
    }

    /// <summary>
    /// Typical content requested by the valkyrie server
    /// </summary>
    public struct Content
    {
        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonPropertyName("Id")]
        public string InstructionId { get; set; }
    }
}

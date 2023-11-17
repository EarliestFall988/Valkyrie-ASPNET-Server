using Avalon;

using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Valkyrie_Server
{
    public class ValkyrieServerController
    {

        public static readonly Uri productionBaseUri = new Uri("https://valkyrie-nu.vercel.app/api/v1/getdata/", UriKind.Absolute);
        public static readonly Uri testBaseUri = new Uri("http://localhost:3000/api/v1/getdata/", UriKind.Absolute);

        public static readonly Uri testURI = new Uri("http://localhost:5000/api/sm/guess", UriKind.Absolute);

        public string ValkyrieAPIKey { get; init; } = "";

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

            var selectedURI = testURI;

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
    }

    public struct Content
    {
        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonPropertyName("Id")]
        public string InstructionId { get; set; }
    }
}

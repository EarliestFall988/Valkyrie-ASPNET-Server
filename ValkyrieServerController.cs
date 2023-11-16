using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Valkyrie_Server
{
    public class ValkyrieServerController
    {

        public static readonly Uri productionBaseUri = new Uri("https://valkyrie-nu.vercel.app/api/v1/getdata/", UriKind.Absolute);
        public static readonly Uri testBaseUri = new Uri("http://localhost:3000/api/v1/getdata/", UriKind.Absolute);
        public string ValkyrieAPIKey { get; init; } = "";

        public async Task<string> TryGetInstructions(string instructionId)
        {
            using HttpClient client = new();

            var data = JsonSerializer.Serialize(new Content() { 
                Key = ValkyrieAPIKey,
                InstructionId = instructionId
            });

            HttpContent content = new StringContent(data);


            Debug.WriteLine("\n\n\t" + productionBaseUri + "\n");
            Debug.WriteLine("\n\n\t" + data + "\n");

            var response = await client.PostAsync(productionBaseUri, content);
            client.DefaultRequestHeaders.Add("x-api-key", ValkyrieAPIKey);
           
            var responseString = await response.Content.ReadAsStringAsync();

            Debug.WriteLine("\n\n\t" + response.StatusCode + "\n");

            return responseString;
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

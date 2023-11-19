using Avalon;

using System.Text.Json;


namespace Valkyrie_Server
{
    public class DiscoverFunctionsHandler
    {

        /// <summary>
        /// Get the functions from the libary as a JSON string
        /// </summary>
        /// <returns>returns the json string of the functions</returns>
        public static string GetFunctionDefinitionsJSON() => JsonSerializer.Serialize(
            new FunctionLibrary().ImportedFunctions
            .Select(x => new FunctionListItem(x.Key, x.Value.Description, x.Value.ExpectedParameters
                    .Select(x => x.Value)
                .ToArray())));
    }
}

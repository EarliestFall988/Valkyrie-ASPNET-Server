using ValkyrieFSMCore;

using System.Text.Json;
using System.Collections;
using System.Diagnostics;

namespace Valkyrie_Server
{
    public class GetSyncDataHandler
    {

        /// <summary>
        /// Get the functions from the libary as a JSON string
        /// </summary>
        /// <returns>returns the json string of the functions</returns>
        public static IEnumerable<FunctionListItem> GetFunctionDefinitions() =>
            new FunctionLibrary().ImportedFunctions.Values.Select(x => new FunctionListItem()
            {
                Name = x.Name,
                Description = x.Description,
                Parameters = x.ExpectedParametersList
            });

        public static IEnumerable<VariableTypeDef> GetVariableTypes() => new VariableFactory().GetAllPossibleVarableTypes.Select(x => new VariableTypeDef()
        {
            Description = x.description,
            Name = x.key
        });

        public static string GetSyncData()
        {
            var functions = GetFunctionDefinitions();
            var variableTypes = GetVariableTypes();

            foreach (var x in functions)
            {
                foreach (var y in x.Parameters)
                {
                    Debug.WriteLine(y.Name + " " + y.Type + " " + y.IO + " " + y.Description);
                }
            }

            var syncData = new SyncRequestBody(functions.ToArray(), variableTypes.ToArray());

            return JsonSerializer.Serialize(syncData);
        }
    }

    public class VariableTypeDef
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
    }

    public class FunctionListItem
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";

        public ExpectedParameter[] Parameters { get; set; } = new ExpectedParameter[0];
    }

    internal record SyncRequestBody(FunctionListItem[] Functions, VariableTypeDef[] CustomTypes);
}



namespace ValkyrieFSMCore
{
    /// <summary>
    /// The custom variable library
    /// </summary>
    public class CustomVariableLibrary
    {
        /// <summary>
        /// The dictionary of custom variables created
        /// </summary>
        public Dictionary<string, IVariableSignature> CustomVariables { get; set; } = new Dictionary<string, IVariableSignature>();

        public CustomVariableLibrary()
        {
            // Add your custom variables here
        }
    }
}

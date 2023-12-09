namespace ValkyrieFSMCore
{
    /// <summary>
    /// Handles the creation of variables with the correct type
    /// </summary>
    public class VariableFactory
    {
        /// <summary>
        /// The variable constructors
        /// </summary>
        private Dictionary<string, (string description, Func<string, VariableIO, IVariableSignature> function)> VariableConstructors { get; set; } = new();

        /// <summary>
        /// Get the possible variable types
        /// </summary>
        public IEnumerable<(string key, string description)> GetAllPossibleVarableTypes =>
            VariableConstructors.Select(x => (x.Key, x.Value.description));


        /// <summary>
        ///  Default constructor
        /// </summary>
        public VariableFactory() => Register();

        /// <summary>
        /// Register the variable types
        /// </summary>
        void Register()
        {
            VariableConstructors.Add("projects", ("this variable maps to a list of projects", (key, io) =>
            {
                var x = VariableDefinition<List<Project>>.CreateCustom(key, "projects", new List<Project>(), io);
                return x;
            }
            ));
        }

        /// <summary>
        ///  Try to get the type
        /// </summary>
        /// <typeparam name="T">the type</typeparam>
        /// <param name="key">the name of the variable</param>
        /// <param name="type">the type of the variable</param>
        /// <param name="result">the resulting IVariableSignature</param>
        /// <param name="io">the io</param>
        /// <returns>returns true if the operation was successful, false if not</returns>
        public bool TryConstructVariable<T>(string key, string type, out T? result, VariableIO io = VariableIO.In) where T : class, IVariableSignature
        {
            if (!VariableConstructors.ContainsKey(type)) //TODO: add json serialization so that the default values can be constructed as well
            {
                result = null;
                return false;
            }

            var func = VariableConstructors[type].function;

            result = func(key, io) as T;
            return true;
        }

        /// <summary>
        /// Does the factory contain the type?
        /// </summary>
        /// <param name="type"> the type</param>
        /// <returns> returns true if the factory contains the type, false if not</returns>
        public bool ContainsType(string type) => VariableConstructors.ContainsKey(type);

    }
}

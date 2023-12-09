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
            VariableConstructors.Add("projects", ("this type maps to a list of projects", (key, io) =>
            {
                var x = VariableDefinition<List<Project>>.CreateCustom(key, "projects", new List<Project>(), io);
                return x;
            }
            ));

            VariableConstructors.Add("project", ("this is a project type", (key, io) =>
            {
                var x = VariableDefinition<Project>.CreateCustom(key, "project", new Project(), io);
                return x;
            }
            ));

            VariableConstructors.Add("string", ("this is a text type that can store words, special characters. and numbers (not the value of the number).", (key, io) =>
            {
                var x = VariableDefinition<string>.CreateCustom(key, "string", "", io);
                return x;
            }
            ));

            VariableConstructors.Add("decimal", ("the type represents decimal values (ex. 1.0, 2.34, -32.355 ...)", (key, io) =>
            {
                var x = VariableDefinition<float>.CreateCustom(key, "decimal", 0, io); // 👈 I know this is not an actual decimal type value. human readability is what I am after...
                return x;
            }
            ));

            VariableConstructors.Add("integer", ("this type represents whole number values (ex. 1, 5 ,-3 ...)", (key, io) =>
            {
                var x = VariableDefinition<int>.CreateCustom(key, "integer", 0, io);
                return x;
            }
            ));

            VariableConstructors.Add("boolean", ("this type is either true or false", (key, io) =>
            {
                var x = VariableDefinition<bool>.CreateCustom(key, "boolean", false, io);
                return x;
            }
            ));

            VariableConstructors.Add("any", ("this type can be anything", (key, io) =>
            {
                var x = VariableDefinition<object>.CreateCustom(key, "any", new object(), io);
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

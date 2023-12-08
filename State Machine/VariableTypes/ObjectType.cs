using ValkyrieFSMCore;

namespace ValkyrieFSMCore
{
    /// <summary>
    /// A class representing an object type.
    /// </summary>
    public class ObjectType : IVariableSignature
    {
        /// <summary>
        /// The variables of the object.
        /// </summary>
        Dictionary<string, IVariableSignature> _variables { get; set; } = new Dictionary<string, IVariableSignature>();

        /// <summary>
        /// Enumerate through the variables.
        /// </summary>
        public IEnumerable<IVariableSignature> Variables => _variables.Values;

        public string Type { get; set; } = StateMachineVariableType.Object.ToString();

        public VariableIO IO { get; set; }

        public string Key { get; set; }

        public string Description { get; set; }

        public ObjectType(string key, string type, VariableIO objIo, string description)
        {
            Key = key;
            Type = type;
            IO = objIo;
            Description = description;
        }

        public bool TryGetItem<T>(string name, out T? res) where T : IVariableSignature
        {
            if (_variables.TryGetValue(name, out IVariableSignature? item))
            {
                if (item is T)
                {
                    res = (T)item;
                    return true;
                }
            }

            res = default(T);
            return false;
        }
    }
}
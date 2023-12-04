using Avalon;

namespace Avalon
{
    /// <summary>
    /// A class representing an object type.
    /// </summary>
    public class ObjectType : IVariableType
    {
        /// <summary>
        /// The variables of the object.
        /// </summary>
        Dictionary<string, IVariableType> _variables { get; set; } = new Dictionary<string, IVariableType>();

        /// <summary>
        /// Enumerate through the variables.
        /// </summary>
        public IEnumerable<IVariableType> Variables => _variables.Values;

        public StateMachineVariableType Type { get; set; } = StateMachineVariableType.Object;

        public VariableIO IO { get; set; }

        public string Key { get; set; }

        public ObjectType(string key, StateMachineVariableType type, VariableIO objIo)
        {
            Key = key;
            Type = type;
            IO = objIo;
        }

        public bool TryGetItem<T>(string name, out T? res) where T : IVariableType
        {
            if (_variables.TryGetValue(name, out IVariableType? item))
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

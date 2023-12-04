
namespace Avalon
{
    /// <summary>
    ///  Set a mutable variable definition.
    /// </summary>
    /// <typeparam name="T">the type of the variable</typeparam>
    public class VariableDefinition<T> : IVariableType
    {
        public T Value { get; set; }

        public StateMachineVariableType Type { get; set; }

        public VariableIO IO { get; set; }

        public string Key { get; set; }

        /// <summary>
        /// The default constructor.
        /// </summary>
        /// <param name="key"> The key of the variable. </param>
        /// <param name="value"> The value of the variable. </param>
        /// <param name="type"> The type of the variable. </param>
        /// <param name="outValue"> Whether or not the variable is an out value. </param>
        public VariableDefinition(string key, T value, StateMachineVariableType type, VariableIO variableIo = VariableIO.In)
        {
            Key = key;
            Value = value;
            Type = type;
            IO = variableIo;
        }

        /// <summary>
        ///  Check if two variable definitions are the same type.
        /// </summary>
        /// <param name="a"> The first variable definition. </param>
        /// <param name="b"> The second variable definition. </param>
        /// <returns></returns>
        public static bool IsSameType(VariableDefinition<T> a, VariableDefinition<T> b)
        {
            return a.Type == b.Type;
        }

        #region create primitive types

        public static VariableDefinition<string> CreateString(string key, string value, VariableIO varIo = VariableIO.In)
        {
            return new VariableDefinition<string>(key, value, StateMachineVariableType.Text, varIo);
        }

        public static VariableDefinition<int> CreateInt(string key, int value, VariableIO varIo = VariableIO.In)
        {
            return new VariableDefinition<int>(key, value, StateMachineVariableType.Integer, varIo);
        }

        public static VariableDefinition<decimal> CreateDecimal(string key, decimal value, VariableIO varIo = VariableIO.In)
        {
            return new VariableDefinition<decimal>(key, value, StateMachineVariableType.Decimal, varIo);
        }

        public static VariableDefinition<bool> CreateBool(string key, bool value, VariableIO varIo = VariableIO.In)
        {
            return new VariableDefinition<bool>(key, value, StateMachineVariableType.YesNo, varIo);
        }

        #endregion


        #region create list types

        public static VariableDefinition<List<string>> CreateListString(string key, List<string> value, VariableIO varIo = VariableIO.In)
        {
            return new VariableDefinition<List<string>>(key, value, StateMachineVariableType.Text, varIo);
        }

        public static VariableDefinition<List<int>> CreateListInt(string key, List<int> value, VariableIO varIo = VariableIO.In)
        {
            return new VariableDefinition<List<int>>(key, value, StateMachineVariableType.Text, varIo);
        }

        public static VariableDefinition<List<decimal>> CreateListString(string key, List<decimal> value, VariableIO varIo = VariableIO.In)
        {
            return new VariableDefinition<List<decimal>>(key, value, StateMachineVariableType.Text, varIo);
        }

        public static VariableDefinition<List<bool>> CreateListYesNo(string key, List<bool> value, VariableIO varIo = VariableIO.In)
        {
            return new VariableDefinition<List<bool>>(key, value, StateMachineVariableType.Text, varIo);
        }

        #endregion
    }
}

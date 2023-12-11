
using System.Text.Json;

namespace ValkyrieFSMCore
{
    /// <summary>
    ///  Set a mutable variable definition.
    /// </summary>
    /// <typeparam name="T">the type of the variable</typeparam>
    public class VariableDefinition<T> : IVariableSignature
    {
        public T Value { get; set; }

        public string Type { get; set; }

        public VariableIO IO { get; set; }

        public string Key { get; set; }

        /// <summary>
        /// The description of the variable type
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The default constructor.
        /// </summary>
        /// <param name="key"> The key of the variable. </param>
        /// <param name="value"> The value of the variable. </param>
        /// <param name="type"> The type of the variable. </param>
        /// <param name="outValue"> Whether or not the variable is an out value. </param>
        public VariableDefinition(string key, T value, string type, VariableIO variableIo = VariableIO.In, string description = "")
        {
            Key = key;
            Value = value;
            Type = type;
            IO = variableIo;
            Description = description;
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
            return new VariableDefinition<string>(key, value, StateMachineVariableType.Text.ToString(), varIo);
        }

        public static VariableDefinition<int> CreateInt(string key, int value, VariableIO varIo = VariableIO.In)
        {
            return new VariableDefinition<int>(key, value, StateMachineVariableType.Integer.ToString(), varIo);
        }

        public static VariableDefinition<decimal> CreateDecimal(string key, decimal value, VariableIO varIo = VariableIO.In)
        {
            return new VariableDefinition<decimal>(key, value, StateMachineVariableType.Decimal.ToString(), varIo);
        }

        public static VariableDefinition<bool> CreateBool(string key, bool value, VariableIO varIo = VariableIO.In)
        {
            return new VariableDefinition<bool>(key, value, StateMachineVariableType.YesNo.ToString(), varIo);
        }

        public static VariableDefinition<T> CreateCustom(string key, string type, T value, VariableIO varIo = VariableIO.In)
        {
            return new VariableDefinition<T>(key, value, type, varIo);
        }

        #endregion


        public string ToJSON()
        {
            return JsonSerializer.Serialize(Value);
        }
    }
}

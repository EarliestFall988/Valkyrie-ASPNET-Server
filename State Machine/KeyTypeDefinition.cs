using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Avalon
{
    /// <summary>
    /// The Key Type Definition handles the type of the key, name, and the value of the key.
    /// </summary>
    public class KeyTypeDefinition
    {
        /// <summary>
        /// The name of the key.
        /// </summary>
        /// <value></value>
        public string Key { get; set; } = "";

        /// <summary>
        /// The type of the key.
        /// </summary>
        /// <value></value>
        public StateMachineVariableType Type { get; set; } = StateMachineVariableType.Text;

        /// <summary>
        /// The value of the key.
        /// </summary>
        /// <value></value>
        public string Value { get; private set; } = "";


        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="type"></param>
        /// <param name="value"></param>
        public KeyTypeDefinition(string key, StateMachineVariableType type, string value)
        {
            Key = key;
            Type = type;
            Value = value;
        }

        /// <summary>
        /// Gets the value of the key as a string.
        /// </summary>
        /// <returns></returns>
        public string GetText()
        {
            return Value;
        }

        /// <summary>
        /// Gets the value of the key as an integer.
        /// </summary>
        /// <returns></returns>
        public Int32 GetInt()
        {
            Console.WriteLine("value: " + Value);
            return Int32.Parse(Value);
        }

        /// <summary>
        /// Gets the value of the key as a decimal.
        /// </summary>
        /// <returns></returns>
        public Decimal GetDecimal()
        {
            return Decimal.Parse(Value);
        }

        /// <summary>
        /// Gets the value of the key as a boolean.
        /// </summary>
        /// <returns></returns>
        public bool GetYesNo()
        {
            if (Value.ToLower() == "yes")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void SetValue(string value)
        {
            this.Value = value;
        }
    }

    /// <summary>
    /// The type of the key.
    /// </summary>
    public enum StateMachineVariableType
    {
        Text, //text
        Decimal, //decimal
        Integer, //int
        YesNo, //boolean
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;



namespace ValkyrieFSMCore
{
    /// <summary>
    /// The Key Type Definition handles the type of the key, name, and the value of the key.
    /// </summary>
    public class KeyTypeDefinition : IVariableSignature
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
        public string Type { get; set; } = StateMachineVariableType.Text.ToString();

        /// <summary>
        /// The value of the key.
        /// </summary>
        /// <value></value>
        public string Value { get; private set; } = "";

        /// <summary>
        /// Is the value an out value?
        /// </summary>
        public VariableIO IO { get; set; } = VariableIO.In;

        /// <summary>
        /// the description of the key type definition
        /// </summary>
        public string Description { get; set; } = "";

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="type"></param>
        /// <param name="value"></param>
        public KeyTypeDefinition(string key, string type, string value, string description = "")
        {
            Key = key;
            Type = type;
            Value = value;
            Description = description;
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

        public void SetValue(decimal value)
        {
            this.Value = value.ToString();
        }


        public void SetValue(bool value)
        {
            if (value)
            {
                this.Value = "yes";
            }
            else
            {
                this.Value = "no";
            }
        }

        public void SetValue(int value)
        {
            this.Value = value.ToString();
        }

        public void SetValue(double value)
        {
            this.Value = value.ToString();
        }

        public string ToJSON()
        {
            return System.Text.Json.JsonSerializer.Serialize(this);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Avalon
{
    /// <summary>
    /// The function metadata definition
    /// </summary>
    public abstract class FunctionDefinition
    {
        public Func<int> Function { get; set; } = () => { return 0; }; //default function

        /// <summary>
        /// The state machine that this function is a part of
        /// </summary>
        public StateMachine? StateMachine { get; set; } = null;

        /// <summary>
        /// The name of the function
        /// </summary>
        /// <value></value>
        public string Name { get; set; } = "";

        /// <summary>
        /// the description of the function
        /// </summary>
        public string Description { get; set; } = "";

        /// <summary>
        /// The dictionary of expected parameters and types
        /// </summary>
        /// <typeparam name="StateMachineVariableType"></typeparam>
        /// <returns></returns>
        public Dictionary<string, ReferenceTuple> ExpectedParameters { get; set; } = new Dictionary<string, ReferenceTuple>();

        /// <summary>
        /// Get the list of expected parameters
        /// </summary>
        public ExpectedParameter[] ExpectedParametersList
        {
            get
            {
                List<ExpectedParameter> parameters = new List<ExpectedParameter>();

                foreach (var x in ExpectedParameters)
                {
                    parameters.Add(new ExpectedParameter(x.Key, x.Value.type.ToString()));
                }

                return parameters.ToArray();
            }
        }

        /// <summary>
        /// The dictionary of parameters
        /// </summary>
        /// <value></value>
        protected Dictionary<string, KeyTypeDefinition> Parameters { get; set; } = new Dictionary<string, KeyTypeDefinition>();

        /// <summary>
        /// Inject the parameters into the function definition so the function knows what to call
        /// </summary>
        /// <param name="parameters">the list of parameters</param>
        /// <param name="result">the result of the injection, outputs error messages</param>
        /// <returns></returns>
        public bool TryInjectParameters(Dictionary<string, KeyTypeDefinition> parameters, out string result)
        {
            if (parameters.Count == ExpectedParameters.Count)
            {

                foreach (var x in parameters)
                {
                    if (ExpectedParameters.ContainsKey(x.Key))
                    {
                        if (ExpectedParameters[x.Key].type == x.Value.Type)
                        {
                            Parameters.Add(x.Key, x.Value);
                            ExpectedParameters[x.Key] = new ReferenceTuple(ExpectedParameters[x.Key].type, true);
                        }
                        else
                        {
                            result = "Parameter type mismatch. The expected type of parameter " + x.Value.Key + " is " + ExpectedParameters[x.Key].type + " but " + x.Value.Type + " was provided for the function " + Name + ".";
                            return false;
                        }
                    }
                    else
                    {

                        result = "";
                        List<string> keys = new List<string>();
                        foreach (var y in ExpectedParameters)
                        {
                            if (y.Value.applied == false)
                            {
                                keys.Add(y.Key);
                            }
                        }
                        result = "Parameter mismatch. The expected parameters " + string.Join(", ", keys) + " were not provided for the function " + Name + ".";
                        return false;
                    }
                }

                result = "Success!";
                return true;
            }
            else
            {
                result = "Parameter count mismatch. The expected number of parameters is " + ExpectedParameters.Count + " but " + parameters.Count + " were provided.";
                return false;
            }
        }

        protected abstract void DefineFunction();

        public bool TryGetVariableType(string name, out StateMachineVariableType result)
        {
            if (ExpectedParameters.ContainsKey(name))
            {
                result = ExpectedParameters[name].type;
                return true;
            }

            result = StateMachineVariableType.Text;
            return false;
        }
    }

    /// <summary>
    /// A struct representing an expected parameter
    /// </summary>
    public struct ExpectedParameter
    {
        public string Name { get; init; }
        public string Type { get; init; }

        public string Description { get; init; }

        public ExpectedParameter(string name, string type, string description = "")
        {
            Name = name;
            Type = type;
            Description = description;
        }
    }
}
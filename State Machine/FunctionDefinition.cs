using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;


namespace ValkyrieFSMCore
{
    /// <summary>
    /// The function metadata definition
    /// </summary>
    public abstract class FunctionDefinition
    {
        /// <summary>
        /// Function to run
        /// </summary>
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
        /// The required param count
        /// </summary>
        public int RequiredParamCount => ExpectedParameters.Count(x => x.Value.Required);

        /// <summary>
        /// The dictionary of expected parameters and types
        /// </summary>
        /// <typeparam name="StateMachineVariableType"></typeparam>
        /// <returns></returns>
        public Dictionary<string, Parameter> ExpectedParameters { get; set; } = new Dictionary<string, Parameter>();

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
                    parameters.Add(new ExpectedParameter(x.Key, x.Value.Type.ToString(), x.Value.IO.ToString(), x.Value.Description));
                }

                return parameters.ToArray();
            }
        }

        /// <summary>
        /// The dictionary of parameters
        /// </summary>
        /// <value></value>
        public Dictionary<string, IVariableSignature> Parameters { get; set; } = new Dictionary<string, IVariableSignature>();



        public VariableDefinition<T> GetVariableDefinitionParameter<T>(string name)
        {
            var param = Parameters[name] as VariableDefinition<T>;

            if (param == null)
            {
                throw new Exception("Parameter " + name + " is not of type " + typeof(T).Name);
            }

            return param;
        }

        /// <summary>
        /// Get the parameter
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public T Get<T>(string name)
        {

            var param = GetVariableDefinitionParameter<T>(name);

            return param.Value;
        }

        /// <summary>
        /// Try get the value from the parameter
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public bool TryGet<T>(string name, out T? value)
        {

            if (!Parameters.ContainsKey(name))
            {
                value = default(T);
                return false;
            }

            var param = GetVariableDefinitionParameter<T>(name);
            value = param.Value;
            return true;

        }

        /// <summary>
        /// set the parameter
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void Set<T>(string name, T value)
        {
            var param = GetVariableDefinitionParameter<T>(name);
            param.Value = value;
        }

        /// <summary>
        ///  set the parameter if it exists
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TrySet<T>(string name, T value)
        {
            if (!Parameters.ContainsKey(name))
            {
                return false;
            }

            var param = GetVariableDefinitionParameter<T>(name);
            param.Value = value;
            return true;
        }

        /// <summary>
        /// Inject the parameters into the function definition so the function knows what to call
        /// </summary>
        /// <param name="parameters">the list of parameters</param>
        /// <param name="result">the result of the injection, outputs error messages</param>
        /// <returns></returns>
        public bool TryInjectParameters(Dictionary<string, IVariableSignature> parameters, out string result)
        {
            //if (parameters.Count >= RequiredParamCount)
            //{

            foreach (var x in parameters)
            {

                Debug.WriteLine(x.Key + " " + x.Value.Key);

                if (ExpectedParameters.ContainsKey(x.Key) && !Parameters.ContainsKey(x.Key))
                {

                    var paramType = ExpectedParameters[x.Key].Type;
                    var paramIo = ExpectedParameters[x.Key].IO;

                    var valueType = x.Value.Type;
                    var valueIo = x.Value.IO;

                    if (paramType == valueType)
                    {
                        Parameters.Add(x.Key, x.Value);
                        ExpectedParameters[x.Key] = new Parameter(ExpectedParameters[x.Key].Type, parameterInjectedSuccessfully: true);
                        Debug.WriteLine("\t\t" + x.Key + " injected successfully");
                    }
                    else
                    {
                        if (paramType.Trim().ToLower() == "any" && paramIo == VariableIO.In)
                        {
                            Parameters.Add(x.Key, x.Value);
                            ExpectedParameters[x.Key] = new Parameter(ExpectedParameters[x.Key].Type, parameterInjectedSuccessfully: true);
                            Debug.WriteLine("\t\t" + x.Key + " injected successfully");
                        }
                        else if (valueType.Trim().ToLower() == "any" && valueIo == VariableIO.In)
                        {
                            Parameters.Add(x.Key, x.Value);
                            ExpectedParameters[x.Key] = new Parameter(ExpectedParameters[x.Key].Type, parameterInjectedSuccessfully: true);
                            Debug.WriteLine("\t\t" + x.Key + " injected successfully");
                        }
                        else
                        {
                            result = "Parameter type mismatch. The expected type of parameter " + x.Value.Key + " is " + ExpectedParameters[x.Key].Type + " but " + x.Value.Type + " was provided for the function " + Name + ".";
                            return false;
                        }
                    }
                }
                else
                {

                    result = "";
                    List<string> keys = new List<string>();
                    foreach (var y in ExpectedParameters)
                    {
                        if (y.Value.InjectedSuccessfully == false)
                        {
                            keys.Add(y.Key);
                        }
                    }
                    result = "Parameter mismatch. The expected parameters " + string.Join(", ", keys) + " were not provided for the function " + Name + ".";
                    return false;
                }
            }

            foreach (var x in ExpectedParameters)
            {
                if (x.Value.Required && x.Value.InjectedSuccessfully == false)
                {
                    result = "Parameter mismatch. The expected parameter " + x.Key + " was not provided for the function " + Name + ".";
                    return false;
                }
            }

            result = "Success!";
            return true;
            //}
            //else
            //{
            //    result = "Parameter count mismatch. The expected number of connected parameters should be at least " + ExpectedParameters.Count + " but only " + parameters.Count + " were provided for the function " + Name + ".";
            //    return false;
            //}
        }

        protected abstract void DefineFunction();

        public bool TryGetVariableType(string name, out string result)
        {
            if (ExpectedParameters.ContainsKey(name))
            {
                result = ExpectedParameters[name].Type;
                return true;
            }

            result = StateMachineVariableType.Text.ToString();
            return false;
        }
    }

    /// <summary>
    /// A struct representing an expected parameter
    /// </summary>
    public class ExpectedParameter
    {
        public string Name { get; init; }
        public string Type { get; init; }
        public string Description { get; init; }
        public string IO { get; init; }

        public ExpectedParameter(string name, string type, string io, string description = "")
        {
            Name = name;
            Type = type;
            Description = description;
            IO = io;
        }
    }
}
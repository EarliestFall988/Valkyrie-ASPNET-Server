
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using System.Text.Json;
using System.Linq;
using System.Diagnostics;

namespace ValkyrieFSMCore
{
    /// <summary>
    /// The State Machine Constructor handles the parsing of the program file and the creation of the state machine.
    /// </summary>
    public class StateMachineBuilder
    {
        private Dictionary<string, FunctionDefinition> _functions { get; set; }
        private Dictionary<string, State> _states { get; set; }
        private Dictionary<string, Transition> _transitions { get; set; }
        private Dictionary<string, IVariableSignature> _variables { get; set; }

        private FunctionLibrary _functionLibrary { get; set; }
        private StateMachine _stateMachine { get; set; }

        private VariableFactory _factory { get; set; } = new();

        // public Dictionary<string, GameObject> GameObjectRefDict = new Dictionary<string, GameObject>();
        public Dictionary<string, string> stringRefDict = new Dictionary<string, string>();
        public Dictionary<string, float> floatRefDict = new Dictionary<string, float>();

        //string FilePath = "";

        public StateMachineBuilder()
        {

            _stateMachine = new StateMachine();
            _functionLibrary = new FunctionLibrary();
            _functions = new Dictionary<string, FunctionDefinition>();
            _states = new Dictionary<string, State>();
            _transitions = new Dictionary<string, Transition>();
            _variables = new Dictionary<string, IVariableSignature>();
        }

        public Func<int> GetDefaultFunction()
        {
            Func<int> Next = () => 1;
            return Next;
        }

        public void AddState(string name, string jobName)
        {

            string trimmedName = name.Trim();
            string trimmedJobName = jobName.Trim();

            var stateJob = GetDefaultFunction();

            if (trimmedJobName != string.Empty)
                stateJob = _functions[trimmedJobName].Function;

            AddState(new State(trimmedName, stateJob));
            if (trimmedJobName != null && trimmedJobName != string.Empty)
                Debug.WriteLine("adding state " + trimmedName + " : " + trimmedJobName);
            else
                Debug.WriteLine("adding state " + trimmedName + " : " + "default");
        }

        public void AddState(State state)
        {
            _states.Add(state.Name, state);

        }

        public void AddStartState(string name, string jobName, StateMachine machine)
        {

            string trimmedName = name.Trim();
            string trimmedJobName = jobName.Trim();

            var stateJob = _functions[trimmedJobName];

            State state = new State(trimmedName, stateJob.Function);

            machine.InitialState = state;
            Debug.WriteLine("adding initial " + trimmedName + " : " + trimmedJobName);

            AddState(state);

        }

        public void AddFallBackState(string name, string jobName, StateMachine machine)
        {

            string trimmedName = name.Trim();
            string trimmedJobName = jobName.Trim();

            var stateJob = GetDefaultFunction();

            if (trimmedJobName != string.Empty)
                stateJob = _functions[trimmedJobName].Function;

            // States.Add(name, new State(name, stateJob));
            var state = new State(trimmedName, stateJob);

            machine.FallbackState = state;
            if (trimmedJobName != null && trimmedJobName != string.Empty)
                Debug.WriteLine("adding fallback " + trimmedName + " : " + trimmedJobName);
            else
                Debug.WriteLine("adding fallback " + trimmedName + " : " + "default");


            AddState(state);
        }

        public void AddTransition(string name, string from, string to, int outcome, StateMachine machine)
        {

            string trimmedName = name.Trim();
            string trimmedFrom = from.Trim();
            string trimmedTo = to.Trim();

            // foreach (var x in States.Keys)
            // {
            //     Debug.WriteLine(x);
            // }

            var transitionName = trimmedFrom + " -> " + trimmedTo;

            Debug.WriteLine(trimmedName);

            Transition t = new Transition(_states[trimmedFrom], _states[trimmedTo], trimmedName, outcome);

            _transitions.Add(trimmedName, t);


            if (_states[trimmedFrom].FallbackState)
            {
                if (machine.FallbackState != null)
                {
                    machine.FallbackState.Transitions.Add(_transitions[trimmedName]);
                    // Debug.WriteLine("Added fallback state transition.");
                }
                else
                {
                    Debug.WriteLine("fallback state not set in the state machine.");
                }
            }

            if (_states[trimmedFrom].InitialState)
            {
                if (machine.InitialState != null)
                {
                    machine.InitialState.Transitions.Add(_transitions[trimmedName]);
                    // Debug.WriteLine("Added initial state transition");
                }
                else
                {
                    Debug.WriteLine("initial state not set in the state machine.");
                }
            }

            if (!_states[trimmedFrom].InitialState && !_states[trimmedFrom].FallbackState)
            {
                _states[trimmedFrom].Transitions.Add(t);
            }

            Debug.WriteLine("adding transition " + trimmedName + " (" + trimmedFrom + " -> " + trimmedTo + " && " + outcome + ")");
        }

        private bool GetType(string type, out StateMachineVariableType result)
        {
            string trimmedType = type.Trim().ToLower();

            switch (trimmedType)
            {
                case "text":
                    result = StateMachineVariableType.Text;
                    return true;

                case "integer":
                    result = StateMachineVariableType.Integer;
                    return true;

                case "decimal":
                    result = StateMachineVariableType.Decimal;
                    return true;

                case "yesno":
                    result = StateMachineVariableType.YesNo;
                    return true;

                // case "gameobject":
                //     result = StateMachineVariableType.GameObject;
                //     return true;

                // case "vector3":
                //     result = StateMachineVariableType.Vector3;
                //     return true;

                // case "float":
                //     result = StateMachineVariableType.Single;
                //     return true;

                default:
                    result = StateMachineVariableType.Text;
                    return false;
            }
        }

        private void InjectParameters(FunctionDefinition function, Dictionary<string, IVariableSignature> parameters, int lineNumber)
        {
            if (parameters.Count > 0 && function != null)
            {
                bool success = function.TryInjectParameters(parameters, out var result);
                if (!success)
                {
                    throw new Exception($"{result} (at line {lineNumber})");
                }

                parameters.Clear();
            }
        }

        [Obsolete("this method is only being used for the non JSON parser")]
        public async Task<string> GetProgramFile(string path)
        {
            string structure = "";
            using (StreamReader reader = File.OpenText(path))
            {
                structure = await reader.ReadToEndAsync();
            }

            return structure;
        }

        public StateMachine ParseInstructionsJSON(string json)
        {
            var jsonDoc = JsonDocument.Parse(json);

            var root = jsonDoc.RootElement;

            var variables = root.GetProperty("variables");
            var functionsJson = root.GetProperty("functions");
            var states = root.GetProperty("states");
            var transitions = root.GetProperty("transitions");

            bool createdStart = false;
            bool createdFallback = false;

            bool createdVariables = false;
            bool createdFunctions = false;
            bool createdStates = false;
            bool createdTransitions = false;

            foreach (var x in variables.EnumerateArray())
            {
                var name = x.GetProperty("name").GetString();
                var type = x.GetProperty("type").GetString();
                var value = x.GetProperty("value").GetString();


                var foundProperty = x.TryGetProperty("visibility", out var refProp);
                string? visibility = "";
                if (foundProperty) visibility = refProp.GetString();


                var foundIO = x.TryGetProperty("io", out var refIO);
                string io = "";
                if (foundIO) io = refIO.GetString() ?? "";

                VariableIO ioResult = VariableIO.In;

                if (io.Trim().ToLower() == "output" || io.Trim().ToLower() == "out")
                {
                    ioResult = VariableIO.Out;
                }

                // Debug.WriteLine($"adding variable {name} ({type}) =  {value}.");

                if (type == null)
                    throw new Exception($"Invalid variable definition. A valid type must be given after the name. ({type})");

                if (value == null)
                    throw new Exception($"Invalid variable definition. A valid value must be given after the name. ({value})");

                bool result = _factory.ContainsType(type);

                if (!result)
                    throw new Exception($"Invalid variable definition. A valid type must be given after the name. You probably need to register the variable" +
                        $"in the variable factory so it can be properly created upon building the state machine. ({type})");

                if (name == null)
                    throw new Exception($"Invalid variable definition. A valid name must be given after the definition.");

                if (name == "")
                    throw new Exception($"Invalid variable definition. A valid name must be given after the definition.");

                if (_variables.ContainsKey(name))
                    throw new Exception($"Invalid variable definition. The variable {name} already exists.");


                if (_factory.TryConstructVariable(name, type, out IVariableSignature? variableConstructionResult, ioResult))
                {
                    if (variableConstructionResult != null)
                        _variables.Add(name, variableConstructionResult);
                    else
                        Debug.WriteLine(GetType().Name + " : " + "variable construction result is null - you did this intentionally to yourself :)");
                }
                else
                {
                    Debug.WriteLine(GetType().Name + " : " + "variable construction failed - you probably need to register the variable in the variable factory.");
                }

                //if (variableType == StateMachineVariableType.Text)
                //{
                //    Debug.WriteLine($"adding variable {name} ({variableType}) = \"{value}\".");
                //}
                //else
                //{
                //    Debug.WriteLine($"adding variable {name} ({variableType}) = {value}.");
                //}


                //switch (variableType)
                //{
                //    case StateMachineVariableType.Text:
                //        _variables.Add(name, VariableDefinition<string>.CreateString(name, value));
                //        break;
                //    case StateMachineVariableType.Decimal:
                //        _variables.Add(name, VariableDefinition<decimal>.CreateDecimal(name, decimal.Parse(value)));
                //        break;

                //    case StateMachineVariableType.Integer:
                //        _variables.Add(name, VariableDefinition<int>.CreateInt(name, int.Parse(value)));
                //        break;

                //    case StateMachineVariableType.YesNo:
                //        _variables.Add(name, VariableDefinition<bool>.CreateBool(name, bool.Parse(value)));
                //        break;

                //    default:
                //        _variables.Add(name, new KeyTypeDefinition(name, variableType.ToString(), value));
                //        break;
                //}

                //Variables.Add(name, new KeyTypeDefinition(name, variableType, value));
                createdVariables = true;
            }

            foreach (var x in functionsJson.EnumerateArray())
            {
                var name = x.GetProperty("name").GetString();
                var parameters = x.GetProperty("parameters");
                // var code = x.GetProperty("code").GetString();




                Dictionary<string, Parameter> parameterList = new Dictionary<string, Parameter>();

                Dictionary<string, IVariableSignature> injectionVariables = new Dictionary<string, IVariableSignature>();

                if (name == null)
                    throw new Exception($"Invalid function definition. A valid name must be given after the definition.");

                if (!_functionLibrary.TryGetFunction(name, out var function))
                    throw new Exception($"Invalid function definition. The function {name} does not exist.");

                if (function == null)
                    throw new Exception($"Invalid function definition. The function {name} does not exist.");

                foreach (var y in parameters.EnumerateArray())
                {
                    var paramName = y.GetProperty("name").GetString();
                    var paramType = y.GetProperty("type").GetString();
                    var varToConnectName = y.GetProperty("connectVar").ToString();

                    //var overrideType = "";

                    if (paramName == null)
                    {
                        throw new Exception($"Invalid function parameter definition. A valid name must be given after the definition.");
                    }

                    // #region unity specific
                    //var extResult = ParseExtraneousVariables(varToConnectName);


                    //if (extResult.type != "")
                    //{
                    //    varToConnectName = extResult.name;
                    //}

                    // if (paramType == "decimal")
                    // {
                    //     paramType = "float";
                    // }

                    //var extParamName = ParseExtraneousVariables(paramName);

                    //if (extParamName.type != "")
                    //{
                    //    paramName = extParamName.name;
                    //    overrideType = extParamName.type;
                    //}

                    //if (varToConnectName == "")
                    //{
                    //    varToConnectName = extParamName.name;
                    //}

                    // #endregion

                    if (paramName == null)
                        throw new Exception($"Invalid function parameter definition. A valid name must be given after the definition.");

                    if (paramName == "")
                        throw new Exception($"Invalid function parameter definition. A valid name must be given after the definition.");

                    if (paramType == null)
                        throw new Exception($"Invalid function parameter definition. A valid type must be given after the name.");


                    bool result = _factory.ContainsType(paramType);

                    if (!result)
                        throw new Exception($"Invalid variable definition. A valid type must be given after the name. You probably need to register the variable" +
                            $"in the variable factory so it can be properly created upon building the state machine. ({paramName} : {paramType})");


                    parameterList.Add(paramName, new Parameter(paramType, parameterInjectedSuccessfully: true));

                    if (!_variables.TryGetValue(varToConnectName, out var variable))
                        throw new Exception($"Invalid function parameter definition. The connection variable \"{varToConnectName}\" does not exist. ({name} - {paramName} : {paramType})");

                    if (variable == null)
                        throw new Exception($"Invalid function parameter definition. The connection variable \"{varToConnectName}\" cannot be generated. ({name} - {paramName} : {paramType})");

                    if (variable.Type != paramType)
                        throw new Exception($"Invalid function parameter definition. The connection variable \"{varToConnectName}\" is not of type {paramType}. ({name} - {paramName} : {paramType})");

                    injectionVariables.Add(paramName, variable);
                }

                function.ExpectedParameters = parameterList;

                bool injectionResult = function.TryInjectParameters(injectionVariables, out var result2);

                if (!injectionResult)
                    throw new Exception($"{result2}");

                if (_functions.ContainsKey(name))
                    throw new Exception($"Invalid function definition. The function {name} already exists.");

                _functions.Add(name, function);

                createdFunctions = true;
            }


            foreach (var x in _functions)
            {
                Debug.WriteLine($"Function added {x.Key} {string.Join(',', x.Value.ExpectedParameters.Keys)} .");
            }


            foreach (var s in states.EnumerateArray())
            {

                var name = s.GetProperty("name").GetString();
                var isStart = false;
                var isFallback = false;

                var type = s.GetProperty("type").GetString();

                if (name == null || name == "")
                    throw new Exception($"Invalid state definition. A valid name must be given after the definition.");

                if (type == "start")
                {
                    isStart = true;
                }
                else if (type == "fallback")
                {
                    isFallback = true;
                }
                else if (type != "state")
                {
                    throw new Exception($"Invalid state definition. A state must be of type 'state, fallback, or start'.");
                }

                if (isStart && createdStart)
                    throw new Exception($"Invalid state definition. A start state has already been defined.");

                if (isFallback && createdFallback)
                    throw new Exception($"Invalid state definition. A fallback state has already been defined.");

                if (isStart)
                {
                    createdStart = true;
                }

                if (isFallback)
                {
                    createdFallback = true;
                }

                if (_states.ContainsKey(name))
                    throw new Exception($"Invalid state definition. The state {name} already exists."); //TODO: should be dependant on ID not the name....

                var functionName = s.GetProperty("function").GetString();

                if (functionName == null)
                    throw new Exception($"Invalid state definition. A valid function must be given after the definition. (Make sure the function is declared in the functions section of the json file.)");


                if (isStart)
                {
                    AddStartState(name, functionName, _stateMachine);
                }
                else if (isFallback)
                {
                    AddFallBackState(name, functionName, _stateMachine);
                }
                else
                {
                    AddState(name, functionName);
                }

                createdStates = true;
            }


            foreach (var x in _states)
            {
                Debug.WriteLine($"State added {x.Key}.");
            }

            foreach (var t in transitions.EnumerateArray())
            {
                var from = t.GetProperty("from").GetString();
                var to = t.GetProperty("to").GetString();
                var outcome = t.GetProperty("outcome").GetInt32();

                string name = $"transition {from} -> {to} : {outcome}";

                if (from == null || from == "")
                    throw new Exception($"Invalid transition definition. A valid from state must be given after the definition.");

                if (to == null || to == "")
                    throw new Exception($"Invalid transition definition. A valid to state must be given after the definition.");

                if (name == null || name == "")
                    throw new Exception($"Invalid transition definition. A valid name must be given after the definition.");

                AddTransition(name, from, to, outcome, _stateMachine);

                createdTransitions = true;
            }


            Debug.WriteLine("\n\t>Building State Machine...\n\n");


            if (!createdVariables)
            {
                //Debug.BackgroundColor = DebugColor.Yellow;
                Debug.WriteLine("No variables defined.");
                //Debug.ResetColor();
            }

            if (!createdFunctions || _functions.Count < 1)
            {
                //Debug.BackgroundColor = DebugColor.Yellow;
                Debug.WriteLine("No functions defined.");
                //Debug.ResetColor();
            }

            if (!createdStart)
            {
                throw new Exception($"No start state defined.");
            }

            if (!createdFallback)
            {
                throw new Exception($"No fallback state defined.");
            }

            if (!createdStates)
            {
                //Debug.BackgroundColor = DebugColor.Yellow;
                Debug.WriteLine("No states defined.");
                //Debug.ResetColor();
            }

            if (!createdTransitions)
            {
                //Debug.BackgroundColor = DebugColor.Yellow;
                Debug.WriteLine("No transitions defined.");
                //Debug.ResetColor();
            }

            Debug.WriteLine("\t>Result:\n\n");

            //Debug.ForegroundColor = DebugColor.Green;
            Debug.WriteLine("Imported Functions:");
            //Debug.ResetColor();
            foreach (var x in _functions.Values)
            {

                string pmtrs = "";

                foreach (var y in x.ExpectedParameters)
                {
                    pmtrs += y.Key + ": " + y.Value.Type + ", ";
                }

                pmtrs = pmtrs.Trim().TrimEnd(',');

                Debug.WriteLine("|" + x.Name + " [" + pmtrs + "]");
            }

            //Debug.ForegroundColor = DebugColor.Green;
            Debug.WriteLine("\n\nDefined States:");
            //Debug.ResetColor();

            foreach (var x in _states.Values)
            {
                Debug.WriteLine("|" + x.Name + " [" + String.Join(',', x.Transitions) + "]");
            }

            Debug.WriteLine("\n\t>Program Loaded.\n");


            foreach (var x in _functions.Values)
            {
                x.StateMachine = _stateMachine;
            }

            _stateMachine.States.AddRange(_states.Values);
            _stateMachine.Variables = _variables;
            _stateMachine.CurrentState = _stateMachine.InitialState;

            return _stateMachine;
        }

        [Obsolete("This method is deprecated, use ParseInstructionsJSON instead.")]
        public async Task<StateMachine> ParseInstructions(string filePath)
        {

            string structure = await GetProgramFile(filePath);

            string[] lines = structure.Split("\n");
            var defineStates = false;
            var hasDefinedStates = false;

            var createVariables = false;
            var hasCreatedVariables = false;

            var importFunctions = false;
            FunctionDefinition? currentFunction = null; // default function
            Dictionary<string, IVariableSignature> parameters = new Dictionary<string, IVariableSignature>();
            var hasImportedFunctions = false;

            var defineTransitions = false;

            var createdStart = false;
            var createdFallback = false;

            int lineNumber = 1;

            Debug.WriteLine("\t>Starting Parser and Interpreter...\n\n");

            foreach (var line in lines)
            {
                var x = line.Trim();
                if (x.StartsWith("//") || x == "")
                    continue;

                if (x.ToLower().Trim() == "create")
                {
                    createVariables = true;
                    continue;
                }

                if (x.ToLower().Trim() == "end create")
                {
                    createVariables = false;
                    hasCreatedVariables = true;
                    Debug.WriteLine("");
                    continue;
                }


                if (x.ToLower().Trim() == "end create")
                {
                    createVariables = false;
                    hasCreatedVariables = true;
                    Debug.WriteLine("");
                    continue;
                }

                if (x.ToLower().Trim() == "import")
                {
                    importFunctions = true;
                    continue;
                }

                if (x.ToLower().Trim() == "end import")
                {
                    if (currentFunction != null)
                        InjectParameters(currentFunction, parameters, lineNumber);

                    importFunctions = false;
                    hasImportedFunctions = true;
                    Debug.WriteLine("");
                    continue;
                }

                if (x.ToLower().Trim() == "define")
                {
                    defineStates = true;
                    continue;
                }

                if (x.ToLower().Trim() == "end define")
                {
                    defineStates = false;
                    hasDefinedStates = true;
                    Debug.WriteLine("");
                    continue;
                }
                if (x.ToLower().Trim() == "connect")
                {
                    if (!hasDefinedStates)
                        throw new Exception($"Cannot define transitions without defining states first. (at line {lineNumber})");

                    defineTransitions = true;
                    continue;
                }

                if (x.ToLower().Trim() == "end connect")
                {
                    defineTransitions = false;
                    hasCreatedVariables = true;
                    Debug.WriteLine("");
                    continue;
                }

                if (createVariables)
                {
                    var split = x.Split(" ");
                    var name = split[0];

                    var type = split[1];


                    if (name == null || name == "")
                        throw new Exception($"Invalid variable definition. A valid name must be given after the definition. (at line {lineNumber})");

                    if (type == null || type == "")
                        throw new Exception($"Invalid variable definition. A valid type must be given after the name. Currently it does not exist. (at line {lineNumber})");

                    var captureValue = x.Split("=");
                    var value = "";

                    if (captureValue.Length < 2 || captureValue[1] == null || captureValue[1] == string.Empty)
                    {
                        throw new Exception($"Invalid variable definition. A valid variable must be given after the \'=\' sign. (at line {lineNumber})");
                    }


                    bool result = GetType(type, out var variableType);

                    if (!result)
                        throw new Exception($"Invalid variable definition. A valid type must be given after the name. ({type}) (at line {lineNumber})");

                    if (variableType == StateMachineVariableType.Text)
                    {

                        var text = captureValue[1].Trim();


                        for (int i = 0; i < text.Length; i++)
                        {

                            if (text[i] == '\"')
                            {
                                continue;
                            }

                            if (text[i] == '\\')
                            {
                                if (text.Length > i + 1)
                                {
                                    if (text[i + 1] == '\"')
                                    {
                                        value += '\"';
                                        i++;
                                        continue;
                                    }
                                }
                            }
                            else
                            {
                                value += text[i];
                            }
                        }
                    }
                    else
                    {
                        value = captureValue[1].Trim();
                    }

                    // Debug.WriteLine($"adding variable {name} ({variableType}) =  {value}.");

                    // KeyTypeDefinition def = new KeyTypeDefinition(name, variableType, value);
                    // Debug.WriteLine($"adding variable {def.Key} ({def.Type}) =  {def.Value}.");

                    if (name == null)
                        throw new Exception($"Invalid variable definition. A valid name must be given after the definition. (at line {lineNumber})");

                    if (name == "")
                        throw new Exception($"Invalid variable definition. A valid name must be given after the definition. (at line {lineNumber})");


                    if (_variables.ContainsKey(name))
                        throw new Exception($"Invalid variable definition. The variable {name} already exists. (at line {lineNumber})");

                    if (variableType == StateMachineVariableType.Text)
                    {
                        Debug.WriteLine($"adding variable {name} ({variableType}) = \"{value}\".");
                    }
                    else
                    {
                        Debug.WriteLine($"adding variable {name} ({variableType}) = {value}.");
                    }

                    _variables.Add(name, new KeyTypeDefinition(name, variableType.ToString(), value));

                    continue;
                }

                if (importFunctions)
                {
                    var split = x.Split(" ");
                    var isUsing = split[0].Trim().ToLower() == "using";
                    var functionName = split[1].Trim();

                    var insertFunParamName = "";
                    var paramName = "";

                    if (x.Contains("="))
                    {
                        insertFunParamName = x.Split("=")[0].Trim();
                        paramName = x.Split("=")[1].Trim();
                    }


                    if (functionName.Length < 1)
                        throw new Exception($"Invalid function definition. A valid function name must be given after the definition. (at line {lineNumber})");





                    if (isUsing)
                    {
                        Debug.WriteLine($"adding function {functionName}.");

                        if (parameters.Count > 0 && currentFunction != null)
                        {
                            InjectParameters(currentFunction, parameters, lineNumber);
                        }

                        if (!_functionLibrary.TryGetFunction(functionName, out var function))
                            throw new Exception($"Invalid function definition. The function {functionName} does not exist. (at line {lineNumber})");

                        if (function == null)
                            throw new Exception($"Invalid function definition. The function {functionName} does not exist. (at line {lineNumber})");

                        currentFunction = function;
                        _functions.Add(functionName, currentFunction);
                        continue;
                    }
                    else
                    {

                        Debug.WriteLine($"adding parameter '{paramName}' to function insert '{insertFunParamName}' in function {currentFunction?.Name ?? "unknown"}.");

                        if (currentFunction == null)
                            throw new Exception($"Invalid function definition. The function {functionName} does not exist. (at line {lineNumber})");

                        if (_functions.ContainsKey(functionName))
                            throw new Exception($"Invalid function definition. The function {functionName} already exists. (at line {lineNumber})");

                        string[] splitStr = x.Split("=");
                        string insertName = splitStr[0];
                        string variableName = "";

                        if (splitStr.Length > 1)
                            variableName = splitStr[1];
                        else
                            throw new Exception($"Invalid function parameter definition. A valid variable name must be given after the parameter insert name. (at line {lineNumber})");

                        variableName = variableName.Trim();
                        insertName = insertName.Trim();

                        if (insertName == null || insertName == "")
                            throw new Exception($"Invalid function parameter definition. A valid function insert name must be given. (at line {lineNumber})");

                        if (variableName == null || variableName == "")
                            throw new Exception($"Invalid function parameter definition. A valid variable name must be given after the parameter insert name. (at line {lineNumber})");

                        if (!_variables.ContainsKey(variableName))
                            throw new Exception($"Invalid function parameter definition. The variable {variableName} does not exist. (at line {lineNumber})");

                        var variable = _variables[variableName];

                        currentFunction.TryGetVariableType(insertName, out var variableType);

                        if (variable.Type != variableType.ToString())
                            throw new Exception($"Invalid function parameter definition. The variable {variableName} is not of type {variableType}. (at line {lineNumber})");

                        parameters.Add(insertName, variable);

                        continue;
                    }
                }

                if (defineStates)
                {
                    if (x.ToLower().StartsWith("state"))
                    {
                        if (x.Split(" ").Length < 3 || !x.Contains(":"))
                            throw new Exception($"Invalid state definition. (at line {lineNumber})");

                        var stateName = x.Split(" ")[1];
                        var splitCheck = x.Split(":");
                        var stateFunctionName = "";
                        if (splitCheck != null && splitCheck.Length > 1) stateFunctionName = x.Split(":")[1];

                        if (stateName == null || stateName == "")
                            throw new Exception($"Invalid fallback state definition. A valid name must be given after the definition. (at line {lineNumber})");


                        AddState(stateName, stateFunctionName);


                        continue;
                    }

                    if (x.ToLower().StartsWith("start"))
                    {
                        if (x.Split(" ").Length < 3 || !x.Contains(":"))
                            throw new Exception($"Invalid start state definition. (at line {lineNumber})");

                        var stateName = x.Split(" ")[1];
                        var stateFunctionName = x.Split(":")[1] ?? throw new Exception($"Invalid start state definition. A valid function must be given after the definition and name. (at line {lineNumber})");

                        if (stateName == null || stateName == "")
                            throw new Exception($"Invalid fallback state definition. A valid name must be given after the definition. (at line {lineNumber})");

                        // AddState(stateName, stateFunctionName);
                        AddStartState(stateName, stateFunctionName, _stateMachine);

                        _states[stateName].InitialState = true;

                        createdStart = true;

                        continue;
                    }

                    if (x.ToLower().StartsWith("fallback"))
                    {
                        if (x.Split(" ").Length < 2)
                            throw new Exception($"Invalid fallback state definition. (at line {lineNumber})");

                        var stateName = x.Split(" ")[1];
                        var splitCheck = x.Split(":");
                        var stateFunctionName = "";
                        if (splitCheck != null && splitCheck.Length > 1) stateFunctionName = x.Split(":")[1];

                        if (stateName == null || stateName == "")
                            throw new Exception($"Invalid fallback state definition. A valid name must be given after the definition. (at line {lineNumber})");

                        // Debug.WriteLine("creating fallback");

                        // AddState(stateName, stateFunctionName);
                        AddFallBackState(stateName, stateFunctionName, _stateMachine);

                        _states[stateName].FallbackState = true;

                        createdFallback = true;

                        continue;
                    }
                }

                if (defineTransitions)
                {
                    if (!x.Contains("->"))
                    {
                        throw new Exception($"Invalid transition definition. (at line {lineNumber})");
                    }

                    var separator = x.Split("=");
                    var name = separator[0].Trim();
                    var transition = separator[1].Trim();

                    var fromState = transition.Split("->")[0];
                    var toState = transition.Split("->")[1].Split(":")[0];
                    var outcome = int.Parse(transition.Split(":")[1].Trim());


                    AddTransition(name, fromState, toState, outcome, _stateMachine);

                    continue;
                }


                lineNumber++;
            }

            Debug.WriteLine("\t>Building State Machine...\n\n");

            if (!hasCreatedVariables)
            {
                //Debug.BackgroundColor = DebugColor.Yellow;
                Debug.WriteLine("No variables defined.");
                //Debug.ResetColor();
            }

            if (!hasImportedFunctions || _functions.Count < 1)
            {
                //Debug.BackgroundColor = DebugColor.Yellow;
                Debug.WriteLine("No functions defined.");
                //Debug.ResetColor();
            }

            if (!createdStart)
            {
                throw new Exception($"No start state defined.");
            }

            if (!createdFallback)
            {
                throw new Exception($"No fallback state defined.");
            }

            Debug.WriteLine("\t>Result:\n\n");

            //Debug.ForegroundColor = DebugColor.Green;
            Debug.WriteLine("Imported Functions:");
            //Debug.ResetColor();
            foreach (var x in _functions.Values)
            {

                string pmtrs = "";

                foreach (var y in x.ExpectedParameters)
                {
                    pmtrs += y.Key + ": " + y.Value.Type + ", ";
                }

                pmtrs = pmtrs.Trim().TrimEnd(',');

                Debug.WriteLine("|" + x.Name + " [" + pmtrs + "]");
            }

            //Debug.ForegroundColor = DebugColor.Green;
            Debug.WriteLine("\n\nDefined States:");
            //Debug.ResetColor();

            foreach (var x in _states.Values)
            {
                Debug.WriteLine("|" + x.Name + " [" + String.Join(',', x.Transitions) + "]");
            }

            Debug.WriteLine("\n\t>Program Loaded.\n");

            _stateMachine.States.AddRange(_states.Values);

            return _stateMachine;
        }

        /// <summary>
        /// Some variables can be parsed like such -> "variableName:variableType"
        /// </summary>
        /// <param name="variableName">the var name</param>
        /// <returns>returns the variable name and type as a tuple</returns>
        public (string name, string type) ParseExtraneousVariables(string variableName)
        {
            if (variableName.Contains(":"))
            {
                var result = variableName.Split(":");
                if (result.Length > 0)
                {
                    return (result[0], result[1]);
                }
            }

            return (variableName, "");
        }
    }

}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

using Valkyrie_Server.State_Machine.Function_Definitions;
using Valkyrie_Server.State_Machine.Function_Definitions.Math;

using ValkyrieFSMCore.WM;

namespace ValkyrieFSMCore
{
    /// <summary>
    /// The Function Library is a collection of all functions that can be called by the state machine.
    /// </summary>
    public class FunctionLibrary
    {
        /// <summary>
        /// The dictionary of all functions that can be called by the state machine.
        /// </summary>
        /// <typeparam name="string">the name of the function</typeparam>
        /// <typeparam name="FunctionDefinition">the Function Definition</typeparam>
        /// <returns></returns>
        public Dictionary<string, FunctionDefinition> ImportedFunctions = new Dictionary<string, FunctionDefinition>();

        /// <summary>
        /// Default constructor. Builds all functions.
        /// </summary>
        public FunctionLibrary() => BuildFunctionLibrary();

        public void BuildFunctionLibrary()
        {
            Debug.WriteLine("\t>Gathering Functions...\n");

            #region simple guess game tutorial functions

            ContinueFunction continueFunction = new ContinueFunction();
            ExitQuestionFunction exitQuestion = new ExitQuestionFunction();
            GreetUserFunction greetFunction = new GreetUserFunction();
            GuessFunction guessFunction = new GuessFunction();
            SetRandomNumberFunction randomFunction = new SetRandomNumberFunction();


            ImportedFunctions.Add(continueFunction.Name, continueFunction);
            ImportedFunctions.Add(exitQuestion.Name, exitQuestion);
            ImportedFunctions.Add(greetFunction.Name, greetFunction);
            ImportedFunctions.Add(guessFunction.Name, guessFunction);
            ImportedFunctions.Add(randomFunction.Name, randomFunction);

            #endregion

            #region math functions

            var addNumber = new AddNumber();
            var subtractNumber = new SubtractNumber();
            var multiplyNumber = new MultiplyNumber();
            var divideNumber = new DivideNumber();
            var powerNumber = new PowNumber();

            var convertfromIntToDec = new ConvertFromIntegerToDecimal();
            var convertfromdecToInt = new ConvertFromDecimalToInteger();

            ImportedFunctions.Add(addNumber.Name, addNumber);
            ImportedFunctions.Add(subtractNumber.Name, subtractNumber);
            ImportedFunctions.Add(divideNumber.Name, divideNumber);
            ImportedFunctions.Add(multiplyNumber.Name, multiplyNumber);
            ImportedFunctions.Add(powerNumber.Name, powerNumber);

            ImportedFunctions.Add(convertfromIntToDec.Name, convertfromIntToDec);
            ImportedFunctions.Add(convertfromdecToInt.Name, convertfromdecToInt);

            #endregion


            #region Project Functions

            var eachProject = new EachProject();
            var getProjects = new GetProjects();
            var splitProject = new SplitProject();
            var totalNumber = new TotalNumberOfProjects();

            ImportedFunctions.Add(eachProject.Name, eachProject);
            ImportedFunctions.Add(getProjects.Name, getProjects);
            ImportedFunctions.Add(splitProject.Name, splitProject);
            ImportedFunctions.Add(totalNumber.Name, totalNumber);

            #endregion


            #region start and end functions (needed for the state machine to work)

            var start = new StartFunction();
            var end = new ExitFunction();

            ImportedFunctions.Add(start.Name, start);
            ImportedFunctions.Add(end.Name, end);

            #endregion

            var respond = new Respond();

            ImportedFunctions.Add(respond.Name, respond);

            CheckFunctions(); // 👈 verifying that the functions are built correctly
        }


        void CheckFunctions()
        {
            foreach (var x in ImportedFunctions)
            {

                if (x.Key == null)
                {
                    throw new Exception("Function name is null");
                }

                if (x.Key != x.Value.Name)
                {
                    throw new Exception("Function name does not match key name");
                }


                if (x.Key.Length == 0)
                {
                    throw new Exception("Function name is empty");
                }

                if (x.Value.Function == null)
                {
                    throw new Exception("Function " + x.Key + " has no function defined");
                }
            }
        }

        /// <summary>
        /// Attempts to get a function from the library.
        /// </summary>
        /// <param name="name">the name of the function</param>
        /// <param name="function">the result of the function found (if successful)</param>
        /// <returns>returns true if the function operation was successful, false if not.</returns>
        public bool TryGetFunction(string name, out FunctionDefinition? function)
        {

            FunctionDefinition? func = null;

            bool res = ImportedFunctions.TryGetValue(name, out func);

            function = func;

            return res;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Avalon
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
        /// Default constructor. Imports all functions.
        /// </summary>
        public FunctionLibrary()
        {
            ImportFunctions();
        }

        public void ImportFunctions()
        {
            Console.WriteLine("\t>Gathering Functions...\n");
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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;


namespace ValkyrieFSMCore
{
    public sealed class ExitQuestionFunction : FunctionDefinition
    {

        public ExitQuestionFunction()
        {
            DefineFunction();
        }

        protected override void DefineFunction()
        {

            Name = "ExitQuestion";

            Func<int> ExitQuestion = () =>
            {

                Debug.WriteLine("Would you like to exit?");
                string? answer = "yes"; // Debug.ReadLine();

                if (answer == null || answer.Trim() == "")
                    return 0;

                // Debug.WriteLine("You typed in " + number);

                if (answer.Trim().ToLower() == "yes" || answer.Trim().ToLower() == "y") // exit the program
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            };

            Function = ExitQuestion;
        }
    }
}
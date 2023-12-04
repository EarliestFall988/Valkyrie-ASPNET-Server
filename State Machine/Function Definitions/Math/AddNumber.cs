﻿using System.Diagnostics;

namespace Avalon
{
    public class AddNumber : FunctionDefinition
    {

        public AddNumber()
        {
            Setup();
            DefineFunction();
        }

        void Setup()
        {
            ExpectedParameters = new Dictionary<string, ReferenceTuple>()
            {
                { "a", new ReferenceTuple(StateMachineVariableType.Decimal, false) },
                { "b", new ReferenceTuple(StateMachineVariableType.Decimal, false) },
                { "out", new ReferenceTuple(StateMachineVariableType.Decimal, false, VariableIO.Out) }
            };
        }

        protected override void DefineFunction()
        {
            Name = nameof(AddNumber);
            Function = () =>
            {
                var a = Parameters["a"].GetDecimal();
                var b = Parameters["b"].GetDecimal();

                Debug.WriteLine($"{a} + {b} = {a + b}");

                Parameters["out"].SetValue(a + b);

                return 1;
            };
        }

    }
}

using Avalon;

namespace Avalon
{
    public interface IVariableType
    {

        public StateMachineVariableType Type { get; set; }

        public VariableIO IO { get; set; }

        public string Key { get; set; }
    }
}

using ValkyrieFSMCore;

namespace ValkyrieFSMCore
{
    /// <summary>
    /// The type of the key.
    /// </summary>
    public interface IVariableSignature
    {
        /// <summary>
        /// The type of variable
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Is the Variable an input, output or both?
        /// </summary>
        public VariableIO IO { get; set; }

        /// <summary>
        /// The KeyName of the variable
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// This is the description of the variable
        /// </summary>
        public string Description { get; set; }
    }
}

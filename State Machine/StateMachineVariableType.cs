namespace ValkyrieFSMCore
{
    /// <summary>
    /// The type of the key.
    /// </summary>
    public enum StateMachineVariableType
    {
        Text, //text
        Decimal, //decimal
        Integer, //int
        YesNo, //boolean

        ListText, //list of text
        ListDecimal, //list of decimal
        ListInteger, //list of int
        ListYesNo, //list of boolean

        Object, //object

        Custom //custom
    }
}

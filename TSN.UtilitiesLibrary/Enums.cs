namespace TSN.UtilitiesLibrary
{
    public enum StringStates : byte
    {
        Valued = 0,
        Empty = 1,
        Null = 2
    }

    [Flags] public enum CharacterKinds : byte
    {
        None = 0,
        Other = 1,
        WhiteSpace = 2,
        Letter = 4,
        Digit = 8,
        AlphaNumeric = Letter |  Digit,
        All = Other | WhiteSpace | AlphaNumeric
    }
}
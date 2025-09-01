namespace TSN.UtilitiesLibrary.Exceptions
{
    public class UnexpectedObjectTypeException : Exception
    {
        public UnexpectedObjectTypeException(string? message, string? objName, Type? unexpectedType, Exception? innerException)
            : base(message ?? FormatMessage(objName, unexpectedType), innerException)
        {
            _objectName = objName?.TrimDeeper();
            _unexpectedType = unexpectedType ?? typeof(object);
        }
        public UnexpectedObjectTypeException(string message, string objName, Type unexpectedType) : this(message, objName, unexpectedType, null) { }
        public UnexpectedObjectTypeException() : this(null, null, null, null) { }
        public UnexpectedObjectTypeException(string objName) : this(null, objName, null, null) { }
        public UnexpectedObjectTypeException(string objName, Exception innerException) : this(null, objName, null, innerException) { }
        public UnexpectedObjectTypeException(Type unexpectedType) : this(null, null, unexpectedType, null) { }
        public UnexpectedObjectTypeException(Type unexpectedType, Exception innerException) : this(null, null, unexpectedType, innerException) { }
        public UnexpectedObjectTypeException(string objName, Type unexpectedType) : this(null, objName, unexpectedType, null) { }
        public UnexpectedObjectTypeException(string objName, Type unexpectedType, Exception innerException) : this(null, objName, unexpectedType, innerException) { }
        public UnexpectedObjectTypeException(Exception innerException) : this(null, null, null, innerException) { }


        private const string _messageFormat1 = "Object named {0} was of type {1}, which was unexpected.";
        private const string _messageFormat2 = "Object was of type {0}, which was unexpected.";
        private const string _messageFormat3 = "Object named {0} was of an unexpected type.";
        private const string _messageFormat4 = "Object was of an unexpected type.";

        private readonly string? _objectName;
        private readonly Type? _unexpectedType;

        public string? ObjectName => _objectName;
        public Type? UnexpectedType => _unexpectedType;



        private static string FormatMessage(string? o = null, Type? t = null)
        {
            if (o.TryToTrimDeeper(out var s) != StringStates.Valued)
                return t == null ? _messageFormat4 : string.Format(_messageFormat2, t.Name);
            return t == null ? string.Format(_messageFormat3, s) : string.Format(_messageFormat1, s, t.Name);
        }
    }
}
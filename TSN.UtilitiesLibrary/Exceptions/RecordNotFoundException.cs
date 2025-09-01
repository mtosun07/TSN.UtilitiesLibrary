namespace TSN.UtilitiesLibrary.Exceptions
{
    public class RecordNotFoundException : Exception
    {
        public RecordNotFoundException(string? message, string? entity, string? identifier, Exception? innerException)
            : base(message ?? FormatMessage(entity, identifier), innerException)
        {
            _entity = entity ?? string.Empty;
            _identifier = identifier ?? string.Empty;
        }
        public RecordNotFoundException() : this(null, null, null, null) { }
        public RecordNotFoundException(string? message) : this(message, null, null, null) { }
        public RecordNotFoundException(string? message, string? entity, string? identifier) : this(message, entity, identifier, null) { }
        public RecordNotFoundException(string? message, Exception? innerException) : this(message, null, null, innerException) { }
        public RecordNotFoundException(string? entity, string? identifier) : this(null, entity, identifier, null) { }
        public RecordNotFoundException(string? entity, string? identifier, Exception? innerException) : this(null, entity, identifier, innerException) { }


        private const string _messageFormat1 = "Record was not found in entity.";
        private const string _messageFormat2 = "Record was not found in entity of {0}.";
        private const string _messageFormat3 = "Record ({0}) was not found in entity.";
        private const string _messageFormat4 = "Record ({0}) was not found in entity of {1}.";

        private readonly string _entity;
        private readonly string _identifier;

        public string Entity => _entity;
        public string Identifier => _identifier;



        private static string FormatMessage(string? entity = null, string? identifier = null) => entity == null ? (identifier == null ? _messageFormat1 : string.Format(_messageFormat3, identifier)) : (identifier == null ? string.Format(_messageFormat2, entity) : string.Format(_messageFormat4, identifier, entity));
    }
}

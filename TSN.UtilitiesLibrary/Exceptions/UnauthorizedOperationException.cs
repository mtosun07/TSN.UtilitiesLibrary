using System.Collections.ObjectModel;

namespace TSN.UtilitiesLibrary.Exceptions
{
    public class UnauthorizedOperationException : InvalidOperationException
    {
        public UnauthorizedOperationException(string? message, IEnumerable<string>? reasons, Exception? innerException)
            : base(message ?? _message, innerException)
        {
            _reasons = new ReadOnlyCollection<string>(reasons == null ? Array.Empty<string>() : reasons.ToArray());
        }
        public UnauthorizedOperationException() : this(null, null, null) { }
        public UnauthorizedOperationException(string? message) : this(message, null, null) { }
        public UnauthorizedOperationException(string? message, IEnumerable<string>? reasons) : this(message, reasons, null) { }
        public UnauthorizedOperationException(string? message, Exception? innerException) : this(message, null, innerException) { }
        public UnauthorizedOperationException(IEnumerable<string>? reasons) : this(null, reasons, null) { }
        public UnauthorizedOperationException(IEnumerable<string>? reasons, Exception? innerException) : this(null, reasons, innerException) { }
        public UnauthorizedOperationException(Exception? innerException) : this(null, null, innerException) { }


        private const string _message = "Operation was unauthorized.";

        private readonly IReadOnlyCollection<string> _reasons;

        public IReadOnlyCollection<string> Reasons => _reasons;
    }
}
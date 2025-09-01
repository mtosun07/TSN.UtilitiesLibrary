namespace TSN.UtilitiesLibrary.Exceptions
{
    public class InvalidRequestException : InvalidOperationException
    {
        public InvalidRequestException(string? message, object? requestInfo, Exception? innerException)
            : base(message ?? _message, innerException)
        {
            _requestInfo = requestInfo;
        }
        public InvalidRequestException() : this(null, null, null) { }
        public InvalidRequestException(string? message) : this(message, null, null) { }
        public InvalidRequestException(string? message, object? requestInfo) : this(message, requestInfo, null) { }
        public InvalidRequestException(string? message, Exception? innerException) : this(message, null, innerException) { }
        public InvalidRequestException(object? requestInfo) : this(null, requestInfo, null) { }
        public InvalidRequestException(object? requestInfo, Exception? innerException) : this(null, requestInfo, innerException) { }
        public InvalidRequestException(Exception innerException) : this(null, null, innerException) { }


        private const string _message = "Request was invalid to operate.";

        private readonly object? _requestInfo;

        public object? RequestInfo => _requestInfo;
    }
}
namespace Cabby.Client.Engine.Exceptions
{
    internal class InvalidWebSocketMessageTypeException : Exception
    {
        public string MessageType { get; }

        public InvalidWebSocketMessageTypeException(string messageType)
            : base($"Message type '{messageType}' is null or could not be deserialized.")
        {
            MessageType = messageType;
        }

        public InvalidWebSocketMessageTypeException(string messageType, Exception innerException)
            : base($"Message type '{messageType}' is null or could not be deserialized.", innerException)
        {
            MessageType = messageType;
        }
    }
}

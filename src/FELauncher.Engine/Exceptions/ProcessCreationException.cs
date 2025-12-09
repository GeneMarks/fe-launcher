namespace FELauncher.Engine.Exceptions
{
    internal sealed class ProcessCreationException : Exception
    {
        public ProcessCreationException() { }
        public ProcessCreationException(string message)
            : base(message) { }
        public ProcessCreationException(string message,  Exception innerException)
            : base(message, innerException) { }
    }
}

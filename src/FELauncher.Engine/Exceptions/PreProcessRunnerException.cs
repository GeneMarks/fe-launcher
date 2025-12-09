namespace FELauncher.Engine.Exceptions
{
    internal sealed class PreProcessRunnerException : Exception
    {
        public PreProcessRunnerException() { }
        public PreProcessRunnerException(string message)
            : base(message) { }
        public PreProcessRunnerException(string message,  Exception innerException)
            : base(message, innerException) { }
    }
}

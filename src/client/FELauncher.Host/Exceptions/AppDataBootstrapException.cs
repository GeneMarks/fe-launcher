namespace FELauncher.Host.Exceptions
{
    internal sealed class AppDataBootstrapException : Exception
    {
        public AppDataBootstrapException() { }
        public AppDataBootstrapException(string message)
            : base(message) { }
        public AppDataBootstrapException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}

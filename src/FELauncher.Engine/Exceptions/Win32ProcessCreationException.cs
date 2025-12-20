namespace FELauncher.Engine.Exceptions
{
    internal class Win32ProcessCreationException : Exception
    {
        public Win32ProcessCreationException() { }
        public Win32ProcessCreationException(string message)
            : base(message) { }
        public Win32ProcessCreationException(string message,  Exception innerException)
            : base(message, innerException) { }
    }
}

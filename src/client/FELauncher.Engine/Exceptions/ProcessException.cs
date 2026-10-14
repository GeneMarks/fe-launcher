using System.ComponentModel;

namespace FELauncher.Engine.Exceptions
{
    internal class ProcessException : Exception
    {
        public ProcessException() { }
        public ProcessException(string message)
            : base(message) { }
        public ProcessException(string message,  Win32Exception innerWin32Exception)
            : base(message, innerWin32Exception) { }
    }
}

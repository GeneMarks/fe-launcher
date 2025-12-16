using System.ComponentModel;

namespace FELauncher.Engine.Exceptions
{
    internal class Win32ProcessException : Exception
    {
        public Win32ProcessException() { }
        public Win32ProcessException(string message)
            : base(message) { }
        public Win32ProcessException(string message,  Win32Exception innerWin32Exception)
            : base(message, innerWin32Exception) { }
    }
}

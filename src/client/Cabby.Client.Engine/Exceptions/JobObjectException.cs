using System.ComponentModel;

namespace Cabby.Client.Engine.Exceptions
{
    internal class JobObjectException : Exception
    {
        public JobObjectException() { }
        public JobObjectException(string message)
            : base(message) { }
        public JobObjectException(string message,  Win32Exception innerWin32Exception)
            : base(message, innerWin32Exception) { }
    }
}

using System.ComponentModel;

namespace Cabby.Client.Engine.Exceptions
{
    internal class WinApiException : Exception
    {
        public WinApiException() { }
        public WinApiException(string message)
            : base(message) { }
        public WinApiException(string message,  Win32Exception innerWin32Exception)
            : base(message, innerWin32Exception) { }
    }
}

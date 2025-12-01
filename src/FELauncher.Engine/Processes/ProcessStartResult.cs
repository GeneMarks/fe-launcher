namespace FELauncher.Engine.Processes
{
    public sealed class ProcessStartResult
    {
        public bool Success { get; }
        public ProcessHandle? Handle { get; }
        public string? Msg { get; }

        private ProcessStartResult(
            bool success,
            ProcessHandle? handle,
            string? msg)
        {
            Success = success;
            Handle = handle;
            Msg = msg;
        }

        public static ProcessStartResult Ok(ProcessHandle handle)
            => new(true, handle, null);

        public static ProcessStartResult Fail(string? msg)
            => new(false, null, msg);
    }
}

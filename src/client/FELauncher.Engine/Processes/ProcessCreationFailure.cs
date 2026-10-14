namespace FELauncher.Engine.Processes
{
    internal enum ProcessCreationFailureReason
    {
        EmptyPath,
        InvalidFileExt,
        FileNotPresent,
    }

    internal sealed record ProcessCreationFailure(ProcessCreationFailureReason Reason, string? FileName = null);
}

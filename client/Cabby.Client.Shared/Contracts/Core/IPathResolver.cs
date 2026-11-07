namespace Cabby.Client.Shared.Contracts.Core
{
    public interface IPathResolver
    {
        public string ResolvePath(string? path);
        public bool IsAbsolute(string path);
    }
}
namespace FELauncher.Engine.IO
{
	public interface IPathResolver
	{
		string ResolvePath(string? path);
		bool IsAbsolute(string path);
	}
}

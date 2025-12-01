namespace FELauncher.Engine.IO
{
	public interface IPathResolver
	{
		/// <summary>
		/// If path is relative, add application directory to base.
		/// Return resolved absolute path.
		/// </summary>
		string ResolvePath(string? path);
		bool IsAbsolute(string path);
	}
}

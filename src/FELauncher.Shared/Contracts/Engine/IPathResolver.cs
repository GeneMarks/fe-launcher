namespace FELauncher.Shared.Contracts.Engine
{
	public interface IPathResolver
	{
		/// <summary>
		/// Resolves relative paths by adding application directory to base.
		/// </summary>
		/// <returns>A resolved absolute path.</returns>
		string ResolvePath(string? path);
	}
}

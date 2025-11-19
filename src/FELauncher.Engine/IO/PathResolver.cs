namespace FELauncher.Engine.IO
{
	public class PathResolver : IPathResolver
	{
		private readonly string _basePath;

		public PathResolver()
		{
			_basePath = AppContext.BaseDirectory;
		}

		public string ResolvePath(string? path)
		{
			if (string.IsNullOrEmpty(path))
				return string.Empty;

			if (IsAbsolute(path))
				return path;

			return Path.GetFullPath(Path.Combine(_basePath, path));
		}

		public bool IsAbsolute(string path)
			=> Path.IsPathFullyQualified(path);
	}
}

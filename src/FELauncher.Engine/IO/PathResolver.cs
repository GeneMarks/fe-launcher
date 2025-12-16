using FELauncher.Engine.IO.Logging;
using Microsoft.Extensions.Logging;

namespace FELauncher.Engine.IO
{
	internal sealed class PathResolver(ILogger<PathResolver> logger)
	{
        private readonly string _basePath = AppContext.BaseDirectory;

		/// <summary>
		/// If path is relative, add application directory to base.
		/// Return resolved absolute path.
		/// </summary>
        public string ResolvePath(string? path)
		{
			if (string.IsNullOrEmpty(path))
			{
				logger.PathNullOrEmpty();
				return String.Empty;
			}

			if (IsAbsolute(path))
			{
				logger.PathAbsolute(path);
				return path;
			}

			var resolvedPath = Path.GetFullPath(Path.Combine(_basePath, path));

			logger.ResolvedResult(resolvedPath);
			return resolvedPath;
		}

		public static bool IsAbsolute(string path)
			=> Path.IsPathFullyQualified(path);
	}
}

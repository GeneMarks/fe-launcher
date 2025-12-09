using Microsoft.Extensions.Logging;

namespace FELauncher.Engine.IO
{
	internal sealed class PathResolver : IPathResolver
	{
		private readonly ILogger<PathResolver> _logger;
		private readonly string _basePath;

		public PathResolver(ILogger<PathResolver> logger)
		{
			_logger = logger;
			_basePath = AppContext.BaseDirectory;
		}

		public string ResolvePath(string? path)
		{
			if (string.IsNullOrEmpty(path))
			{
				_logger.PathNullOrEmpty();
				return String.Empty;
			}

			if (IsAbsolute(path))
			{
				_logger.PathAbsolute(path);
				return path;
			}

			var resolvedPath = Path.GetFullPath(Path.Combine(_basePath, path));

			_logger.ResolvedResult(resolvedPath);
			return resolvedPath;
		}

		public bool IsAbsolute(string path)
			=> Path.IsPathFullyQualified(path);
	}
}

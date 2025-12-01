using FELauncher.Engine.Logging;
using Microsoft.Extensions.Logging;

namespace FELauncher.Engine.IO
{
	public sealed class PathResolver : IPathResolver
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
				_logger.IO_ResolverPathNullOrEmpty();
				return String.Empty;
			}

			if (IsAbsolute(path))
			{
				_logger.IO_ResolverPathAbsolute(path);
				return path;
			}

			var resolvedPath = Path.GetFullPath(Path.Combine(_basePath, path));

			_logger.IO_ResolverResult(resolvedPath);
			return resolvedPath;
		}

		public bool IsAbsolute(string path)
			=> Path.IsPathFullyQualified(path);
	}
}

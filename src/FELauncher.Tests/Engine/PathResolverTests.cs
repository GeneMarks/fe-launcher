using FELauncher.Engine.IO;
using Microsoft.Extensions.Logging.Abstractions;

namespace FELauncher.Tests.Engine
{
    public sealed class PathResolverTests
    {
        private readonly PathResolver _sut = new(NullLogger<PathResolver>.Instance);

        [Fact]
        public void ResolvePath_NullPath_ReturnsEmptyString()
        {
            var result = _sut.ResolvePath(null);

            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void ResolvePath_EmptyPath_ReturnsEmptyString()
        {
            var result = _sut.ResolvePath(string.Empty);

            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void ResolvePath_AbsolutePath_ReturnsSamePath()
        {
            var absolutePath = Path.GetFullPath(Path.Combine(Path.GetTempPath(), "app.exe"));

            var result = _sut.ResolvePath(absolutePath);

            Assert.Equal(absolutePath, result);
        }

        [Fact]
        public void ResolvePath_RelativePath_ReturnsResolvedPathWithBaseDirectory()
        {
            var relativePath = @".\test\app.exe";
            var expected = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, relativePath));

            var result = _sut.ResolvePath(relativePath);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(@"C:\test\app.exe", true)]
        [InlineData(@"test\app.exe", false)]
        [InlineData(@".\app.exe", false)]
        public void IsAbsolute_ReturnsExpectedResult(string path, bool expected)
        {
            var result = PathResolver.IsAbsolute(path);

            Assert.Equal(expected, result);
        }
    }
}

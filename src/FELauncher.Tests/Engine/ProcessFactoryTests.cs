using FELauncher.Engine.IO;
using FELauncher.Engine.Processes;
using Microsoft.Extensions.Logging.Abstractions;

namespace FELauncher.Tests.Engine
{
    public sealed class ProcessFactoryTests : IDisposable
    {
        private readonly string _tempDirectory = Path.Combine(
            Path.GetTempPath(),
            "FELauncher.Tests",
            Guid.NewGuid().ToString("N"));

        public ProcessFactoryTests()
        {
            Directory.CreateDirectory(_tempDirectory);
        }

        private ProcessFactory CreateSut()
        {
            var pathResolver = new PathResolver(NullLogger<PathResolver>.Instance);

            return new ProcessFactory(
                NullLogger<ProcessFactory>.Instance,
                NullLogger<FELauncher.Engine.Processes.Process>.Instance,
                pathResolver);
        }

        private string CreateTempFile(string fileName)
        {
            var path = Path.Combine(_tempDirectory, fileName);
            File.WriteAllText(path, "test");
            return path;
        }

        [Fact]
        public void TryCreate_EmptyPath_ReturnsFalse()
        {
            var sut = CreateSut();
            var path = String.Empty;

            var success = sut.TryCreate(path, null, out var process, out var failure);

            Assert.False(success);
            Assert.Null(process);
            Assert.NotNull(failure);
            Assert.Equal(ProcessCreationFailureReason.EmptyPath, failure.Reason);
            Assert.Null(failure.FileName);
        }

        [Fact]
        public void TryCreate_NullPath_ReturnsFalse()
        {
            var sut = CreateSut();
            string? path = null;

            var success = sut.TryCreate(path, null, out var process, out var failure);

            Assert.False(success);
            Assert.Null(process);
            Assert.NotNull(failure);
            Assert.Equal(ProcessCreationFailureReason.EmptyPath, failure.Reason);
            Assert.Null(failure.FileName);
        }

        [Fact]
        public void TryCreate_ExtensionNotExe_ReturnsFalse()
        {
            var sut = CreateSut();
            var path = "test.txt";

            var success = sut.TryCreate(path, null, out var process, out var failure);

            Assert.False(success);
            Assert.Null(process);
            Assert.NotNull(failure);
            Assert.Equal(ProcessCreationFailureReason.InvalidFileExt, failure.Reason);
            Assert.Equal(path, failure.FileName);
        }

        [Fact]
        public void TryCreate_MissingFile_ReturnsFalse()
        {
            var sut = CreateSut();
            var filename = "missing.exe";
            var path = Path.Combine(_tempDirectory, filename);

            var success = sut.TryCreate(path, null, out var process, out var failure);

            Assert.False(success);
            Assert.Null(process);
            Assert.NotNull(failure);
            Assert.Equal(ProcessCreationFailureReason.FileNotPresent, failure.Reason);
            Assert.Equal(filename, failure.FileName);
        }

        [Fact]
        public void TryCreate_ExistingExeWithoutArgs_ReturnsTrue()
        {
            var sut = CreateSut();
            var filename = "frontend.exe";
            var prettyName = "frontend";
            var path = CreateTempFile(filename);

            var success = sut.TryCreate(path, null, out var process, out var failure);

            Assert.True(success);
            Assert.NotNull(process);
            Assert.Null(failure);
            Assert.Equal(path, process.PathWithArgs);
            Assert.Equal(prettyName, process.PrettyName);
        }

        [Fact]
        public void TryCreate_ExistingExeWithArgs_ReturnsTrue()
        {
            var sut = CreateSut();
            var filename = "frontend.exe";
            var prettyName = "frontend";
            var args = "--flag test";
            var path = CreateTempFile(filename);

            var success = sut.TryCreate(path, args, out var process, out var failure);

            Assert.True(success);
            Assert.NotNull(process);
            Assert.Null(failure);
            Assert.Equal($"{path} {args}", process.PathWithArgs);
            Assert.Equal(prettyName, process.PrettyName);
        }

        [Fact]
        public void TryCreate_ExistingExeWithEmptyArgs_TreatAsNoArgs()
        {
            var sut = CreateSut();
            var filename = "frontend.exe";
            var args = String.Empty;
            var path = CreateTempFile(filename);

            var success = sut.TryCreate(path, args, out var process, out var failure);

            Assert.True(success);
            Assert.NotNull(process);
            Assert.Null(failure);
            Assert.Equal(path, process.PathWithArgs);
        }

        public void Dispose()
        {
            if (Directory.Exists(_tempDirectory))
            {
                Directory.Delete(_tempDirectory, true);
            }
        }
    }
}

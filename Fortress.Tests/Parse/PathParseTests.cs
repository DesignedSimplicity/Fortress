using System;
using Fortress.Core.Common;

namespace Fortress.Tests.Parse
{
    public class PathParseTests
	{
		[Test]
		public void TestGetPathsEmpty()
		{
			var uri = @"";
			var paths = PathUtils.GetPaths(uri);
			Assert.That(paths.Length, Is.EqualTo(0));
		}

		[Test]
		public void TestGetPathsAtRoot()
		{
			var uri = @"C:";
			var paths = PathUtils.GetPaths(uri);
			Assert.That(paths[0], Is.EqualTo("C:"));
		}
		
		[Test]
		public void TestGetPathsAtRootWithSlash()
		{
			var uri = @"C:\";
			var paths = PathUtils.GetPaths(uri);
			Assert.That(paths[0], Is.EqualTo("C:"));
		}

		[Test]
		public void TestGetPathsAtRootWithExtraSlashs()
		{
			var uri = @"C:\\";
			var paths = PathUtils.GetPaths(uri);
			Assert.That(paths[0], Is.EqualTo("C:"));
		}

		[Test]
		public void TestGetPathsForDirectory()
		{
			var uri = @"C:\RootDir";
			var paths = PathUtils.GetPaths(uri);
			Assert.That(paths[0], Is.EqualTo("C:"));
			Assert.That(paths[1], Is.EqualTo("RootDir"));
		}

		[Test]
		public void TestGetPathsForDirectoryWithSlash()
		{
			var uri = @"C:\RootDir\";
			var paths = PathUtils.GetPaths(uri);
			Assert.That(paths[0], Is.EqualTo("C:"));
			Assert.That(paths[1], Is.EqualTo("RootDir"));
		}

		[Test]
		public void TestGetPathsForDirectoryWithDoubleSlashes()
		{
			var uri = @"C:\\RootDir\\";
			var paths = PathUtils.GetPaths(uri);
			Assert.That(paths[0], Is.EqualTo("C:"));
			Assert.That(paths[1], Is.EqualTo("RootDir"));
		}

		[Test]
		public void TestGetPathsForSubDirectory()
		{
			var uri = @"C:\\RootDir\\SubDir";
			var paths = PathUtils.GetPaths(uri);
			Assert.That(paths[0], Is.EqualTo("C:"));
			Assert.That(paths[1], Is.EqualTo("RootDir"));
			Assert.That(paths[2], Is.EqualTo("SubDir"));
		}

		[Test]
		public void TestGetPathsForFileInSubDirectory()
		{
			var uri = @"C:\RootDir\SubDir\\FileName.ext";
			var paths = PathUtils.GetPaths(uri);
			Assert.That(paths[0], Is.EqualTo("C:"));
			Assert.That(paths[1], Is.EqualTo("RootDir"));
			Assert.That(paths[2], Is.EqualTo("SubDir"));
			Assert.That(paths[3], Is.EqualTo("FileName.ext"));
		}
	}
}

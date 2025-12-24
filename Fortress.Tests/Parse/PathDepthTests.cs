using System;
using Fortress.Core.Common;

namespace Fortress.Tests.Parse
{
    public class PathDepthTests
	{
		[Test]
		public void TestGetDepthInvalid()
		{
			var uri = @"";
			var depth = PathUtils.GetDepth(uri);
			Assert.That(depth, Is.EqualTo(-1));
		}

		[Test]
		public void TestGetDepthAtRoot()
		{
			var uri = @"C:";
			var depth = PathUtils.GetDepth(uri);
			Assert.That(depth, Is.EqualTo(0));
		}

		[Test]
		public void TestGetDepthAtRootWithSlash()
		{
			var uri = @"C:\";
			var depth = PathUtils.GetDepth(uri);
			Assert.That(depth, Is.EqualTo(0));
		}

		[Test]
		public void TestGetDepthAtRootWithExtraSlashs()
		{
			var uri = @"C:\\";
			var depth = PathUtils.GetDepth(uri);
			Assert.That(depth, Is.EqualTo(0));
		}

		[Test]
		public void TestGetDepthForDirectory()
		{
			var uri = @"C:\RootDir";
			var depth = PathUtils.GetDepth(uri);
			Assert.That(depth, Is.EqualTo(1));
		}

		[Test]
		public void TestGetDepthForDirectoryWithSlash()
		{
			var uri = @"C:\RootDir\";
			var depth = PathUtils.GetDepth(uri);
			Assert.That(depth, Is.EqualTo(1));
		}

		[Test]
		public void TestGetDepthForDirectoryWithDoubleSlashes()
		{
			var uri = @"C:\\RootDir\\";
			var depth = PathUtils.GetDepth(uri);
			Assert.That(depth, Is.EqualTo(1));
		}

		[Test]
		public void TestGetDepthForSubDirectory()
		{
			var uri = @"C:\\RootDir\\SubDir";
			var depth = PathUtils.GetDepth(uri);
			Assert.That(depth, Is.EqualTo(2));
		}

		[Test]
		public void TestGetDepthForFileInSubDirectory()
		{
			var uri = @"C:\RootDir\SubDir\\FileName.ext";
			var depth = PathUtils.GetDepth(uri);
			Assert.That(depth, Is.EqualTo(3));
		}
	}
}

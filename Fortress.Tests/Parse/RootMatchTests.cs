using System;
using Fortress.Core.Common;

namespace Fortress.Tests.Parse
{
    public class RootMatchTests
	{
		[Test]
		public void TestHasSameRootAtRoot()
		{
			var uri1 = @"C:";
			var uri2 = @"C:";
			var same = PathUtils.HasSameRoot(uri1, uri2);
			Assert.IsTrue(same);
		}

		[Test]
		public void TestHasSameRootAtRootWithSlash1()
		{
			var uri1 = @"C:\";
			var uri2 = @"C:";
			var same = PathUtils.HasSameRoot(uri1, uri2);
			Assert.IsTrue(same);
		}

		[Test]
		public void TestHasSameRootAtRootWithSlash2()
		{
			var uri1 = @"C:";
			var uri2 = @"C:\";
			var same = PathUtils.HasSameRoot(uri1, uri2);
			Assert.IsTrue(same);
		}

		[Test]
		public void TestHasSameRootAtRootWithSlash1And2()
		{
			var uri1 = @"C:\";
			var uri2 = @"C:\";
			var same = PathUtils.HasSameRoot(uri1, uri2);
			Assert.IsTrue(same);
		}

		[Test]
		public void TestHasSameRootAtRootWithDirectory1()
		{
			var uri1 = @"C:\Directory1";
			var uri2 = @"C:\";
			var same = PathUtils.HasSameRoot(uri1, uri2);
			Assert.IsTrue(same);
		}

		[Test]
		public void TestHasSameRootAtRootWithDirectory2()
		{
			var uri1 = @"C:\";
			var uri2 = @"C:\Directory2";
			var same = PathUtils.HasSameRoot(uri1, uri2);
			Assert.IsTrue(same);
		}

		[Test]
		public void TestHasSameRootAtRootWithDirectory1And2()
		{
			var uri1 = @"C:\Directory1";
			var uri2 = @"C:\Directory2";
			var same = PathUtils.HasSameRoot(uri1, uri2);
			Assert.IsTrue(same);
		}

		[Test]
		public void TestHasSameRootAtRootWithSubDirectories()
		{
			var uri1 = @"C:\Directory1\Subdirectory";
			var uri2 = @"C:\Directory2\Subdirectory";
			var same = PathUtils.HasSameRoot(uri1, uri2);
			Assert.IsTrue(same);
		}

		[Test]
		public void TestHasSameRootNotSameRootAtRoot()
		{
			var uri1 = @"C:\";
			var uri2 = @"D:\Directory2\Subdirectory";
			var same = PathUtils.HasSameRoot(uri1, uri2);
			Assert.That(same, Is.False);
		}

		[Test]
		public void TestHasSameRootNotSameRoot()
		{
			var uri1 = @"C:\Directory1\Subdirectory";
			var uri2 = @"D:\Directory2\Subdirectory";
			var same = PathUtils.HasSameRoot(uri1, uri2);
			Assert.That(same, Is.False);
		}
	}
}

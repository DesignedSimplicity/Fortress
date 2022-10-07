using System;
using Fortress.Core.Common;

namespace Fortress.Tests.Parse
{
    public class RootParseTests
	{
		[Test]
		public void TestParseCommonPathRootInvalid()
		{
			var uri1 = @"C:";
			var uri2 = @"D:";
			var path = PathUtils.GetCommonPath(uri1, uri2);
			Assert.That(path, Is.EqualTo(@""));
		}

		[Test]
		public void TestParseCommonPathRootInvalidWithDirectory()
		{
			var uri1 = @"C:\Same";
			var uri2 = @"D:\Same";
			var path = PathUtils.GetCommonPath(uri1, uri2);
			Assert.That(path, Is.EqualTo(@""));
		}

		[Test]
		public void TestParseCommonPathRoot()
		{
			var uri1 = @"C:";
			var uri2 = @"C:";
			var path = PathUtils.GetCommonPath(uri1, uri2);
			Assert.That(path, Is.EqualTo(@"C:"));
		}

		[Test]
		public void TestParseCommonPathRootWithDirectory1()
		{
			var uri1 = @"C:\Directory1";
			var uri2 = @"C:";
			var path = PathUtils.GetCommonPath(uri1, uri2);
			Assert.That(path, Is.EqualTo(@"C:"));
		}

		[Test]
		public void TestParseCommonPathRootWithDirectory2()
		{
			var uri1 = @"C:\";
			var uri2 = @"C:\Directory2";
			var path = PathUtils.GetCommonPath(uri1, uri2);
			Assert.That(path, Is.EqualTo(@"C:"));
		}

		[Test]
		public void TestParseCommonPathRootWithDirectory1And2()
		{
			var uri1 = @"C:\Directory1";
			var uri2 = @"C:\Directory2\\";
			var path = PathUtils.GetCommonPath(uri1, uri2);
			Assert.That(path, Is.EqualTo(@"C:"));
		}

		[Test]
		public void TestParseCommonPathRootWithCommonDirectory()
		{
			var uri1 = @"C:\Directory\Sub1";
			var uri2 = @"C:\Directory\Sub2\";
			var path = PathUtils.GetCommonPath(uri1, uri2);
			Assert.That(path, Is.EqualTo(@"C:\Directory"));
		}

		[Test]
		public void TestParseCommonPathRootWithCommonSubDirectory()
		{
			var uri1 = @"C:\Directory\Sub\Child1";
			var uri2 = @"C:\Directory\Sub\\Child2";
			var path = PathUtils.GetCommonPath(uri1, uri2);
			Assert.That(path, Is.EqualTo(@"C:\Directory\Sub"));
		}

		[Test]
		public void TestParseCommonPathRootWithCommonRootDirectory()
		{
			var uri1 = @"C:\Directory\Sub1\Child1";
			var uri2 = @"C:\Directory\Sub2\Child2\";
			var path = PathUtils.GetCommonPath(uri1, uri2);
			Assert.That(path, Is.EqualTo(@"C:\Directory"));
		}

		[Test]
		public void TestParseCommonPathRootWithCommonRootDirectoryDifferentLevels()
		{
			var uri1 = @"C:\Directory\Sub1\Child1\";
			var uri2 = @"C:\Directory\Sub2\Child2\Grandchild2";
			var path = PathUtils.GetCommonPath(uri1, uri2);
			Assert.That(path, Is.EqualTo(@"C:\Directory"));
		}
	}
}

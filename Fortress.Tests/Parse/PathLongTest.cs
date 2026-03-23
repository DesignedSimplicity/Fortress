using Fortress.Core.Common;
using Fortress.Core.Entities;
using Fortress.Core.Services;

namespace Fortress.Tests.Parse
{
    public class PathLongTest
    {
        [Test]
        public void TestIsMaxPath()
        {
            var uri = @"D:\Temp\LongPathAwareTestThisIsAreallyLongString\LongPathAwareTestThisIsAreallyLongString - Copy\LongPathAwareTestThisIsAreallyLongString - Copy - Copy\LongPathAwareTestThisIsAreallyLongString - Copy - Copy - Copy\LongPathAwareTestThisIsAreallyLongString - Copy - Copy - Copy - Copy";
            var max = PathUtils.IsMaxPath(uri);
            Assert.That(max, Is.True);
        }

        [Test]
        public void TestAccessMaxPath()
        {
            var uri = @"D:\Temp\LongPathAwareTestThisIsAreallyLongString\LongPathAwareTestThisIsAreallyLongString - Copy\LongPathAwareTestThisIsAreallyLongString - Copy - Copy\LongPathAwareTestThisIsAreallyLongString - Copy - Copy - Copy\LongPathAwareTestThisIsAreallyLongString - Copy - Copy - Copy - Copy\TEST.txt";
            var file = new FileInfo(uri);
            Assert.That(file.Exists, Is.True);
        }

        [Test]
        public void TestHashMaxPath()
        {
            var uri = @"D:\Temp\LongPathAwareTestThisIsAreallyLongString\LongPathAwareTestThisIsAreallyLongString - Copy\LongPathAwareTestThisIsAreallyLongString - Copy - Copy\LongPathAwareTestThisIsAreallyLongString - Copy - Copy - Copy\LongPathAwareTestThisIsAreallyLongString - Copy - Copy - Copy - Copy\TEST.txt";
            var file = new FileInfo(uri);
            var hasher = new GetHash();
            var hash = hasher.Calculate(new PatrolFile(file), HashType.Md5);
            Assert.That(hash.HashValue, Is.EqualTo("86862FB0BE4CF19074B4604A0F784746"));
        }
    }
}

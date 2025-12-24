using System.Diagnostics;
using Fortress.Core.Entities;
using Fortress.Core.Services;

var count = 3;
var size = 4 * 1024 * 1024;

var hash = new GetHash(size);

var data = new FileInfo("D:\\Temp\\Small.zip");
var file = new PatrolFile(data);

var time = new Stopwatch();
time.Start();
for (var i = 0; i < count; i++)
{
    Console.Write($"#{i + 1} ");
    var result = hash.Calculate(file, HashType.Md5);
    Console.WriteLine($"Hash: {result.HashValue}");
}
time.Stop();
Console.WriteLine($"Count: {count}");
Console.WriteLine($"Buffer: {size}");
Console.WriteLine($"Elapsed: {time.ElapsedMilliseconds} ms");
//Console.WriteLine($"Speed: {result.BytesPerMillisecond / 1024.0:F2} KB/ms");
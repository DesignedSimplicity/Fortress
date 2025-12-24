using Fortress.Core.Entities;
using System.Diagnostics;
using System.Security.Cryptography;

namespace Fortress.Core.Services;

public enum HashType { Md5, Sha512 }
public record HashResult(HashType HashType, string HashValue, long FileSize, long Milliseconds) {}

public class GetHash
{
	private readonly int _bufferSize = 10 * 1024 * 1024; // 10 MB

    public GetHash(int? bufferSize = null)
	{
        _bufferSize = bufferSize ?? _bufferSize;
    }

    public HashResult Calculate(PatrolFile file, HashType hashType)
    {
        var hashValue = "";
        var s = new Stopwatch();
        s.Start();
        try
        {
            using var fs = new FileStream(file.Uri, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, _bufferSize);
            hashValue = Convert.ToHexString(hashType == HashType.Sha512 ? SHA512.HashData(fs) : MD5.HashData(fs));
            if (hashType == HashType.Sha512)
            {
                file.Sha512 = hashValue;
                file.Sha512Status = HashStatus.Created;
                //file.FirstHashed = file.LastVerified = DateTime.UtcNow;

            }
            else if (hashType == HashType.Md5)
            {
                file.Md5 = hashValue;
                file.Md5Status = HashStatus.Created;
                //file.FirstHashed = file.LastVerified = DateTime.UtcNow;
            }
        }
        finally
        {
            s.Stop();
        }

        return new HashResult(hashType, hashValue, file.Size, s.ElapsedMilliseconds);
    }

    public async Task<HashResult> CalculateAsync(PatrolFile file, HashType hashType)
    {
        var hashValue = "";
        var s = new Stopwatch();
        s.Start();
        try
        {
            using var fs = new FileStream(file.Uri, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, _bufferSize);
            var hash = await (hashType == HashType.Sha512 ? SHA512.HashDataAsync(fs) : MD5.HashDataAsync(fs));
            hashValue = Convert.ToHexString(hash);
            if (hashType == HashType.Sha512)
            {
                file.Sha512 = hashValue;
                file.Sha512Status = HashStatus.Created;
                //file.FirstHashed = file.LastVerified = DateTime.UtcNow;

            }
            else if (hashType == HashType.Md5)
            {
                file.Md5 = hashValue;
                file.Md5Status = HashStatus.Created;
                //file.FirstHashed = file.LastVerified = DateTime.UtcNow;
            }
        }
        finally
        {
            s.Stop();
        }

        return new HashResult(hashType, hashValue, file.Size, s.ElapsedMilliseconds);
    }
}

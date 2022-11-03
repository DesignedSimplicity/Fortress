using Fortress.Core.Entities;
using System.Diagnostics;
using System.Security.Cryptography;

namespace Fortress.Core.Services;

public enum HashType { Md5, Sha512 }
public class HashResult
{
	public HashType HashType { get; private set; }
	public string HashValue { get; private set; }

	public long Milliseconds { get; private set; }
	public long FileSize { get; private set; }

	public double BytesPerMillisecond => FileSize * 1.0 / (Milliseconds + 1.0);

	public HashResult(HashType hashType, string hashValue, long size, long ms) {
		HashType = hashType;
		HashValue = hashValue;
		FileSize = size;
		Milliseconds = ms;
	}
}

public class GetHash : IDisposable
{
	private readonly HashAlgorithm _md5;
	private readonly HashAlgorithm _sha512;

	public GetHash()
	{
		_md5 = MD5.Create();
		_sha512 = SHA512.Create();
	}

	public HashResult Calculate(PatrolFile file, HashType hashType)
	{
		var hashValue = "";
		var s = new Stopwatch();
		s.Start();
		try
		{
			hashValue = Calculate(file.Uri, hashType);
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

	public string Calculate(string filePath, HashType hashType)
	{
		using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
		return Calculate(fs, hashType);
	}

	public string Calculate(Stream s, HashType hashType)
	{
		var hash = GetHashAlgorithm(hashType).ComputeHash(s);
		return BitConverter.ToString(hash).Replace("-", "");
	}

	private HashAlgorithm GetHashAlgorithm(HashType hashType)
	{
		if (hashType == HashType.Sha512)
			return _sha512;
		else
			return _md5;
	}

	public void Dispose()
	{
		_md5.Dispose();
		_sha512.Dispose();
	}
}

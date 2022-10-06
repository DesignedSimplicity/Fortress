using System.Security.Cryptography;

namespace Fortress.Core.Services
{
	public enum HashType { Md5, Sha512 }
	public class CalculateHash : IDisposable
	{
		private readonly HashAlgorithm _md5;
		private readonly HashAlgorithm _sha512;

		public CalculateHash()
		{
			_md5 = MD5.Create();
			_sha512 = SHA512.Create();
		}

		public string GetHash(string filePath, HashType hasher)
		{
			using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			return GetHash(fs, hasher);
		}

		public string GetHash(Stream s, HashType hasher)
		{
			var hash = GetHashAlgorithm(hasher).ComputeHash(s);
			return BitConverter.ToString(hash).Replace("-", "");
		}

		private HashAlgorithm GetHashAlgorithm(HashType hasher)
		{
			if (hasher == HashType.Sha512)
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
}

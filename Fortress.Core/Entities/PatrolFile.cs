namespace Fortress.Core.Entities;

public record PatrolFile
{
	public Guid Guid { get; set; }
	public FileInfo? File { get; set; }
	public string Uri { get; set; }
	public string Name { get; set; }
	public string Extension { get; set; }

	public string DirectoryName { get; set; }

	public long Size { get; set; }

	public DateTime CreatedUtc { get; set; }
	public DateTime? ModifiedUtc { get; set; }

	public string Md5 = string.Empty;
	public string Sha512 = string.Empty;

	public HashStatus Md5Status = HashStatus.Unknown;
	public HashStatus Sha512Status = HashStatus.Unknown;

	public FileStatus Status { get; set; } = FileStatus.Default;

	public PatrolFile(FileInfo file)
	{
		Guid = Guid.CreateVersion7();
		Uri = file.FullName;
		Name = file.Name;
		File = file;
		Size = file.Length;
		Extension = file.Extension;
		DirectoryName = file.Directory?.Name ?? "";

		CreatedUtc = file.CreationTimeUtc;
		ModifiedUtc = file.LastWriteTimeUtc;
	}

	/*
	public PatrolFile(string uri)
	{
		Guid = Guid.CreateVersion7();
		Uri = uri;
		Name = Path.GetFileName(uri);
		Extension = Path.GetExtension(uri);
	}
	*/
}

public enum HashStatus { Unknown = 0, Created = 1, Updated = 2, Verified = 3 }

public enum FileStatus
{
	/// <summary>
	/// Error accessing file
	/// </summary>
	Error = -9,

	/// <summary>
	/// Not found on file system
	/// </summary>
	NotFound = -1,

	/// <summary>
	/// Unspecified state
	/// </summary>
	Default = 0,

	/// <summary>
	/// Confirmed existing
	/// </summary>
	Exists = 1,

	/// <summary>
	/// File name and size matched
	/// </summary>
	Matched = 2,

	/// <summary>
	/// File hash created
	/// </summary>
	Hashed = 3,

	/// <summary>
	/// File hash verified
	/// </summary>
	Verified = 9,
}

public record PatrolFileState
{
	public PatrolFile File { get; set; }
	public Guid StateId { get; set; } = Guid.CreateVersion7();
	public FileStatus Status { get; set; } = FileStatus.Default;
	public DateTime Timestamp { get; set; } = DateTime.UtcNow;
	public string Message { get; set; } = string.Empty;
	public Exception? Exception { get; set; } = null;


	public PatrolFileState(PatrolFile file, FileStatus status = FileStatus.Default, string message = "")
	{
		File = file;
		Status = status;
		Message = message;
	}

	public PatrolFileState(PatrolFile file, Exception ex)
	{
		File = file;
		Status = FileStatus.Error;
		Message = ex.Message;
	}
}

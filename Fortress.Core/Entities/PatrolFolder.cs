using Fortress.Core.Common;

namespace Fortress.Core.Entities;

public record PatrolFolder
{
	public DirectoryInfo? Directory { get; set; }

	public Guid Guid { get; set; }
	public string Uri { get; set; }
	public string Name { get; set; }

	public DateTime CreatedUtc { get; set; }
	public DateTime? ModifiedUtc { get; set; }
	public DateTime? DeclaredUTC { get; set; }
	public DateTime? VerifiedUTC { get; set; }

	public List<PatrolFolder> PatrolFolders { get; set; } = [];
	public List<PatrolFile> PatrolFiles { get; set; } = [];

	public long TotalFileSize => PatrolFiles.Sum(x => x.Size);

	public PatrolFolder(DirectoryInfo dir)
	{
		Guid = Guid.NewGuid();
		Uri = dir.FullName;
		Name = dir.Name;
		Directory = dir;
		CreatedUtc = dir.CreationTimeUtc;
		ModifiedUtc = dir.LastWriteTimeUtc;
	}

	public PatrolFolder(string uri)
	{
		Uri = uri;
		Name = PathUtils.GetParentName(uri);
	}
}

public enum PatrolFolderStatus
{
	/// <summary>
	/// Error accessing folder
	/// </summary>
	Exception = -9,

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
	/// All contents recursively verifed
	/// </summary>
	Verified = 9,
}

public record PatrolFolderState
{
	public PatrolFolder Folder { get; set; }
	public Guid StateId { get; set; } = Guid.NewGuid();
	public PatrolFolderStatus Status { get; set; } = PatrolFolderStatus.Default;
	public DateTime Timestamp { get; set; } = DateTime.UtcNow;
	public string Message { get; set; } = string.Empty;
	public Exception? Exception { get; set; } = null;


	public PatrolFolderState(PatrolFolder folder, PatrolFolderStatus status = PatrolFolderStatus.Default, string message = "")
	{
		Folder = folder;
		Status = status;
		Message = message;
	}

	public PatrolFolderState(PatrolFolder folder, Exception ex)
	{
		Folder = folder;
		Status = PatrolFolderStatus.Exception;
		Message = ex.Message;
	}
}

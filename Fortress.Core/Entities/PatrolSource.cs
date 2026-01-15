namespace Fortress.Core.Entities;

public enum PatrolType { Unknown, FileSystem, PatrolSource }

public record PatrolSource
{
	public Guid Guid { get; private set; }
	public PatrolType PatrolType { get; set; } = PatrolType.Unknown;

	public string SystemName { get; set; } = string.Empty;
	public string PatrolName { get; set; } = string.Empty;
	public string PatrolFileUri { get; set; } = string.Empty;
	public string PatrolFolderUri { get; set; } = string.Empty;

	public PatrolFolder RootFolder { get; set; }		
	public List<PatrolFolder> AllFolders { get; set; }
	public List<PatrolFile> AllFiles { get; set; }

	public TimeSpan ElapsedTime { get; set; }
	public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
	public DateTime? ModifiedUtc { get; set; }

	public PatrolSource(string uri, List<PatrolFolder>? folders = null, List<PatrolFile>? files = null)
	{
		Guid = Guid.CreateVersion7();
		RootFolder = new PatrolFolder(uri);
		AllFolders = folders ?? [];
		AllFiles = files ?? [];
	}

	public PatrolSource(PatrolFolder root, List<PatrolFolder>? folders = null, List<PatrolFile>? files = null)
	{
		Guid = Guid.CreateVersion7();
		RootFolder = root;
		AllFolders = folders ?? [];
		AllFiles = files ?? [];
	}
}

namespace Fortress.Core.Services.Settings;

public abstract class QueryBaseSettings
{
	public StreamWriter? Output;
	public bool StopOnError;
	public bool VerboseLog;
}

public class QueryFilesSettings : QueryBaseSettings
{
	public FileNotify? Notify;
}

public class QueryFoldersSettings : QueryBaseSettings
{
	public FolderNotify? Notify;
}

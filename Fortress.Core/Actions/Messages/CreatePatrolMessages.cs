using Fortress.Core.Entities;
using Fortress.Core.Services;
namespace Fortress.Core.Actions.Messages;

public class CreatePatrolRequest : BaseRequest
{
	public string? DirectoryUri { get; set; }

	public HashType HashType { get; set; }

	public string? SearchFilter { get; set; }

	public bool Recursive { get; set; }

	public bool VerboseLog { get; set; }
}

public class CreatePatrolExecute : BaseExecute
{
	public CreatePatrolRequest Request { get; private set; }

	public string RunName { get; set; } = string.Empty;
	public string SystemName { get; set; } = string.Empty;
	public string SourceFolderUri { get; set; } = string.Empty;

	public List<PatrolFolder> Folders { get; set; } = new();
	public List<PatrolFile> Files { get; set; } = new();

	public CreatePatrolExecute(CreatePatrolRequest request)
	{
		Request = request;
		SystemName = Environment.MachineName;
	}
}

public class CreatePatrolReview : BaseReview
{
	public CreatePatrolExecute Execute { get; private set; }

	public PatrolSource Source { get; set; }

	public CreatePatrolReview(CreatePatrolExecute execute)
	{
		Execute = execute;

		Source = new PatrolSource(execute.Folders.First(), execute.Folders, execute.Files);
		
		Source.SystemName = execute.SystemName;
		Source.PatrolName = execute.RunName;
		
		Source.PatrolType = PatrolType.FileSystem;
		Source.PatrolFileUri = execute.ReportFileUri;
		Source.PatrolFolderUri = execute.SourceFolderUri;

		Source.ElapsedTime = FinishUtc - Execute.StartUtc;
	}
}

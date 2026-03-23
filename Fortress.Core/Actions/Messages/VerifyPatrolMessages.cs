using Fortress.Core.Entities;
using Fortress.Core.Services;
namespace Fortress.Core.Actions.Messages;

public class VerifyPatrolRequest : BaseRequest
{
	public string? DirectoryUri { get; set; }

	public HashType HashType { get; set; }

	public string? SearchFilter { get; set; }

	public bool Recursive { get; set; }

	public bool VerboseLog { get; set; }
}

public class VerifyPatrolExecute : BaseExecute
{
	public VerifyPatrolRequest Request { get; private set; }

	public string RunName { get; set; } = string.Empty;
	public string SystemName { get; set; } = string.Empty;
	public string SourceFolderUri { get; set; } = string.Empty;

	public List<PatrolFolder> Folders { get; set; } = [];
	public List<PatrolFile> Files { get; set; } = [];

	public VerifyPatrolExecute(VerifyPatrolRequest request)
	{
		Request = request;
		SystemName = Environment.MachineName;
	}
}

public class VerifyPatrolReview : BaseReview
{
	public VerifyPatrolExecute Execute { get; private set; }

	public PatrolSource Source { get; set; }

	public VerifyPatrolReview(VerifyPatrolExecute execute)
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

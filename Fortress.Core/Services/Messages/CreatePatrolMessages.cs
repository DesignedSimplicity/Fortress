using Fortress.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Fortress.Core.Services.Messages
{
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

		public List<PatrolFolder> Folders { get; set; } = new List<PatrolFolder>();
		public List<PatrolFile> Files { get; set; } = new List<PatrolFile>();

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
			Source = new PatrolSource(execute.SourceFolderUri, execute.Folders, execute.Files);
			Source.ElapsedTime = FinishUtc - Execute.StartUtc;
		}
	}
}

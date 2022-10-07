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
		public string SourceFolderUri { get; set; } = string.Empty;

		public List<PatrolFolder> Folders { get; set; } = new List<PatrolFolder>();
		public List<PatrolFile> Files { get; set; } = new List<PatrolFile>();

		public CreatePatrolExecute(CreatePatrolRequest request)
		{
			Request = request;
		}
	}
}

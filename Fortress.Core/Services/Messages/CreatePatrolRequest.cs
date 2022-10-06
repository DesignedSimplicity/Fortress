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
	}

	public class CreatePatrolExecute
	{
		public CreatePatrolRequest Request { get; private set; }

		public string RunName { get; set; } = string.Empty;
		public string SourceFolderUri { get; set; } = string.Empty;

		public bool CreateLog { get; set; }
		public bool CreateOutput { get; set; }
		public bool CreateReport { get; set; }

		public string LogFileUri { get; set; } = string.Empty;
		public string OutputFileUri { get; set; } = string.Empty;
		public string ReportFileUri { get; set; } = string.Empty;

		public CreatePatrolExecute(CreatePatrolRequest request)
		{
			Request = request;
		}
	}
}

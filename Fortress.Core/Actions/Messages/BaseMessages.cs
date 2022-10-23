using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fortress.Core.Actions.Messages
{
	public abstract class BaseRequest
	{
		public string? NamePrefix { get; set; }
		public bool StopOnError { get; set; }
		public bool LogOutput { get; set; }
		public bool CreateExport { get; set; }
		public bool CreateReport { get; set; }
		public bool IndexOnly { get; set; }
	}

	public abstract class BaseExecute
	{
		public DateTime StartUtc { get; private set; }

		public bool CreateLog { get; set; }
		public bool CreateOutput { get; set; }
		public bool CreateExport { get; set; }
		public bool CreateReport { get; set; }

		public string LogFileUri { get; set; } = string.Empty;
		public string ExportFileUri { get; set; } = string.Empty;
		public string OutputFileUri { get; set; } = string.Empty;
		public string ReportFileUri { get; set; } = string.Empty;


		public List<Exception> Exceptions = new List<Exception>();

		public BaseExecute()
		{
			StartUtc = DateTime.UtcNow;
		}
	}

	public abstract class BaseReview
	{
		public DateTime FinishUtc { get; set; }

		public List<Exception> Exceptions { get; set; } = new List<Exception>();

		public BaseReview()
		{
			FinishUtc = DateTime.UtcNow;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fortress.Core.Services.Messages
{
	public abstract class BaseRequest
	{
		public DateTime StartUtc { get; }

		public string? NamePrefix { get; set; }

		public bool StopOnError { get; set; }

		public bool LogResult { get; set; }

		public bool CreateReport { get; set; }

		public bool IndexOnly { get; set; }

		public BaseRequest()
		{
			StartUtc = DateTime.UtcNow;
		}
	}
}

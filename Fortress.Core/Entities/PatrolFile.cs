using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fortress.Core.Entities
{
	public class PatrolFile
	{
		public FileInfo? File { get; set; }
		public string Uri { get; set; }
		public string Name { get; set; }
		public long Size { get; set; }

		public PatrolFileStatus Status { get; set; } = PatrolFileStatus.Default;

		public PatrolFile(FileInfo file)
		{
			Uri = file.FullName;
			Name = file.Name;
			File = file;
			Size = file.Length;
		}

		public PatrolFile(string uri)
		{
			Uri = uri;
			Name = Path.GetFileName(uri);
		}
	}


	public enum PatrolFileStatus
	{
		/// <summary>
		/// Error accessing file
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
		/// File name and size matched
		/// </summary>
		Matched = 2,

		/// <summary>
		/// File hash created
		/// </summary>
		Hashed = 3,

		/// <summary>
		/// File hash verified
		/// </summary>
		Verified = 9,
	}

	public class PatrolFileState
	{
		public PatrolFile File { get; set; }
		public Guid StateId { get; set; } = Guid.NewGuid();
		public PatrolFileStatus Status { get; set; } = PatrolFileStatus.Default;
		public DateTime Timestamp { get; set; } = DateTime.UtcNow;
		public string Message { get; set; } = string.Empty;
		public Exception? Exception { get; set; } = null;


		public PatrolFileState(PatrolFile file, PatrolFileStatus status = PatrolFileStatus.Default, string message = "")
		{
			File = file;
			Status = status;
			Message = message;
		}

		public PatrolFileState(PatrolFile file, Exception ex)
		{
			File = file;
			Status = PatrolFileStatus.Exception;
			Message = ex.Message;
		}
	}
}

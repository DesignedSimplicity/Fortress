using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fortress.Core.Entities
{

	public class PatrolFolder
	{
		public DirectoryInfo? Directory { get; set; }
		public string Uri { get; set; }
		public string Name { get; set; }

		public List<PatrolFolder> PatrolFolders { get; set; } = new List<PatrolFolder>();
		public List<PatrolFile> PatrolFiles { get; set; } = new List<PatrolFile>();

		public long TotalFileSize => PatrolFiles.Sum(x => x.Size);

		public PatrolFolder(DirectoryInfo dir)
		{
			Uri = dir.FullName;
			Name = dir.Name;
			Directory = dir;
		}

		public PatrolFolder(string uri)
		{
			Uri = uri;
			Name = PathUtils.GetParentName(uri);
		}
	}

	public enum PatrolFolderStatus
	{
		/// <summary>
		/// Error accessing folder
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
		/// All contents recursively verifed
		/// </summary>
		Verified = 9,
	}

	public class PatrolFolderState
	{
		public PatrolFolder Folder { get; set; }
		public Guid StateId { get; set; } = Guid.NewGuid();
		public PatrolFolderStatus Status { get; set; } = PatrolFolderStatus.Default;
		public DateTime Timestamp { get; set; } = DateTime.UtcNow;
		public string Message { get; set; } = string.Empty;
		public Exception? Exception { get; set; } = null;


		public PatrolFolderState(PatrolFolder folder, PatrolFolderStatus status = PatrolFolderStatus.Default, string message = "")
		{
			Folder = folder;
			Status = status;
			Message = message;
		}

		public PatrolFolderState(PatrolFolder folder, Exception ex)
		{
			Folder = folder;
			Status = PatrolFolderStatus.Exception;
			Message = ex.Message;
		}
	}
}

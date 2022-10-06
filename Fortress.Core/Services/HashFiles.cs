using Fortress.Core.Entities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fortress.Core.Services
{

	public class HashFiles
	{
		private IProgress<PatrolFileState>? _progress;
		/*
		public delegate void FileStateChange(PatrolFileState state);

		private FolderStateChange? _change;

		private int _folderCount { get; set; }
		private readonly ConcurrentQueue<DirectoryInfo> _folderQueue = new ConcurrentQueue<DirectoryInfo>();
		private readonly ConcurrentBag<Task> _tasks = new ConcurrentBag<Task>();

		public QueryFolders(FolderStateChange progress)
		{
			_change = progress;
		}
		*/
		public HashFiles(IProgress<PatrolFileState> progress)
		{
			_progress = progress;
		}

		public List<PatrolFile> HashAllFiles(List<PatrolFile> files, CancellationToken? token = null)
		{
			foreach (var f in files)
			{
				// cancellation returns empty list
				if (token?.IsCancellationRequested ?? false) return new List<PatrolFile>();

				var state = new PatrolFileState(f, PatrolFileStatus.Hashed);
				if (f.Name.StartsWith("a", StringComparison.OrdinalIgnoreCase))
				{
					state.Status = PatrolFileStatus.NotFound;
				}
				else if (f.Name.StartsWith("e", StringComparison.OrdinalIgnoreCase))
				{
					state.Status = PatrolFileStatus.Exception;
				}
				else if (f.Name.StartsWith("m", StringComparison.OrdinalIgnoreCase))
				{
					state.Status = PatrolFileStatus.Matched;
				}
				else if (f.Name.StartsWith("v", StringComparison.OrdinalIgnoreCase))
				{
					state.Status = PatrolFileStatus.Verified;
				}
				f.Status = state.Status;

				Thread.Sleep(1);

				_progress?.Report(state);
				//_change?.Invoke(state);
			}

			return files;
		}
	}
}

using Fortress.Core.Entities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fortress.Core.Services
{

	public class QueryFiles
	{
		/*
		public delegate void FileStateChange(PatrolFileState state);

		private IProgress<PatrolFolderState>? _progress;
		private FolderStateChange? _change;

		private int _folderCount { get; set; }
		private readonly ConcurrentQueue<DirectoryInfo> _folderQueue = new ConcurrentQueue<DirectoryInfo>();
		private readonly ConcurrentBag<Task> _tasks = new ConcurrentBag<Task>();

		public QueryFolders(IProgress<PatrolFolderState> progress)
		{
			_progress = progress;
		}

		public QueryFolders(FolderStateChange progress)
		{
			_change = progress;
		}
		*/
		public QueryFiles() { }

		public async Task<List<PatrolFile>> LoadFilesAsync(string uri, CancellationToken? token = null)
		{
			return await Task.Run(() => {
				return LoadFiles(uri, token);
			});

		}

		public List<PatrolFile> LoadFiles(string uri, CancellationToken? token = null)
		{
			var list = new List<PatrolFile>();
			
			foreach (var f in Directory.EnumerateFiles(uri, "*.*", new EnumerationOptions { IgnoreInaccessible = true, RecurseSubdirectories = false }))
			{
				// cancellation returns empty list
				if (token?.IsCancellationRequested ?? false) return new List<PatrolFile>();

				var file = new PatrolFile(f);
				var state = new PatrolFileState(file, PatrolFileStatus.Exists);
				list.Add(file);

				Thread.Sleep(100);

				//_progress?.Report(state);
				//_change?.Invoke(state);
			}

			return list;
		}
	}
}

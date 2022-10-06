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
		public QueryFiles()
		{
		}

		public QueryFiles(IProgress<PatrolFileState> progress)
		{
			_progress = progress;
		}

		public async Task<List<PatrolFile>> LoadFilesAsync(string uri, bool recursive = false, CancellationToken? token = null)
		{
			return await Task.Run(() => {
				return LoadFiles(uri, recursive, token);
			});

		}

		public List<PatrolFile> LoadFiles(string uri, bool recursive = false, CancellationToken? token = null)
		{
			var list = new List<PatrolFile>();
			var dir = new DirectoryInfo(uri);
			
			foreach (var f in dir.EnumerateFiles("*.*", new EnumerationOptions { IgnoreInaccessible = true, RecurseSubdirectories = recursive }))
			{
				// cancellation returns empty list
				if (token?.IsCancellationRequested ?? false) return new List<PatrolFile>();

				var file = new PatrolFile(f);
				var state = new PatrolFileState(file, PatrolFileStatus.Exists);
				list.Add(file);

				_progress?.Report(state);
				//_change?.Invoke(state);
				
				Thread.Sleep(1);
			}

			return list;
		}
	}
}

using Fortress.Core.Entities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fortress.Core.Services
{

	public class QueryFolders
	{
		public delegate void FolderStateChange(PatrolFolderState state);

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

		public async Task<List<PatrolFolder>> LoadAllFoldersAsync(string uri, bool recursive = false, CancellationToken? token = null)
		{
			return await Task.Run(() => {
				return LoadAllFolders(uri, recursive, token);
			});

		}

		public List<PatrolFolder> LoadAllFolders(string uri, bool recursive = false, CancellationToken? token = null)
		{
			var list = new List<PatrolFolder>();
			
			var start = new DirectoryInfo(uri);
			Queue<string> dirs = new Queue<string>();
			dirs.Enqueue(start.FullName);
			
			while (dirs.Any())
			{
				if (token?.IsCancellationRequested??false)
				{
					return list;
				}

				var dir = dirs.Dequeue();
				var folder = new PatrolFolder(dir);
				var state = new PatrolFolderState(folder, PatrolFolderStatus.Exists);
				list.Add(folder);
				
				Thread.Sleep(200);

				_progress?.Report(state);
				_change?.Invoke(state);

				foreach (var s in Directory.EnumerateDirectories(dir, "*", new EnumerationOptions { IgnoreInaccessible = true, RecurseSubdirectories = recursive }))
				{
					dirs.Enqueue(s);
				}
			}

			return list;
		}


		/*
		public async Task LoadFolders(DirectoryInfo dir, CancellationToken token)
		{
			await Task.Run(() =>
			{
				Queue<string> dirs = new Queue<string>();
				dirs.Enqueue(dir.FullName);
				while (dirs.Any())
				{
					if (token.IsCancellationRequested) break;
					var d = dirs.Dequeue();
					_progress.Report(new PatrolFolder(d));
					foreach(var s in Directory.EnumerateDirectories(d))
					{
						dirs.Enqueue(s);
					}
				}
			});
		}
		*/

		/*
		public async Task LoadFolders(DirectoryInfo dir)
		{
			await Task.Run(() =>
			{
				CollectFolders(dir.FullName);
			});
		}

		private void CollectFolders(string path)
		{

			DirectoryInfo directoryInfo = new DirectoryInfo(path);
			_tasks.Add(Task.Run(() => CrawlFolder(directoryInfo)));

			Task? taskToWaitFor;
			while (_tasks.TryTake(out taskToWaitFor))
				taskToWaitFor.Wait();
		}


		private void CrawlFolder(DirectoryInfo dir)
		{
			try
			{
				DirectoryInfo[] directoryInfos = dir.GetDirectories();
				foreach (DirectoryInfo childInfo in directoryInfos)
				{
					// here may be dragons using enumeration variable as closure!!
					DirectoryInfo di = childInfo;
					_tasks.Add(Task.Run(() => CrawlFolder(di)));
				}
				_progress.Report(new PatrolFolder(dir.FullName));
				_folderCount++;
			}
			catch (Exception ex)
			{
				while (ex != null)
				{
					Console.WriteLine($"{ex.GetType()} {ex.Message}\n{ex.StackTrace}");
					ex = ex?.InnerException ?? new Exception("InnerException is null");
				}
			}
		}
		*/
	}
}

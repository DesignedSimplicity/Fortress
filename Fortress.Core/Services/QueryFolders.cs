using Fortress.Core.Entities;
using Fortress.Core.Services.Settings;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fortress.Core.Services
{
	public delegate void FolderNotify(PatrolFolder folder);
	public delegate void FolderStateChange(PatrolFolderState state);

	public class QueryFolders
	{
		public List<Exception> Exceptions = new List<Exception>();
		private IProgress<PatrolFolderState>? _progress;
		private FolderStateChange? _change;
		private FolderNotify? _notify;
		private StreamWriter? _output;
		private readonly bool _stopOnError;

		private int _folderCount { get; set; }
		private readonly ConcurrentQueue<DirectoryInfo> _folderQueue = new ConcurrentQueue<DirectoryInfo>();
		private readonly ConcurrentBag<Task> _tasks = new ConcurrentBag<Task>();

		public QueryFolders()
		{
		}

		public QueryFolders(QueryFoldersSettings settings)
		{
			_output = settings.Output;
			_notify = settings.Notify;
			_stopOnError = settings.StopOnError;
		}

		public QueryFolders(StreamWriter? output, FolderNotify? notify)
		{
			_output = output;
			_notify = notify;
		}

		public QueryFolders(IProgress<PatrolFolderState> progress)
		{
			_progress = progress;
		}

		public QueryFolders(FolderStateChange progress)
		{
			_change = progress;
		}

		public PatrolFolder LoadFolder(string uri)
		{
			_output?.WriteLineAsync($"LoadFolder: {uri}");
			var dir = new DirectoryInfo(uri);
			AssertPathTooLongException(dir.FullName);

			var folder = new PatrolFolder(dir);
			var state = new PatrolFolderState(folder, PatrolFolderStatus.Exists);			

			_notify?.Invoke(folder);
			_change?.Invoke(state);
			_progress?.Report(state);

			//Thread.Sleep(1);

			return folder;
		}

		public List<PatrolFolder> LoadFolders(string uri, CancellationToken? token = null)
		{
			_output?.WriteLineAsync($"LoadFolders: {uri}");
			var start = new DirectoryInfo(uri);
			AssertPathTooLongException(start.FullName);

			var list = new List<PatrolFolder>();
			foreach (var dir in start.EnumerateDirectories("*", new EnumerationOptions { IgnoreInaccessible = !_stopOnError, RecurseSubdirectories = false }))
			{
				_output?.WriteLineAsync($"LoadFolder: {dir.FullName}");
				AssertPathTooLongException(dir.FullName);

				var folder = new PatrolFolder(dir);
				var state = new PatrolFolderState(folder, PatrolFolderStatus.Exists);
				list.Add(folder);

				// cancellation returns empty list
				if (token?.IsCancellationRequested ?? false) return new List<PatrolFolder>();

				_notify?.Invoke(folder);
				_change?.Invoke(state);
				_progress?.Report(state);

				//Thread.Sleep(1);
			}

			return list;
		}

		/// <summary>
		/// Loads all folders recursively, including start folder
		/// </summary>
		/// <param name="uri"></param>
		/// <param name="token"></param>
		/// <returns></returns>
		public List<PatrolFolder> LoadAllFolders(string uri, CancellationToken? token = null)
		{
			_output?.WriteLineAsync($"LoadAllFolders: {uri}");
			var start = new DirectoryInfo(uri);
			AssertPathTooLongException(start.FullName);

			var list = new List<PatrolFolder>();			
			Queue<string> dirs = new Queue<string>();
			dirs.Enqueue(start.FullName);
			
			while (dirs.Any())
			{
				var dir = dirs.Dequeue();
				_output?.WriteLineAsync($"LoadFolder: {dir}");
				AssertPathTooLongException(dir);

				var folder = new PatrolFolder(dir);
				var state = new PatrolFolderState(folder, PatrolFolderStatus.Exists);
				list.Add(folder);
				
				// cancellation returns empty list
				if (token?.IsCancellationRequested ?? false) return new List<PatrolFolder>();

				_notify?.Invoke(folder);
				_change?.Invoke(state);
				_progress?.Report(state);

				//Thread.Sleep(1);

				foreach (var f in Directory.EnumerateDirectories(dir, "*", new EnumerationOptions { IgnoreInaccessible = _stopOnError, RecurseSubdirectories = false }))
				{
					dirs.Enqueue(f);
				}
			}

			return list;
		}

		private bool AssertPathTooLongException(string uri)
		{
			if (!PathUtils.IsMaxPath(uri)) return false;

			var message = $"FolderPathTooLong: {uri}";
			_output?.WriteLineAsync(message);
			var exception = new PathTooLongException(message);
			Exceptions.Add(exception);

			if (_stopOnError)
				throw exception;
			else
				return true;
		}
	}
}

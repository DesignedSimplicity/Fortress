using Fortress.Core.Entities;
using Fortress.Core.Services.Settings;
using OfficeOpenXml.Style;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Fortress.Core.Services.QueryFolders;

namespace Fortress.Core.Services
{
	public delegate void FileNotify(PatrolFile file);

	public class QueryFiles
	{
		public List<Exception> Exceptions = new List<Exception>();
		private IProgress<PatrolFileState>? _progress;
		private readonly StreamWriter? _output;
		private readonly FileNotify? _notify;
		private readonly bool _stopOnError;
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

		public QueryFiles(QueryFilesSettings settings)
		{
			_output = settings.Output;
			_notify = settings.Notify;
			_stopOnError = settings.StopOnError;
		}

		public QueryFiles(StreamWriter? output, FileNotify? notify)
		{
			_output = output;
			_notify = notify;
		}

		public QueryFiles(IProgress<PatrolFileState> progress)
		{
			_progress = progress;
		}

		public List<PatrolFile> LoadAllFiles(string uri, string? filter = "", CancellationToken? token = null)
		{
			return LoadFiles(uri, filter, true, token);
		}

		public List<PatrolFile> LoadFiles(string uri, string? filter = "", bool recursive = false, CancellationToken? token = null)
		{
			var list = new List<PatrolFile>();
			var dir = new DirectoryInfo(uri);
			
			foreach (var f in dir.EnumerateFiles(filter ?? "*.*", new EnumerationOptions { IgnoreInaccessible = !_stopOnError, RecurseSubdirectories = recursive }))
			{
				_output?.WriteLine($"LoadFile: {f.FullName}");
				AssertPathTooLongException(f.FullName);

				// cancellation returns empty list
				if (token?.IsCancellationRequested ?? false) return new List<PatrolFile>();

				var file = new PatrolFile(f);
				var state = new PatrolFileState(file, PatrolFileStatus.Exists);
				list.Add(file);

				_notify?.Invoke(file);
				_progress?.Report(state);
				//_change?.Invoke(state);
				
				//Thread.Sleep(1);
			}

			return list;
		}
		private bool AssertPathTooLongException(string uri)
		{
			if (!PathUtils.IsMaxPath(uri)) return false;

			var message = $"FilePathTooLong: {uri}";
			_output?.WriteLine(message);
			var exception = new PathTooLongException(message);
			Exceptions.Add(exception);

			if (_stopOnError)
				throw exception;
			else
				return true;
		}
	}
}

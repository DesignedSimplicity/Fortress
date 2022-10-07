using Fortress.Core.Entities;
using Fortress.Core.Services;
using Fortress.Core.Services.Settings;
using Fortress.Core.Services.Messages;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fortress.Core.Services
{
	public sealed class CreatePatrol : BasePatrol, IDisposable
	{
		private StreamWriter? _log;
		private readonly FolderNotify? _folderNotify;
		private readonly FileNotify? _fileNotify;

		public CreatePatrol(FolderNotify? folder, FileNotify? file)
		{
			_folderNotify = folder;
			_fileNotify = file;
		}

		public void Dispose()
		{
			_log?.Dispose();
		}

		public CreatePatrolExecute Validate(CreatePatrolRequest request)
		{
			var execute = new CreatePatrolExecute(request);

			// directory is accessible
			var dir = new DirectoryInfo(String.IsNullOrWhiteSpace(request.DirectoryUri) ? Directory.GetCurrentDirectory() : request.DirectoryUri);
			if (!dir.Exists) throw new DirectoryNotFoundException($"Target folder '{dir.FullName}' does not exist or is not accessible");
			var uri = execute.SourceFolderUri = PathUtils.FixUri(dir.FullName, true);

			// output name is valid
			string name = request.NamePrefix ?? "";
			if (!String.IsNullOrEmpty(name) && !PathUtils.IsValidFilename(name)) throw new ArgumentOutOfRangeException($"Name prefix {name} is not valid");
			name += $"_{execute.StartUtc:yyyyMMdd}-{execute.StartUtc:HHmmss}";
			execute.RunName = name;

			// ensure output file is accessible
			if (!request.IndexOnly)
			{
				execute.CreateOutput = true;
				execute.OutputFileUri = Path.Combine(uri, name + "." + request.HashType.ToString().ToLowerInvariant());
				if (File.Exists(execute.OutputFileUri)) throw new IOException($"Output file '{execute.OutputFileUri}' already exists");				
			}

			// ensure log file is accessible
			if (request.LogOutput)
			{
				execute.CreateLog = true;
				execute.LogFileUri = Path.Combine(uri, name + LogFileExtension);
				if (File.Exists(execute.LogFileUri)) throw new IOException($"Log file '{execute.LogFileUri}' already exists");
			}

			// ensure report file is accessible
			if (request.CreateReport)
			{
				execute.CreateReport = true;
				execute.ReportFileUri = Path.Combine(uri, name + ReportFileExtension);
				if (File.Exists(execute.ReportFileUri)) throw new IOException($"Report file '{execute.ReportFileUri}' already exists");
			}

			return execute;
		}

		public bool Prepare(CreatePatrolExecute execute)
		{
			// prepare output files
			if (execute.CreateLog) _log = new StreamWriter(execute.LogFileUri);

			// load all directories
			var queryFolders = new QueryFolders(new QueryFoldersSettings()
			{
				StopOnError = execute.Request.StopOnError,
				VerboseLog = execute.Request.VerboseLog,
				Notify = _folderNotify,
				Output = _log,
			});
			execute.Folders = queryFolders.LoadAllFolders(execute.SourceFolderUri).OrderBy(x => x.Uri).ToList();
			execute.Exceptions.AddRange(queryFolders.Exceptions);

			// load files for each directory
			var queryFiles = new QueryFiles(new QueryFilesSettings()
			{
				StopOnError = execute.Request.StopOnError,
				VerboseLog = execute.Request.VerboseLog,
				Notify = _fileNotify,
				Output = _log,
			});
			foreach(var folder in execute.Folders)
			{
				var files = queryFiles.LoadFiles(folder.Uri, execute.Request.SearchFilter).OrderBy(x => x.Name).ToList();
				execute.Files.AddRange(files);
			}
			execute.Exceptions.AddRange(queryFiles.Exceptions);

			// return success if no exceptions
			return execute.Exceptions.Count == 0;
		}

		public void Execute(CreatePatrolExecute execute)
		{

		}

		public void Output(CreatePatrolExecute execute)
		{

		}

		public void Report(CreatePatrolExecute execute)
		{

		}

		public void Review(CreatePatrolExecute execute)
		{
		}

		public void Finish()
		{
			_log?.Flush();
			_log?.Close();
		}
	}
}

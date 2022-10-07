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
using System.Collections;
using Pastel;
using static System.Collections.Specialized.BitVector32;
using System.Diagnostics;

namespace Fortress.Core.Services
{
	public sealed class CreatePatrol : BasePatrol, IDisposable
	{
		private StreamWriter? _log;
		private TextWriter? _console;
		private readonly FolderNotify? _folderNotify;
		private readonly FileNotify? _fileNotify;
		private readonly FileNotify? _hashNotify;

		public CreatePatrol(TextWriter? console = null)
		{
			_console = console;
		}

		public CreatePatrol(FolderNotify? folder, FileNotify? file, FileNotify? hash)
		{
			_folderNotify = folder;
			_fileNotify = file;
			_hashNotify = hash;
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

			// show result in console
			_console?.WriteLine(ConsoleDivider);
			_console?.WriteLine($"RunName: {execute.RunName}");
			_console?.WriteLine($"SourceFolderUri: {execute.SourceFolderUri}");
			_console?.WriteLine($"LogFileUri: {execute.LogFileUri}");
			_console?.WriteLine($"OutputFileUri: {execute.OutputFileUri}");
			_console?.WriteLine($"ReportFileUri: {execute.ReportFileUri}");

			// return execute package
			return execute;
		}

		public bool Prepare(CreatePatrolExecute execute)
		{
			// show prep in console
			_console?.WriteLine(ConsoleSection.Pastel(Color.Orange));
			_console?.WriteLine($"Prepare: {execute.SourceFolderUri}".Pastel(Color.Orange));
			_console?.WriteLine(ConsoleDivider.Pastel(Color.Orange));

			// prepare output files
			if (execute.CreateLog) _log = new StreamWriter(execute.LogFileUri);
			var verbose = execute.Request.VerboseLog;

			// prepare query system
			var queryFolders = new QueryFolders(new QueryFoldersSettings()
			{
				StopOnError = execute.Request.StopOnError,
				VerboseLog = verbose,
				Notify = _folderNotify,
				Output = _log,
			});
			var queryFiles = new QueryFiles(new QueryFilesSettings()
			{
				StopOnError = execute.Request.StopOnError,
				VerboseLog = verbose,
				Notify = _fileNotify,
				Output = _log,
			});

			// start with root folder
			var root = queryFolders.LoadFolder(execute.SourceFolderUri);
			Queue<PatrolFolder> queue = new Queue<PatrolFolder>();
			queue.Enqueue(root);

			// until all folders are complete
			while (queue.Count > 0)
			{
				// load current folder
				var folder = queue.Dequeue();
				_console?.Write(folder.Uri.Pastel(Color.Goldenrod));

				// load subfolders
				var folders = queryFolders.LoadFolders(folder.Uri);
				folder.PatrolFolders.AddRange(folders);
				if (verbose) _console?.Write($"\tFolders: {folder.PatrolFolders.Count}".Pastel(Color.Goldenrod));

				// load files for current folder
				var files = queryFiles.LoadFiles(folder.Uri, execute.Request.SearchFilter).OrderBy(x => x.Name).ToList();
				folder.PatrolFiles.AddRange(files);
				if (verbose) _console?.Write($"\tFiles: {folder.PatrolFiles.Count}".Pastel(Color.Goldenrod));
				_console?.WriteLine();

				// show verbose log if requested
				if (verbose)
				{
					if (files.Any()) _console?.WriteLine(ConsoleDivider.Pastel(Color.LightGoldenrodYellow));
					foreach (var file in files)
					{
						_console?.WriteLine($"{file.Name}".Pastel(Color.LightGoldenrodYellow));
					}
					_console?.WriteLine(ConsoleDivider.Pastel(Color.Goldenrod));
				}

				// add to execute list
				execute.Folders.Add(folder);
				execute.Files.AddRange(files);

				// recursion
				foreach (var sub in folders)
				{
					queue.Enqueue(sub);
				}
			}

			// show summary
			_console?.WriteLine();
			_console?.WriteLine(ConsoleSection.Pastel(Color.Orange));
			_console?.WriteLine($"Folders: {execute.Folders.Count.ToString(WholeNumberFormat)}\tFiles: {execute.Files.Count.ToString(WholeNumberFormat)}\tTotal Size: {execute.Files.Sum(x => x.Size).ToString(WholeNumberFormat)}".Pastel(Color.Orange));
			_console?.WriteLine(ConsoleDivider.Pastel(Color.Orange));

			// add collected exceptions
			execute.Exceptions.AddRange(queryFolders.Exceptions);
			execute.Exceptions.AddRange(queryFiles.Exceptions);
			var exceptions = execute.Exceptions.Any();

			// show expcetions
			if (exceptions)
			{
				_console?.WriteLine();
				_console?.WriteLine(ConsoleFooter.Pastel(Color.Red));
				_console?.WriteLine($"Exceptions: {execute.Exceptions.Count}".Pastel(Color.Red));
				foreach (var ex in execute.Exceptions)
				{
					_console?.WriteLine(ConsoleDivider.Pastel(Color.Red));
					ShowException(_console, ex, verbose);
				}
			}

			// return success if no exceptions
			return exceptions;
		}

		public void Process(CreatePatrolExecute execute)
		{
			// show prep in console
			_console?.WriteLine(ConsoleSection.Pastel(Color.Cyan));
			_console?.WriteLine($"Process: {execute.SourceFolderUri}".Pastel(Color.Cyan));
			_console?.WriteLine(ConsoleDivider.Pastel(Color.Cyan));

			long totalDataProcessed = 0;
			var stopwatch = new Stopwatch();
			using (var hasher = new GetHash())
			{
				// prepare totals
				var fileIndex = 0;
				var fileCount = execute.Files.Count;
				foreach (var folder in execute.Folders.OrderBy(x => x.Uri))
				{
					var anyFiles = folder.PatrolFiles.Any();
					_console?.WriteLine($"Folder: {folder.Uri}".Pastel(anyFiles ? Color.Goldenrod : Color.DarkGoldenrod));
					if (anyFiles)
					{
						_console?.WriteLine(ConsoleDivider.Pastel(Color.Gray));

						foreach (var file in folder.PatrolFiles.OrderBy(x => x.Name))
						{
							fileIndex++;
							totalDataProcessed += file.Size;

							_console?.Write($"[{fileIndex} of {fileCount}] ".Pastel(Color.Gray));
							_console?.Write($"{file.Name} -> ");
							_console?.Write($"{file.Size:###,###,###,###,##0}".Pastel(Color.LightYellow));

							if (execute.Request.IndexOnly)
							{
								_console?.WriteLine($" LOG".Pastel(Color.GreenYellow));
							}
							else
							{
								// calculate hash and output hash to log
								var hashType = execute.Request.HashType;
								_console?.Write($" {hashType.ToString().Pastel(Color.LightGreen)} = ");
								try
								{
									var result = hasher.Calculate(file, hashType);
									_console?.WriteLine($"{result.HashValue.Pastel(Color.Green)} @ {result.BytesPerMillisecond:###,###,###,###,##0} b/ms".Pastel(Color.Gray));
								}
								catch (UnauthorizedAccessException ex)
								{
									file.Status = FileStatus.Error;
									execute.Exceptions.Add(ex);

									// log exception and re-throw if not silent
									if (!execute.Request.StopOnError)
									{
										_console?.WriteLine($"UnauthorizedAccessException".Pastel(Color.Red));
									}
									else
										throw;
								}
							}
						}

						_console?.WriteLine(ConsoleDivider.Pastel(Color.Goldenrod));
					}
				}
			}

			stopwatch.Stop();
			var time = stopwatch.Elapsed;
			var mb = 1.0 * totalDataProcessed / 1024.0 / 1024.0;
			var gb = 1.0 * mb / 1024;
			var tb = 1.0 * gb / 1024.0;
			var gbh = 1.0 * gb / time.TotalHours;
			var mbs = 1.0 * mb / time.TotalMinutes;

			_console?.WriteLine(ConsoleSection.Pastel(Color.Cyan));
			_console?.WriteLine($"BYTES:\t{totalDataProcessed.ToString(WholeNumberFormat)} = {mb.ToString(SingleDecimalFormat)} MB = {gb.ToString(SingleDecimalFormat)} GB = {tb.ToString(SingleDecimalFormat)} TB".Pastel(Color.Cyan));
			_console?.WriteLine($"TIME:\t{time:hh\\:mm\\:ss} H:M:S = {time.TotalSeconds.ToString(WholeNumberFormat)} SEC".Pastel(Color.Cyan));
			_console?.WriteLine($"RATE:\t{gbh.ToString(SingleDecimalFormat)} GB/HOUR = {mbs.ToString(SingleDecimalFormat)} MB/SEC".Pastel(Color.Cyan));
			_console?.WriteLine(ConsoleDivider.Pastel(Color.Cyan));
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

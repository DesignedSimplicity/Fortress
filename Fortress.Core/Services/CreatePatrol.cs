//TODO:
//Better exception reporting - add to Review section - test all instances - add to excel file
//Simple vs verbose logging


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

		public CreatePatrolExecute Prepare(CreatePatrolRequest request)
		{
			var execute = Validate(request);

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
				execute.Folders.Add(folder);
				if (verbose) _console?.Write($"\tFolders: {folders.Count}".Pastel(Color.Goldenrod));

				// load files for current folder
				var files = queryFiles.LoadFiles(folder.Uri, execute.Request.SearchFilter).OrderBy(x => x.Name).ToList();
				folder.PatrolFiles.AddRange(files);
				execute.Files.AddRange(files);
				if (verbose)  _console?.Write($"\tFiles: {files.Count}".Pastel(Color.Goldenrod));
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
			return execute;
		}

		private CreatePatrolExecute Validate(CreatePatrolRequest request)
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

		public CreatePatrolReview Execute(CreatePatrolExecute execute)
		{
			// show prep in console
			_console?.WriteLine(ConsoleSection.Pastel(Color.Cyan));
			_console?.WriteLine($"Execute: {(execute.Request.IndexOnly ? "Index" : execute.Request.HashType)}".Pastel(Color.Cyan));
			_console?.WriteLine(ConsoleDivider.Pastel(Color.Cyan));

			//long totalDataProcessed = 0;
			//var stopwatch = new Stopwatch();
			var exceptions = new List<Exception>();
			var verbose = execute.Request.VerboseLog;
			using (var hasher = new GetHash())
			{
				// prepare totals
				var fileIndex = 0;
				var fileCount = execute.Files.Count;
				foreach (var folder in execute.Folders.OrderBy(x => x.Uri))
				{
					var anyFiles = folder.PatrolFiles.Any();
					
					if (verbose)
						_console?.WriteLine($"Folder: {folder.Uri}".Pastel(anyFiles ? Color.Goldenrod : Color.DarkGoldenrod));
					else
						_console?.Write("*".Pastel(Color.Goldenrod));

					if (anyFiles)
					{
						if (verbose) _console?.WriteLine(ConsoleDivider.Pastel(Color.Gray));

						foreach (var file in folder.PatrolFiles.OrderBy(x => x.Name))
						{
							fileIndex++;

							if (verbose)
							{
								_console?.Write($"[{fileIndex} of {fileCount}] ".Pastel(Color.Gray));
								_console?.Write($"{file.Name} -> ");
								_console?.Write($"{file.Size:###,###,###,###,##0}".Pastel(Color.LightGreen));
							}

							if (execute.Request.IndexOnly)
							{
								if (verbose) 
									_console?.WriteLine($" LOG".Pastel(Color.GreenYellow));
								else
									_console?.Write(".".Pastel(Color.Goldenrod));
							}
							else
							{
								// calculate hash and output hash to log
								var hashType = execute.Request.HashType;
								if (verbose) _console?.Write($" {hashType.ToString().Pastel(Color.LightGreen)} = ");
								try
								{
									var result = hasher.Calculate(file, hashType);
									if (verbose) 
										_console?.WriteLine($"{result.HashValue.Pastel(Color.Green)} @ {result.BytesPerMillisecond:###,###,###,###,##0} b/ms".Pastel(Color.Gray));
									else
										_console?.Write(".".Pastel(Color.Goldenrod));
								}
								catch (Exception ex)
								{
									_console?.Write("!".Pastel(Color.Red));

									file.Status = FileStatus.Error;
									exceptions.Add(ex);

									// log exception and re-throw if not silent
									if (execute.Request.StopOnError) throw;
								}
							}
						}

						if (verbose) _console?.WriteLine(ConsoleDivider.Pastel(Color.Goldenrod));
					}
				}
			}
			_console?.WriteLine();

			// show expcetions
			if (exceptions.Any())
			{
				_console?.WriteLine();
				_console?.WriteLine(ConsoleFooter.Pastel(Color.Red));
				_console?.WriteLine($"Exceptions: {exceptions.Count}".Pastel(Color.Red));
				foreach (var ex in exceptions)
				{
					_console?.WriteLine(ConsoleDivider.Pastel(Color.Red));
					ShowException(_console, ex, verbose);
				}
			}

			// return review state
			var review = new CreatePatrolReview(execute);
			review.Exceptions.AddRange(exceptions);
			return review;
		}

		public void Review(CreatePatrolReview review)
		{
			var execute = review.Execute;
			var verbose = execute.Request.VerboseLog;

			var color = Color.Violet;
			_console?.WriteLine(ConsoleSection.Pastel(color));
			_console?.WriteLine($"Review: {execute.LogFileUri}".Pastel(color));

			Output(review);
			Report(review);

			if (verbose) Summary(review);
			
			_console?.WriteLine(ConsoleDivider.Pastel(color));
			_console?.WriteLine($"Mode:\t\tCreate {(execute.Request.IndexOnly ? "Index" : execute.Request.HashType)}".Pastel(color));
			_console?.WriteLine($"Time:\t\t{review.Source.ElapsedTime:hh\\:mm\\:ss}".Pastel(color));
			_console?.WriteLine($"Files:\t\t{execute.Files.Count}".Pastel(color));
			_console?.WriteLine($"Folders:\t{execute.Folders.Count}".Pastel(color));			
			_console?.WriteLine(ConsoleFooter.Pastel(color));
		}

		private void Summary(CreatePatrolReview review)
		{
			var time = review.FinishUtc - review.Execute.StartUtc;

			double bytes = review.Execute.Files.Sum(x => x.Size);
			var kilobytes = bytes / 1024.0;
			var megabytes = kilobytes / 1024.0;
			var gigabytes = megabytes / 1024.0;
			var terabytes = gigabytes / 1024.0;

			var bpms = 1.0 * bytes / time.TotalMilliseconds;
			var kbps = 1.0 * kilobytes / time.TotalSeconds;
			var mbpm = 1.0 * megabytes / time.TotalMinutes;
			var gbph = 1.0 * gigabytes / time.TotalHours;
			var tbpd = 1.0 * terabytes / time.TotalDays;

			var format1 = GetPaddedNumberFormat(GetMaxForFormat(bytes, time.TotalMilliseconds, bpms));
			var format2 = GetPaddedNumberFormat(GetMaxForFormat(kilobytes, time.TotalSeconds, kbps));
			var format3 = GetPaddedNumberFormat(GetMaxForFormat(megabytes, time.TotalMinutes, mbpm), 1);
			var format4 = GetPaddedNumberFormat(GetMaxForFormat(gigabytes, time.TotalHours, gbph), 2);
			var format5 = GetPaddedNumberFormat(GetMaxForFormat(terabytes, time.TotalDays, tbpd), 3);

			var color = Color.Pink;
			_console?.WriteLine(ConsoleDivider.Pastel(color));

			_console?.Write($"Size:".Pastel(color));
			_console?.Write($"\t{String.Format(format1, bytes)} Bytes    ".Pastel(color));
			_console?.Write($"\t{String.Format(format2, kilobytes)} Kilobytes".Pastel(color));
			_console?.Write($"\t{String.Format(format3, megabytes)} Megabytes".Pastel(color));
			_console?.Write($"\t{String.Format(format4, gigabytes)} Gigabytes".Pastel(color));
			_console?.Write($"\t{String.Format(format5, terabytes)} Terabytes".Pastel(color));
			_console?.WriteLine();

			color = Color.LightPink;
			_console?.Write($"Time:".Pastel(color));
			_console?.Write($"\t{String.Format(format1, time.TotalMilliseconds)} Msec     ".Pastel(color));
			_console?.Write($"\t{String.Format(format2, time.TotalSeconds)} Seconds  ".Pastel(color));
			_console?.Write($"\t{String.Format(format3, time.TotalMinutes)} Minutes  ".Pastel(color));
			_console?.Write($"\t{String.Format(format4, time.TotalHours)} Hours    ".Pastel(color));
			_console?.Write($"\t{String.Format(format5, time.TotalDays)} Days     ".Pastel(color));
			_console?.WriteLine();

			color = Color.HotPink;
			_console?.Write($"Rate:".Pastel(color));
			_console?.Write($"\t{String.Format(format1, bpms)} B/Ms     ".Pastel(color));
			_console?.Write($"\t{String.Format(format2, kbps)} KB/Sec   ".Pastel(color));
			_console?.Write($"\t{String.Format(format3, mbpm)} MB/Min   ".Pastel(color));
			_console?.Write($"\t{String.Format(format4, gbph)} GB/Hour  ".Pastel(color));
			_console?.Write($"\t{String.Format(format5, tbpd)} TB/Day   ".Pastel(color));
			_console?.WriteLine();
		}

		private void Output(CreatePatrolReview review)
		{
			var execute = review.Execute;
			if (!execute.CreateOutput) return;

			var color = Color.Violet;
			//_console?.WriteLine(ConsoleSection.Pastel(color));
			_console?.WriteLine($"Output: {execute.OutputFileUri}".Pastel(color));

			// create md5/sha512 output file header
			using (StreamWriter output = File.CreateText(execute.OutputFileUri))
			{
				output.WriteLine($"# Generated {execute.Request.HashType} with Patrol at UTC {DateTime.UtcNow}");
				output.WriteLine($"# https://github.com/DesignedSimplicity/Expedition/");
				output.WriteLine($"# --------------------------------------------------");
				output.WriteLine();
				output.WriteLine($"# System:\t{execute.SystemName}");
				output.WriteLine($"# Source:\t{execute.SourceFolderUri}");
				//output.WriteLine($"# Target Folder: {execute.PatrolSource.TargetFolderUri}");
				output.WriteLine($"# Hash:\t\t{execute.Request.HashType}");
				output.WriteLine($"# Size:\t\t{execute.Files.Sum(x => x.Size).ToString(WholeNumberFormat)}");
				output.WriteLine($"# Files:\t{execute.Files.Count.ToString(WholeNumberFormat)}");
				output.WriteLine($"# Folders:\t{execute.Folders.Count.ToString(WholeNumberFormat)}");
				if (execute.Exceptions.Count() > 0) output.WriteLine($"# Errors:\t{execute.Exceptions.Count().ToString(WholeNumberFormat)}");
				output.WriteLine($"# --------------------------------------------------");

				// prepare for files output
				var maxFiles = execute.Folders.Max(x => x.PatrolFiles.Count);
				var filesFormat = GetPaddedNumberFormat(maxFiles);
				var maxSizes = execute.Folders.Max(x => x.PatrolFiles.Sum(y => y.Size));
				var sizesFormat = GetPaddedNumberFormat(maxSizes);
				foreach (var folder in execute.Folders.OrderBy(x => x.Uri))
				{
					output.WriteLine($"# Files: {String.Format(filesFormat, folder.PatrolFiles.Count)}\tSize: {String.Format(sizesFormat, folder.TotalFileSize)}\t\t{folder.Uri}");
				}
				output.WriteLine($"# --------------------------------------------------");
				output.WriteLine();

				// format and write checksum to stream
				foreach (var file in execute.Files.OrderBy(x => x.Uri))
				{
					var path = file.Uri;// execute.GetOuputPath(file.Uri);
					output.WriteLine($"{((execute.Request.HashType == HashType.Md5) ? file.Md5 : file.Sha512)} {path}");
				}

				// clean up output file
				output.Flush();
				output.Close();

				// show summary
				//_console?.WriteLine(ConsoleDivider.Pastel(color));
				//_console?.WriteLine($"Saved {execute.Files.Count().ToString(WholeNumberFormat)} {execute.Request.HashType} file hashes".Pastel(color));
				//_console?.WriteLine(ConsoleDivider.Pastel(color));
			}
		}

		private void Report(CreatePatrolReview review)
		{
			var execute = review.Execute;
			if (!execute.CreateReport) return;

			var color = Color.Violet;
			//_console?.WriteLine(ConsoleSection.Pastel(color));
			_console?.WriteLine($"Report: {execute.ReportFileUri}".Pastel(color));

			// set up reporting
			using (var report = new BuildReport())
			{
				// format and write checksum to stream
				report.PopulateSource(review.Source);
				report.PopulateFolders(execute.Folders.OrderBy(x => x.Uri));
				report.PopulateFiles(execute.Files.OrderBy(x => x.Uri));

				// close out report
				report.SaveAs(execute.ReportFileUri);
				report.Dispose();
			}

			// show summary
			//_console?.WriteLine(ConsoleDivider.Pastel(color));
			//_console?.WriteLine($"Saved {execute.Files.Count().ToString(WholeNumberFormat)} files in {execute.Folders.Count().ToString(WholeNumberFormat)} folders".Pastel(color));
			//_console?.WriteLine(ConsoleDivider.Pastel(color));
		}

		public void Finish()
		{
			_log?.Flush();
			_log?.Close();
		}
	}
}

using Fortress.Core.Entities;
using Fortress.Core.Services;
using Fortress.Core.Services.Messages;
using OfficeOpenXml.Style;
using Pastel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fortress.Patrol
{
	internal class Engine
	{
		private const string _formatNumber = "###,###,###,###,###,##0";
		private const string _section = "================================================================================";
		private const string _divider = "--------------------------------------------------------------------------------";
		private const string _footer = "################################################################################";

		public void Verify(VerifyOptions v)
		{
			Console.WriteLine($"Verify");
			Console.WriteLine(_section);
		}

		public void Create(CreateOptions c)
		{
			Console.WriteLine($"Create");
			Console.WriteLine(_section);
			Console.WriteLine($"Name: {c.NamePrefix}");
			Console.WriteLine($"Directory: {c.DirectoryUri}");
			Console.WriteLine($"Search: {c.SearchFilter}");
			Console.WriteLine($"Hash: {c.HashType}");
			Console.WriteLine($"Report: {c.CreateReport}");
			Console.WriteLine($"Index: {c.IndexOnly}");
			Console.WriteLine($"Error: {c.StopOnError}");
			Console.WriteLine($"Flat: {c.FlatFolder}");
			Console.WriteLine($"Log: {c.LogOutput}");
			Console.WriteLine($"Verbose: {c.VerboseLog}");

			bool verbose = c.VerboseLog;

			Console.SetWindowSize(200, 50);

			var createPatrol = new CreatePatrol(this.FolderLoaded, this.FileLoaded);
			try
			{
				// validate
				var request = new CreatePatrolRequest()
				{
					// common
					NamePrefix = c.NamePrefix,
					IndexOnly = c.IndexOnly,
					LogOutput = c.LogOutput,
					VerboseLog = c.VerboseLog,
					StopOnError = c.StopOnError,
					CreateReport = c.CreateReport,

					// create
					DirectoryUri = c.DirectoryUri,
					SearchFilter = c.SearchFilter,
					Recursive = !c.FlatFolder,
					HashType = c.HashType,
				};
				var execute = createPatrol.Validate(request);
				Console.WriteLine(_divider);
				Console.WriteLine($"RunName: {execute.RunName}");
				Console.WriteLine($"SourceFolderUri: {execute.SourceFolderUri}");
				Console.WriteLine($"LogFileUri: {execute.LogFileUri}");
				Console.WriteLine($"OutputFileUri: {execute.OutputFileUri}");
				Console.WriteLine($"ReportFileUri: {execute.ReportFileUri}");

				// prepare
				Console.WriteLine(_section.Pastel(Color.Orange));
				Console.WriteLine(execute.SourceFolderUri.Pastel(Color.Orange));
				Console.WriteLine(_divider.Pastel(Color.Orange));
				var prepared = createPatrol.Prepare(execute);
				if (!prepared)
				{
					Console.WriteLine(_footer.Pastel(Color.Red));
					Console.WriteLine($"Exceptions: {execute.Exceptions.Count}".Pastel(Color.Red));
					foreach (var ex in execute.Exceptions)
					{
						Console.WriteLine(_divider.Pastel(Color.Red));
						ShowException(ex, verbose);
					}
				}
				Console.WriteLine(_section.Pastel(Color.DarkGoldenrod));
				Console.WriteLine($"Folders: {execute.Folders.Count}\tFiles: {execute.Files.Count}\tTotal Size: {execute.Files.Sum(x => x.Size)}".Pastel(Color.DarkGoldenrod));
				Console.WriteLine(_divider.Pastel(Color.DarkGoldenrod));
			}
			catch (Exception ex)
			{
				Console.WriteLine(_section.Pastel(Color.Red));
				ShowException(ex, verbose);
			}
			finally
			{
				createPatrol.Finish();
			}
		}

		private void ShowException(Exception ex, bool verbose = false)
		{
			Console.WriteLine(ex.Message.Pastel(Color.Red));
			if (verbose) Console.WriteLine(ex.ToString().Pastel(Color.DarkRed));
		}

		private void FolderLoaded(PatrolFolder folder)
		{
			Console.WriteLine(folder.Uri.Pastel(Color.Goldenrod));
		}

		private void FileLoaded(PatrolFile file)
		{
			Console.WriteLine(file.Uri.Pastel(Color.LightGoldenrodYellow));
		}

		private void HashCreated(PatrolFile file)
		{
			Console.WriteLine($"{file.Uri}\t{file.Size.ToString(_formatNumber)}".Pastel(Color.Cyan));
		}
	}
}

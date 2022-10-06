using Fortress.Core.Services;
using Fortress.Core.Services.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fortress.Patrol
{
	internal class Engine
	{
		public void Verify(VerifyOptions v)
		{
			Console.WriteLine($"Verify");
			Console.WriteLine($"==========");
		}

		public void Create(CreateOptions c)
		{
			Console.WriteLine($"Create");
			Console.WriteLine($"==========");
			Console.WriteLine($"Name: {c.NamePrefix}");
			Console.WriteLine($"Directory: {c.DirectoryUri}");
			Console.WriteLine($"Search: {c.SearchFilter}");
			Console.WriteLine($"Hash: {c.HashType}");
			Console.WriteLine($"Report: {c.CreateReport}");
			Console.WriteLine($"Index: {c.IndexOnly}");
			Console.WriteLine($"Error: {c.StopOnError}");
			Console.WriteLine($"Flat: {c.FlatFolder}");
			Console.WriteLine($"Log: {c.LogOutput}");

			try
			{
				var createPatrol = new CreatePatrol();
				var execute = createPatrol.Validate(new CreatePatrolRequest()
				{
					// common
					NamePrefix = c.NamePrefix,
					IndexOnly = c.IndexOnly,
					LogResult = c.LogOutput,
					StopOnError = c.StopOnError,
					CreateReport = c.CreateReport,

					// create
					DirectoryUri = c.DirectoryUri,
					SearchFilter = c.SearchFilter,
					Recursive = !c.FlatFolder,
					HashType = c.HashType,
				});


				Console.WriteLine($"----------");
				Console.WriteLine($"RunName: {execute.RunName}");
				Console.WriteLine($"SourceFolderUri: {execute.SourceFolderUri}");
				Console.WriteLine($"LogFileUri: {execute.LogFileUri}");
				Console.WriteLine($"OutputFileUri: {execute.OutputFileUri}");
				Console.WriteLine($"ReportFileUri: {execute.ReportFileUri}");
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
		}
	}
}

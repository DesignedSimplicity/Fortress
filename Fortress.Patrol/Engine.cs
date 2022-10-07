using Fortress.Core.Entities;
using Fortress.Core.Services;
using Fortress.Core.Services.Messages;
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
		public void Verify(VerifyOptions v)
		{
			Console.WriteLine($"Verify");
		}

		public void Create(CreateOptions c)
		{
			Console.WriteLine($"Create");
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

			var createPatrol = new CreatePatrol(Console.Out);
			try
			{				
				var execute = createPatrol.Prepare(request);
				var review = createPatrol.Execute(execute);
				createPatrol.Review(review);
			}
			catch (Exception ex)
			{
				Console.WriteLine("");
				Console.WriteLine("####################################################################################################".Pastel(Color.Red));
				Console.WriteLine($"EXCEPTION: {ex.Message}".Pastel(Color.Red));
				Console.WriteLine(ex.ToString().Pastel(Color.DarkRed));
			}
			finally
			{
				createPatrol.Finish();
			}
		}
	}
}

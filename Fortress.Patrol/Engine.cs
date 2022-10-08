using Fortress.Core.Actions;
using Fortress.Core.Actions.Messages;
using Pastel;
using System.Drawing;

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
			Console.WriteLine($"Name:\t {c.NamePrefix}");
			Console.WriteLine($"Uri:\t {c.DirectoryUri}");
			Console.WriteLine($"Search:\t {c.SearchFilter}");
			Console.WriteLine($"Hash:\t {c.HashType}");
			Console.WriteLine($"Report:\t {c.CreateReport}");
			Console.WriteLine($"Index:\t {c.IndexOnly}");
			Console.WriteLine($"Error:\t {c.StopOnError}");
			Console.WriteLine($"Flat:\t {c.FlatFolder}");
			Console.WriteLine($"Log:\t {c.LogOutput}");
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

			var createPatrol = new CreatePatrol(c.Background ? null : Console.Out);
			try
			{
				if (c.Background) Console.WriteLine($"Start:\t {DateTime.Now}");

				var execute = createPatrol.Prepare(request);
				var review = createPatrol.Execute(execute);
				createPatrol.Review(review);

				if (c.Background)
				{
					Console.WriteLine($"Done:\t {DateTime.Now}");
					Console.WriteLine($"Time:\t {review.FinishUtc - execute.StartUtc}");
				}
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

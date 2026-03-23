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
			Console.WriteLine($"Name Prefix:    {c.NamePrefix}");
			Console.WriteLine($"Directory Uri:  {c.DirectoryUri}");
			Console.WriteLine($"Search Filter:  {c.SearchFilter}");
			Console.WriteLine($"Hash Type:      {c.HashType}");
            Console.WriteLine();
            Console.WriteLine($"Create Report:  {c.CreateReport}");
			Console.WriteLine($"Index Only:     {c.IndexOnly}");
			Console.WriteLine($"Stop On Error:  {c.StopOnError}");
			Console.WriteLine($"Flat Folder:    {c.FlatFolder}");
			Console.WriteLine($"Log Output:     {c.LogOutput}");
			Console.WriteLine($"Verbose Log     {c.VerboseLog}");
			Console.WriteLine();
            Console.WriteLine("Load directory...");
            Console.Out.Flush();
            //Console.SetWindowSize(200, 50);

            // validate
            var request = new CreatePatrolRequest()
			{
				// common
				NamePrefix = c.NamePrefix,
				IndexOnly = c.IndexOnly,
				LogOutput = c.LogOutput,
				VerboseLog = c.VerboseLog,
				StopOnError = c.StopOnError,
				CreateExport = c.CreateExport,
				CreateReport = c.CreateReport,

				// create
				DirectoryUri = c.DirectoryUri,
				SearchFilter = c.SearchFilter,
				Recursive = !c.FlatFolder,
				HashType = c.HashType,
			};

            using var createPatrol = new CreatePatrol(c.Background ? null : Console.Out);
            
			try
            {
				if (c.Background)
				{
					Console.WriteLine($"Start:          {DateTime.Now}");
				}

                var execute = createPatrol.Prepare(request);
                var review = createPatrol.Execute(execute);
                createPatrol.Review(review);

                if (c.Background)
                {
                    Console.WriteLine($"Done:           {DateTime.Now}");
                    Console.WriteLine($"Time:           {review.FinishUtc - execute.StartUtc}");
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

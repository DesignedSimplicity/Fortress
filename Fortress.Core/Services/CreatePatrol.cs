using Fortress.Core.Services.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fortress.Core.Services
{
	public class CreatePatrol : BasePatrol
	{
		public CreatePatrolExecute Validate(CreatePatrolRequest request)
		{
			var execute = new CreatePatrolExecute(request);

			// directory is accessible
			var dir = new DirectoryInfo(String.IsNullOrWhiteSpace(request.DirectoryUri) ? Directory.GetCurrentDirectory() : request.DirectoryUri);
			if (!dir.Exists) throw new DirectoryNotFoundException($"Target folder '{dir.FullName}' does not exist or is not accessible");
			var uri = execute.SourceFolderUri = PathUtils.FixUri(dir.FullName);

			// output name is valid
			string name = request.NamePrefix ?? "";
			if (!String.IsNullOrEmpty(name) && !PathUtils.IsValidFilename(name)) throw new ArgumentOutOfRangeException($"Name prefix {name} is not valid");
			name += $"_{request.StartUtc:yyyyMMdd}-{request.StartUtc:HHmmss}";
			execute.RunName = name;

			// ensure output file is accessible
			if (!request.IndexOnly)
			{
				execute.CreateOutput = true;
				execute.OutputFileUri = Path.Combine(uri, name + "." + request.HashType.ToString().ToLowerInvariant());
				if (File.Exists(execute.OutputFileUri)) throw new IOException($"Output file '{execute.OutputFileUri}' already exists");				
			}

			// ensure log file is accessible
			if (request.LogResult)
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

		public void Prepare()
		{
			// load all directories
			// load all files in each directory
				// apply filter criteria
			// load file details for each file
			// check all pathnames are < 260
		}

		public void Execute()
		{

		}

		public void Output()
		{

		}

		public void Report()
		{

		}

		public void Review()
		{

		}
	}
}

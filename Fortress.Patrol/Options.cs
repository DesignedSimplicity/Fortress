using CommandLine.Text;
using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fortress.Core.Services;

namespace Fortress.Patrol
{
	public class CommonOptions
	{
		[Option('n', "name", HelpText = "Specify the ouput file name prefix")]
		public string? NamePrefix { get; set; }

		[Option('e', "error", HelpText = "Stops processing on first error")]
		public bool StopOnError { get; set; }

		[Option('l', "log", HelpText = "Create a simple text log output")]
		public bool LogOutput { get; set; }

		[Option('r', "report", HelpText = "Create a detailed report in Excel")]
		public bool CreateReport { get; set; }

		[Option('i', "index", HelpText = "Records file properties only")]
		public bool IndexOnly { get; set; }
	}

	[Verb("create", HelpText = "Creates a new hash report")]
	public class CreateOptions : CommonOptions
	{
		[Option('d', "directory", HelpText = "Specify the directory to start in")]
		public string? DirectoryUri { get; set; }

		[Option('h', "hash", HelpText = "Specify the hash algorithm to use", Default = HashType.Md5)]
		public HashType HashType { get; set; }

		[Option('s', "search", HelpText = "Filename wildcard filter")]
		public string? SearchFilter { get; set; }

		[Option('f', "flat", HelpText = "Supresses directory recursion")]
		public bool FlatFolder { get; set; }

	}

	[Verb("verify", HelpText = "Verifies an existing hash report")]
	public class VerifyOptions : CommonOptions
	{
	}
}

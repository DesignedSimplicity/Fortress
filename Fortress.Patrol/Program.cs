using CommandLine;
using Pastel;
using System.Diagnostics;
using System.Drawing;

namespace Fortress.Patrol
{
	internal class Program
	{
		static void Main(string[] args)
		{
			var parser = new Parser(with =>
			{
				with.CaseSensitive = false;
				with.CaseInsensitiveEnumValues = true;
			});

			var engine = new Engine();

			string[] test = { "create", "-d", @"G:\Others", "-lr" };
			var result = parser.ParseArguments<CreateOptions, VerifyOptions>(test)
				.WithParsed<CreateOptions>(x => engine.Create(x))
				.WithParsed<VerifyOptions>(x => engine.Verify(x));

			if (result.Tag == ParserResultType.NotParsed)
			{
				Console.WriteLine($"Error parsing options".Pastel(Color.Red));
			}
		}

	}
}
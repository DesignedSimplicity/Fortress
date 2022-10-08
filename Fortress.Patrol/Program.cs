using CommandLine;
using Pastel;
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

			args = new string[]  { "create", "-d", @"F:\", "-irv", "-e" };
			var result = parser.ParseArguments<CreateOptions, VerifyOptions>(args)
				.WithParsed<CreateOptions>(x => engine.Create(x))
				.WithParsed<VerifyOptions>(x => engine.Verify(x));

			if (result.Tag == ParserResultType.NotParsed)
			{
				Console.WriteLine($"Error parsing options".Pastel(Color.Red));
			}
		}
	}
}
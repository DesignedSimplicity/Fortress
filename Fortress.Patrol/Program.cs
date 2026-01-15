using CommandLine;

namespace Fortress.Patrol
{
	internal class Program
	{
		static void Main(string[] args)
		{
			var parser = new Parser(options =>
			{
				options.AutoHelp = true;
				options.AutoVersion = true;
				options.CaseSensitive = false;
				options.CaseInsensitiveEnumValues = true;
				options.HelpWriter = Console.Out;
			});

			var engine = new Engine();

            //args = new string[]  { "create", "-d", @"C:\Kevin\2025", "-rv", "-ex" };			
            //args = new string[] { "create", "--d", @"\\Blaze\Backup\Backup" };

            var result = parser.ParseArguments<CreateOptions, VerifyOptions>(args)
				.WithParsed<CreateOptions>(x => engine.Create(x))
				.WithParsed<VerifyOptions>(x => engine.Verify(x));

			/*
			if (result.Tag == ParserResultType.NotParsed)
			{
				Console.WriteLine($"Error parsing options".Pastel(Color.Red));
			}
			*/
		}
	}
}
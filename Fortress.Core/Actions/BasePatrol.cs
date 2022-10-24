using Fortress.Core.Entities;
using Pastel;
using System.Drawing;

namespace Fortress.Core.Actions
{
    public abstract class BasePatrol
    {
        protected const string LogFileExtension = ".txt";
        protected const string ReportFileExtension = ".xlsx";

        protected const string WholeNumberFormat = "###,###,###,###,###,##0";
        protected const string SingleDecimalFormat = WholeNumberFormat + ".0";
        protected const string DoubleDecimalFormat = WholeNumberFormat + ".00";
        protected const string TripleDecimalFormat = WholeNumberFormat + ".000";

        protected const string ConsoleSection = "====================================================================================================";
        protected const string ConsoleDivider = "----------------------------------------------------------------------------------------------------";
        protected const string ConsoleFooter = "####################################################################################################";


        protected void ShowException(TextWriter? console, Exception ex, bool verbose = false)
        {
            console?.WriteLine(ex.Message.Pastel(Color.Red));
            if (verbose) console?.WriteLine(ex.ToString().Pastel(Color.DarkRed));
        }

        //TODO: formatting is buggy
        protected string GetPaddedNumberFormat(long maxNumber, int decimals = 0)
        {
            var length = maxNumber.ToString().Length;
            var commas = length / 3;
            var extra = commas;
            var format = WholeNumberFormat;
            if (decimals > 0)
            {
                extra++;
                format += ".";
                for (int i = 0; i < decimals; i++) format += "0";
            }
            return "{0," + (length + extra).ToString("0") + ":" + format + "}";
        }

        protected long GetMaxForFormat(double a, double b, double c)
        {
            return Convert.ToInt64(Math.Max(Math.Max(Math.Round(a), Math.Round(b)), Math.Round(c)));
        }

		protected void ExportFileCsv(IEnumerable<PatrolFile> files, string uri)
		{
			using (StreamWriter output = File.CreateText(uri))
			{
				// write header
				output.Write("\"FileId\",\"FileGuid\",\"FileUri\",\"FileName\",\"DirectoryName\",\"FileExtension\",\"FileSize\",\"FileStatus\",\"Md5\",\"Md5Status\",\"Sha512\",\"Sha512Status\",");
				output.WriteLine("\"CreatedUtc\",\"ModifiedUtc\"");
				//output.WriteLine("\"CreatedUtc\",\"ModifiedUtc\",\"DeclaredUtc\",\"VerifiedUtc\"");

				var count = 0;
				foreach (var file in files)
				{
					count++;
					var path = file.Uri;
					output.Write($"{count},");
					output.Write($"\"{file.Guid}\",");
					output.Write($"\"{path}\",");

					output.Write($"\"{file.Name}\",");
					output.Write($"\"{file.DirectoryName}\",");
					output.Write($"\"{file.Extension}\",");
					output.Write($"{file.Size},");
					output.Write($"\"{file.Status}\",");

					output.Write($"\"{file.Md5}\",");
					output.Write($"\"{file.Md5Status}\",");
					output.Write($"\"{file.Sha512}\",");
					output.Write($"\"{file.Sha512Status}\",");

					output.WriteLine($"\"{file.CreatedUtc}\",\"{file.ModifiedUtc}\"");

					/*
					output.Write($"\"{file.CreatedUtc}\",");
					output.Write($"\"{file.UpdatedUtc}\",");
					output.Write($"\"{file.DeclaredUTC}\",");
					output.Write($"\"{file.VerifiedUTC}\",");
					*/
				}

				// clean up output file
				output.Flush();
				output.Close();
			}
		}
		}
}

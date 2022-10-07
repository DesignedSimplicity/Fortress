using Pastel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fortress.Core.Services
{
	public abstract class BasePatrol
	{
		protected const string LogFileExtension = ".txt";
		protected const string ReportFileExtension = ".xlsx";

		protected const string NumberFormat = "###,###,###,###,###,##0";

		protected const string ConsoleSection = "================================================================================";
		protected const string ConsoleDivider = "--------------------------------------------------------------------------------";
		protected const string ConsoleFooter = "################################################################################";


		protected void ShowException(TextWriter? console, Exception ex, bool verbose = false)
		{
			console?.WriteLine(ex.Message.Pastel(Color.Red));
			if (verbose) console?.WriteLine(ex.ToString().Pastel(Color.DarkRed));
		}
	}
}

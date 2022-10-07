﻿using Pastel;
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

		protected const string WholeNumberFormat = "###,###,###,###,###,##0";
		protected const string SingleDecimalFormat = WholeNumberFormat + ".0";
		protected const string DoubleDecimalFormat = WholeNumberFormat + ".00";
		protected const string TripleDecimalFormat = WholeNumberFormat + ".000";

		protected const string ConsoleSection = "================================================================================";
		protected const string ConsoleDivider = "--------------------------------------------------------------------------------";
		protected const string ConsoleFooter = "################################################################################";


		protected void ShowException(TextWriter? console, Exception ex, bool verbose = false)
		{
			console?.WriteLine(ex.Message.Pastel(Color.Red));
			if (verbose) console?.WriteLine(ex.ToString().Pastel(Color.DarkRed));
		}

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
	}
}

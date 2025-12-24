using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExcelToEnumerable;
using Fortress.Core.Entities;

namespace Fortress.Core.Services;

public sealed class ReadReport
{
	public ReadReport()
	{
	}

	public PatrolSource? LoadFrom(string uri)
	{
		/*
		var sourceExceptions = new List<Exception>();
		return uri.ExcelToEnumerable<PatrolSource>(x => x
			.UsingSheet("Sources").OutputExceptionsTo(sourceExceptions)

			.Property(y => y.ElapsedTime).Ignore()
			.Property(y => y.RootFolder).Ignore()

		).FirstOrDefault();
		*/
		return null;
	}
}


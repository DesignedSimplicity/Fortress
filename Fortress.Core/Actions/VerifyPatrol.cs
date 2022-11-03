using Fortress.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fortress.Core.Actions
{
	public class VerifyPatrol : BasePatrol
	{
		//private StreamWriter? _log;
		private TextWriter? _console;

		public VerifyPatrol(TextWriter? console = null)
		{
			_console = console;
		}
	}
}

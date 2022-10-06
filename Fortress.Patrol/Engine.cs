using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fortress.Patrol
{
	internal class Engine
	{
		public void Verify(VerifyOptions v)
		{
			Console.WriteLine($"Verify");
		}

		public void Create(CreateOptions c)
		{
			Console.WriteLine($"Create");
		}
	}
}

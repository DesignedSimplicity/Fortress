using Fortress.Core.Entities;
using Fortress.Core.Services;
using System.Diagnostics;

namespace Fortress.Patrol
{
	internal class Program
	{
		static void Main(string[] args)
		{
			var engine = new Engine();
			engine.Start();
		}

	}

	public class Engine
	{
		public void Start()
		{
			var s = new Stopwatch();
			s.Start();
			var q = new QueryFolders(this.FolderStateChange);
			//var q = new QueryFolders(Console.Out);
			q.LoadAllFolders(@"G:\Others");
			s.Stop();
			Console.WriteLine(s.ElapsedMilliseconds);
		}

		public void FolderStateChange(PatrolFolderState state)
		{
			Console.WriteLine(state.Folder.Uri);
		}
	}
}
using Fortress.Core.Entities;
using Fortress.Core.Services;

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
			var q = new QueryFolders(this.FolderStateChange);
			q.LoadAllFolders(@"N:\");
		}

		public void FolderStateChange(PatrolFolderState state)
		{
			Console.WriteLine(state.Folder.Uri);
		}
	}
}
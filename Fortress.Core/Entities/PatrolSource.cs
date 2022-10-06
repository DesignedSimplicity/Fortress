using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fortress.Core.Entities
{
	public class PatrolSource
	{
		public PatrolFolder Root { get; set; }
		public List<PatrolFolder> AllFolders { get; set; }
		public List<PatrolFile> AllFiles { get; set; }
		public TimeSpan ElapsedTime { get; set; }

		public PatrolSource(string uri, List<PatrolFolder>? folders = null, List<PatrolFile>? files = null)
		{
			Root = new PatrolFolder(uri);
			AllFolders = folders ?? new List<PatrolFolder>();
			AllFiles = files ?? new List<PatrolFile>();
		}

		public PatrolSource(PatrolFolder root, List<PatrolFolder>? folders = null, List<PatrolFile>? files = null)
		{
			Root = root;
			AllFolders = folders ?? new List<PatrolFolder>();
			AllFiles = files ?? new List<PatrolFile>();
		}
	}
}

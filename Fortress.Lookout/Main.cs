using Fortress.Core;
using Fortress.Core.Entities;
using Fortress.Core.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;

namespace Fortress.Lookout
{
	public partial class Main : Form
	{
		private CancellationTokenSource _cancel = new CancellationTokenSource();
		private Progress<PatrolFolderState> _folderProgress;
		private QueryFolders _query;
		private Dictionary<string, TreeNode> _nodes = new Dictionary<string, TreeNode>();

		public Main()
		{
			InitializeComponent();

			txtStart.Text = @"G:\Others";

			_folderProgress = new Progress<PatrolFolderState>();
			_query = new QueryFolders(_folderProgress);

			/*
			_background.WorkerReportsProgress = true;
			_background.DoWork += (object? sender, DoWorkEventArgs e) =>
			{
				
			};
			_background.ProgressChanged += (object? sender, ProgressChangedEventArgs e) =>
			{
				PatrolFolder? folder = e.UserState as PatrolFolder;
				treeView.Nodes.Add(folder?.Name??"");
			};
			*/

			_folderProgress.ProgressChanged += (s, state) =>
			{
				var node = new TreeNode(state.Folder.Name);
				_nodes.Add(state.Folder.Uri.ToLowerInvariant(), node);
				var directory = PathUtils.GetParentPath(state.Folder.Uri).ToLowerInvariant();
				if (_nodes.ContainsKey(directory))
					_nodes[directory].Nodes.Add(node);
				else
					treeView.Nodes.Add(node);

				lblStatus.Text = $"Loading folder #{_nodes.Count} {state.Folder.Uri}";
			};
		}

		private async void cmdStart_Click(object sender, EventArgs e)
		{
			if (!txtStart.Enabled)
			{
				_cancel.Cancel();
			}
			//cmdStart.Enabled = false;
			txtStart.Enabled = false;

			var folders = await _query.LoadAllFoldersAsync(txtStart.Text, true, _cancel.Token);
			lblStatus.Text = $"Loaded {folders.Count} folders from {txtStart.Text}";

			//cmdStart.Enabled = true;
			txtStart.Enabled = true;
		}
	}
}

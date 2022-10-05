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
using static System.Windows.Forms.AxHost;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;

namespace Fortress.Lookout
{
	public partial class Main : Form
	{
		private Dictionary<string, TreeNode> _nodeCache;
		private Dictionary<string, List<PatrolFile>> _fileCache;
		private Progress<PatrolFolderState> _progressFolders;
		private CancellationTokenSource _cancelFolders;
		private CancellationTokenSource _cancelFiles;
		private QueryFolders _queryFolders;
		private QueryFiles _queryFiles;

		public Main()
		{
			InitializeComponent();

			cmdStop.Enabled = false;
			txtStart.Text = @"G:\Others";

			_nodeCache = new Dictionary<string, TreeNode>();
			_fileCache = new Dictionary<string, List<PatrolFile>>();
			_progressFolders = new Progress<PatrolFolderState>();
			_cancelFolders = new CancellationTokenSource();
			_cancelFiles = new CancellationTokenSource();
			_queryFolders = new QueryFolders(_progressFolders);
			_queryFiles = new QueryFiles();


			_progressFolders.ProgressChanged += FolderProgress_ProgressChanged;

			cmdStart.Click += Start_Click;
			cmdStop.Click += Stop_Click;

			treeView.BeforeSelect += TreeView_BeforeSelect;
			treeView.AfterSelect += TreeView_AfterSelect;
		}

		private void TreeView_AfterSelect(object? sender, TreeViewEventArgs e)
		{
		}

		private async void TreeView_BeforeSelect(object? sender, TreeViewCancelEventArgs e)
		{
			var selectedNode = e.Node;
			var uri = selectedNode?.Tag.ToString();
			if (!String.IsNullOrEmpty(uri))
			{
				var key = uri.ToLowerInvariant();

				// cancel last query
				_cancelFiles.Cancel();
				listView.Items.Clear();
				listView.Enabled = false;
				listView.Tag = key;

				// create new query
				_cancelFiles = new CancellationTokenSource();
				if (!_fileCache.ContainsKey(key))
				{
					var files = await _queryFiles.LoadFilesAsync(uri, _cancelFolders.Token);
					if (!_fileCache.ContainsKey(key) && files.Count > 0)
						_fileCache.Add(key, files);
				}

				// populate listview
				if (listView.Tag?.ToString() == key && _fileCache.ContainsKey(key))
				{
					listView.BeginUpdate();
					foreach (var file in _fileCache[key])
					{
						listView.Items.Add(file.Name);
					}

					listView.Enabled = true;
					listView.EndUpdate();
				}
			}
		}

		private void FolderProgress_ProgressChanged(object? sender, PatrolFolderState e)
		{
			var key = e.Folder.Uri.ToLowerInvariant();
			var node = new TreeNode(e.Folder.Name);
			node.Tag = key;
			_nodeCache.Add(key, node);
			var directory = PathUtils.GetParentPath(e.Folder.Uri).ToLowerInvariant();
			if (_nodeCache.ContainsKey(directory))
				_nodeCache[directory].Nodes.Add(node);
			else
				treeView.Nodes.Add(node);

			lblStatus.Text = $"Loading folder #{_nodeCache.Count} {e.Folder.Uri}";
		}

		private async void Start_Click(object? sender, EventArgs e)
		{
			cmdStop.Enabled = true;
			cmdStart.Enabled = false;
			txtStart.Enabled = false;

			_cancelFolders = new CancellationTokenSource();
			_nodeCache = new Dictionary<string, TreeNode>();
			var folders = await _queryFolders.LoadAllFoldersAsync(txtStart.Text, _cancelFolders.Token);
			lblStatus.Text = $"Loaded {folders.Count} folders from {txtStart.Text}";

			cmdStop.Enabled = false;
			cmdStart.Enabled = true;
			txtStart.Enabled = true;
		}

		private void Stop_Click(object? sender, EventArgs e)
		{
			_cancelFolders.Cancel();
		}
	}
}

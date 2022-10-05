using Fortress.Core;
using Fortress.Core.Entities;
using Fortress.Core.Services;
using System;
using System.Collections.Concurrent;
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
		private ConcurrentDictionary<string, PatrolFile> _fileCache;
		private Dictionary<string, TreeNode> _nodeCache;
		private Progress<PatrolFolderState> _progressFolders;
		private Progress<PatrolFileState> _progressFiles;
		private CancellationTokenSource _cancel;
		private QueryFolders _queryFolders;
		private QueryFiles _queryFiles;

		public Main()
		{
			InitializeComponent();

			cmdStop.Enabled = false;
			txtStart.Text = @"G:\Others";


			_fileCache = new ConcurrentDictionary<string, PatrolFile>();
			_nodeCache = new Dictionary<string, TreeNode>();
			_progressFolders = new Progress<PatrolFolderState>();
			_progressFiles = new Progress<PatrolFileState>();
			_queryFolders = new QueryFolders(_progressFolders);
			_queryFiles = new QueryFiles(_progressFiles);
			_cancel = new CancellationTokenSource();


			cmdStart.Click += Start_Click;
			cmdStop.Click += Stop_Click;

			treeView.BeforeSelect += TreeView_BeforeSelect;
			treeView.AfterSelect += TreeView_AfterSelect;

			_progressFolders.ProgressChanged += FolderProgress_ProgressChanged;
			_progressFiles.ProgressChanged += FileProgress_ProgressChanged;
		}

		private void TreeView_AfterSelect(object? sender, TreeViewEventArgs e)
		{
			var selectedNode = e.Node;
			var uri = selectedNode?.Tag.ToString();
			if (!String.IsNullOrEmpty(uri))
			{
				var key = uri.ToLowerInvariant();

				// cancel last query
				listView.Items.Clear();
				listView.Enabled = false;
				listView.Tag = key;

				// populate listview
				listView.BeginUpdate();
				foreach (var file in _fileCache.Values.Where(x => String.Equals(PathUtils.GetParentPath(x.Uri), uri, StringComparison.InvariantCultureIgnoreCase)).OrderBy(x => x.Name))
				{
					var item = listView.Items.Add(file.Name);
					switch (file.Status)
					{
						case PatrolFileStatus.Exception:
							item.ForeColor = Color.DarkRed; break;
						case PatrolFileStatus.NotFound:
							item.ForeColor = Color.DarkOrange; break;
						case PatrolFileStatus.Verified:
							item.ForeColor = Color.DarkGreen; break;
					}
				}
				listView.Enabled = true;
				listView.EndUpdate();
			}
		}

		private void TreeView_BeforeSelect(object? sender, TreeViewCancelEventArgs e)
		{			
		}

		private void FileProgress_ProgressChanged(object? sender, PatrolFileState e)
		{
			var file = _fileCache.AddOrUpdate(e.File.Uri.ToLowerInvariant(), e.File, (string s, PatrolFile f) => e.File);
			file.Status = e.Status;
			lblStatus.Text = $"Loading file #{_fileCache.Count} {e.File.Uri}";
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

			var uri = txtStart.Text;
			_cancel = new CancellationTokenSource();
			_nodeCache = new Dictionary<string, TreeNode>();
			var folders = await _queryFolders.LoadAllFoldersAsync(uri, _cancel.Token);
			lblStatus.Text = $"Loaded {folders.Count} folders from {uri}";

			cmdStop.Enabled = false;
			cmdStart.Enabled = true;
			txtStart.Enabled = true;
			
			//_fileCache = 
			await _queryFiles.LoadFilesAsync(uri, true, _cancel.Token);
			lblStatus.Text = $"Loaded {_fileCache.Count} files from {uri}";

			var hashFiles = new HashFiles(_progressFiles);
			await hashFiles.HashAllFilesAsync(_fileCache.Values.OrderBy(x => x.Uri).ToList(), _cancel.Token);
			lblStatus.Text = $"Hashed {_fileCache.Count} files from {uri}";
		}

		private void Stop_Click(object? sender, EventArgs e)
		{
			_cancel.Cancel();
		}
	}
}

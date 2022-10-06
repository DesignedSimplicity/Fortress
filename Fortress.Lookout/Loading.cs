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
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Fortress.Lookout
{
	public partial class Loading : Form
	{
		private Progress<PatrolFolderState> _progressFolders;
		private Progress<PatrolFileState> _progressFiles;
		private Progress<string> _progressTotal;
		private CancellationTokenSource _cancel;
		private QueryFolders _queryFolders;
		private QueryFiles _queryFiles;

		private const int _folderCountIndex = 1;
		private const int _fileCountIndex = 2;
		private const int _fileSizeIndex = 3;
		private const string _numberFormat = "###,###,###,###,###,###0";

		public Loading()
		{
			InitializeComponent();

			_progressFolders = new Progress<PatrolFolderState>();
			_progressFiles = new Progress<PatrolFileState>();
			_progressTotal = new Progress<string>();
			_queryFolders = new QueryFolders(_progressFolders);
			_queryFiles = new QueryFiles(_progressFiles);
			_cancel = new CancellationTokenSource();

			_progressFolders.ProgressChanged += FolderProgress_ProgressChanged;
			_progressFiles.ProgressChanged += FileProgress_ProgressChanged;
			_progressTotal.ProgressChanged += TotalProgress_ProgressChanged;

			this.FormClosing += Loading_FormClosing;
		}

		private void Loading_FormClosing(object? sender, FormClosingEventArgs e)
		{
			_cancel.Cancel();
		}

		public void StartLoad(string uri)
		{
			_cancel = new CancellationTokenSource();

			Task.Run(() => {
				var root = _queryFolders.LoadFolder(uri);

				var folders = LoadSubFolders(root);
				folders.Add(root);
				
				var files = LoadAllFiles(folders.OrderBy(x => x.Uri));
				
				IProgress<string> progress = _progressTotal as IProgress<string>;
				progress.Report("");
			});

			this.ShowDialog();
		}

		private List<PatrolFile> LoadAllFiles(IEnumerable<PatrolFolder> folders)
		{
			var list = new List<PatrolFile>();

			foreach (var sub in folders)
			{
				var files = _queryFiles.LoadFiles(sub.Uri, false, _cancel.Token);
				list.AddRange(files);
			}

			return list;
		}

		private List<PatrolFolder> LoadSubFolders(PatrolFolder folder)
		{
			var list = new List<PatrolFolder>();
			
			var folders = _queryFolders.LoadFolders(folder.Uri, _cancel.Token);
			list.AddRange(folders);

			foreach (var sub in folders)
			{
				list.AddRange(LoadSubFolders(sub));
			}

			return list;
		}

		private void FolderProgress_ProgressChanged(object? sender, PatrolFolderState e)
		{
			lblStatus.Text = $"Loading folder #{listLog.Items.Count} {e.Folder.Uri}";
			
			// add folder to list
			var uri = e.Folder.Uri;
			var item = listLog.Items.Add(uri);
			item.Name = uri;
			item.Tag = uri;
			item.SubItems.Add("0");
			item.SubItems.Add("0");
			item.SubItems.Add("0");

			// update parent
			var parent = FindFolderIten(e.Folder.Uri);
			if (parent != null) UpdateTotal(parent, _folderCountIndex);
		}

		private void FileProgress_ProgressChanged(object? sender, PatrolFileState e)
		{
			// update folder
			var parent = FindFolderIten(e.File.Uri);
			if (parent != null)
			{
				lblStatus.Text = $"Loading files for folder {parent.Name}";
				barProgress.Value = 100 * (parent.Index + 1) / listLog.Items.Count;

				parent.EnsureVisible();
				UpdateTotal(parent, _fileCountIndex);
				UpdateTotal(parent, _fileSizeIndex, e.File.Size);
			}
		}

		private void TotalProgress_ProgressChanged(object? sender, string e)
		{
			// total
			lblStatus.Text = $"Done!";
		}

		private ListViewItem? FindFolderIten(string uri)
		{
			var parent = PathUtils.FixUri(PathUtils.GetParentPath(uri));
			if (listLog.Items.ContainsKey(parent))
				return listLog.Items[parent];
			else
				return null;
		}

		private void UpdateTotal(ListViewItem item, int index, long increment = 1)
		{
			var count = Convert.ToInt64(item.SubItems[index].Text.Replace(",", "")) + increment;
			item.SubItems[index].Text = count.ToString(_numberFormat);
		}
	}
}

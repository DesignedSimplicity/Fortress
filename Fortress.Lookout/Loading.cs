using Fortress.Core.Common;
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
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Fortress.Lookout
{
	public partial class Loading : Form
	{
		private Progress<PatrolFolder> _progressFolders;
		private Progress<PatrolFolder> _progressFiles;
		private Progress<PatrolSource> _progressTotal;
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

			//_progressFolders = new Progress<PatrolFolderState>();
			_progressFolders = new Progress<PatrolFolder>();
			_progressFiles = new Progress<PatrolFolder>();
			_progressTotal = new Progress<PatrolSource>();
			_queryFolders = new QueryFolders();
			_queryFiles = new QueryFiles();
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

		public PatrolSource StartLoad(string uri)
		{
			this.Text = $"Loading {uri}";
			_cancel = new CancellationTokenSource();
			PatrolSource source = new PatrolSource(uri);

			Task.Run(() => {
				var timer = new Stopwatch();
				timer.Start();

				var root = _queryFolders.LoadFolder(uri);

				var folders = LoadSubFolders(root);
				folders.Add(root);
				
				var files = LoadAllFiles(folders.OrderBy(x => x.Uri));

				timer.Stop();
				source.RootFolder = root;
				source.AllFolders = folders;
				source.AllFiles = files;
				source.ElapsedTime = timer.Elapsed;

				(_progressTotal as IProgress<PatrolSource>).Report(source);
			});

			this.ShowDialog();

			return source;
		}

		private List<PatrolFile> LoadAllFiles(IEnumerable<PatrolFolder> folders)
		{
			var list = new List<PatrolFile>();

			foreach (var sub in folders)
			{
				var files = _queryFiles.LoadFiles(sub.Uri, null, false, _cancel.Token);
				sub.PatrolFiles.AddRange(files);
				list.AddRange(files);
				(_progressFiles as IProgress<PatrolFolder>)?.Report(sub);
			}

			return list;
		}

		private List<PatrolFolder> LoadSubFolders(PatrolFolder folder)
		{
			var list = new List<PatrolFolder>();
			
			var folders = _queryFolders.LoadFolders(folder.Uri, _cancel.Token);
			folder.PatrolFolders.AddRange(folders);
			list.AddRange(folders);
			(_progressFolders as IProgress<PatrolFolder>)?.Report(folder);


			foreach (var sub in folders)
			{
				list.AddRange(LoadSubFolders(sub));
				(_progressFolders as IProgress<PatrolFolder>)?.Report(sub);
			}

			return list;
		}
		private void FolderProgress_ProgressChanged(object? sender, PatrolFolder folder)
		{
			listLog.BeginUpdate();

			lblStatus.Text = $"Loading folder #{listLog.Items.Count} {folder.Uri}";

			// add folder to list
			var uri = folder.Uri;
			var item = listLog.Items.Add(uri);
			item.Name = uri;
			item.Tag = uri;
			item.SubItems.Add(folder.PatrolFolders.Count.ToString(_numberFormat));
			item.SubItems.Add("0");
			item.SubItems.Add("0");

			listLog.EndUpdate();
		}

		private void FolderProgress_ProgressChanged2(object? sender, PatrolFolderState folder)
		{
			listLog.BeginUpdate();

			lblStatus.Text = $"Loading folder #{listLog.Items.Count} {folder.Folder.Uri}";
			
			// add folder to list
			var uri = folder.Folder.Uri;			
			var item = listLog.Items.Add(uri);
			item.Name = uri;
			item.Tag = uri;
			item.SubItems.Add("0");
			item.SubItems.Add("0");
			item.SubItems.Add("0");

			// update parent
			var parent = FindFolderIten(folder.Folder.Uri);
			if (parent != null) UpdateTotal(parent, _folderCountIndex);

			listLog.EndUpdate();
		}

		private void FileProgress_ProgressChanged(object? sender, PatrolFolder folder)
		{
			listLog.BeginUpdate();

			// ensure list is sorted
			if (listLog.Sorting == SortOrder.None) listLog.Sorting = SortOrder.Ascending;

			// update folder
			ListViewItem? item = FindFolderIten(folder.Uri);
			if (item != null)
			{
				lblStatus.Text = $"Loaded files for folder {item.Name}";
				barProgress.Value = 100 * (item.Index + 1) / listLog.Items.Count;

				item.EnsureVisible();
				item.SubItems[_fileCountIndex].Text = folder.PatrolFiles.Count.ToString(_numberFormat);
				item.SubItems[_fileSizeIndex].Text = folder.PatrolFiles.Sum(x => x.Size).ToString(_numberFormat);
			}

			listLog.EndUpdate();
		}

		private ListViewItem? lastItem = null;
		private void FileProgress_ProgressChanged2(object? sender, PatrolFileState file)
		{
			listLog.BeginUpdate();

			// ensure list is sorted
			if (listLog.Sorting == SortOrder.None) listLog.Sorting = SortOrder.Ascending;

			// update folder
			ListViewItem? parent = null;
			if (lastItem == null || lastItem.Name != PathUtils.FixUri(PathUtils.GetParentPath(file.File.Uri)))
				parent = FindFolderIten(file.File.Uri);
			else
				parent = lastItem;

			if (parent != null)
			{
				lblStatus.Text = $"Loading files for folder {parent.Name}";
				barProgress.Value = 100 * (parent.Index + 1) / listLog.Items.Count;

				parent.EnsureVisible();
				UpdateTotal(parent, _fileCountIndex);
				UpdateTotal(parent, _fileSizeIndex, file.File.Size);
			}
			lastItem = parent;

			listLog.EndUpdate();
		}

		private void TotalProgress_ProgressChanged(object? sender, PatrolSource source)
		{
			// show completed totals
			var item = listLog.Items.Insert(0, "Totals");
			item.Font = new Font(item.Font, FontStyle.Bold);
			item.BackColor = listLog.ForeColor;
			item.ForeColor = listLog.BackColor;
			item.SubItems.Add(source.AllFolders.Count.ToString(_numberFormat));
			item.SubItems.Add(source.AllFiles.Count.ToString(_numberFormat));
			item.SubItems.Add(source.AllFiles.Sum(x => x.Size).ToString(_numberFormat));
			item.EnsureVisible();

			lblStatus.Text = $"Elapsed Time {source.ElapsedTime.Hours:00}:{source.ElapsedTime.Minutes:00}.{source.ElapsedTime.Seconds:00}";
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

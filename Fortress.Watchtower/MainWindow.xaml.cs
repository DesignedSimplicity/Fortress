using Fortress.Core;
using Fortress.Core.Entities;
using Fortress.Core.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Fortress.Watchtower
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private ObservableCollection<PatrolFolder> _folders = new ObservableCollection<PatrolFolder>();
		//private List<PatrolFolder> _folders = new List<PatrolFolder>();
		private Progress<PatrolFolderState> _progressFolders;
		private Progress<PatrolFileState> _progressFiles;
		private Progress<PatrolSource> _progressTotal;
		private CancellationTokenSource _cancel;
		private QueryFolders _queryFolders;
		private QueryFiles _queryFiles;
		public MainWindow()
		{
			InitializeComponent();

			//_progressFolders = new Progress<PatrolFolderState>();
			_progressFolders = new Progress<PatrolFolderState>();
			_progressFiles = new Progress<PatrolFileState>();
			_progressTotal = new Progress<PatrolSource>();
			_queryFolders = new QueryFolders(_progressFolders);
			_queryFiles = new QueryFiles(_progressFiles);
			_cancel = new CancellationTokenSource();

			_progressFolders.ProgressChanged += FolderProgress_ProgressChanged;
			_progressFiles.ProgressChanged += FileProgress_ProgressChanged;
			//_progressTotal.ProgressChanged += TotalProgress_ProgressChanged;

			listView.ItemsSource = _folders;
			CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(listView.ItemsSource);
			view.SortDescriptions.Add(new SortDescription("Uri", ListSortDirection.Ascending));

			Task.Run(() => StartLoad(@"G:\Others"));
		}

		public PatrolSource StartLoad(string uri)
		{
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
				source.Root = root;
				source.AllFolders = folders;
				source.AllFiles = files;
				source.ElapsedTime = timer.Elapsed;
			});

			return source;
		}

		private List<PatrolFile> LoadAllFiles(IEnumerable<PatrolFolder> folders)
		{
			var list = new List<PatrolFile>();

			foreach (var sub in folders)
			{
				var files = _queryFiles.LoadFiles(sub.Uri, false, _cancel.Token);
				sub.PatrolFiles.AddRange(files);
				list.AddRange(files);
			}

			return list;
		}

		private List<PatrolFolder> LoadSubFolders(PatrolFolder folder)
		{
			var list = new List<PatrolFolder>();

			var folders = _queryFolders.LoadFolders(folder.Uri, _cancel.Token);
			folder.PatrolFolders.AddRange(folders);
			list.AddRange(folders);

			foreach (var sub in folders)
			{
				list.AddRange(LoadSubFolders(sub));
			}

			return list;
		}


		private void FolderProgress_ProgressChanged(object? sender, PatrolFolderState state)
		{
			_folders.Add(state.Folder);
		}

		private void FileProgress_ProgressChanged(object? sender, PatrolFileState file)
		{
		}
	}
}

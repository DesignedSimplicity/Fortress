namespace Fortress.Lookout
{
	partial class Loading
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.barProgress = new System.Windows.Forms.ProgressBar();
			this.lblStatus = new System.Windows.Forms.Label();
			this.listLog = new System.Windows.Forms.ListView();
			this.colUri = new System.Windows.Forms.ColumnHeader();
			this.colFolders = new System.Windows.Forms.ColumnHeader();
			this.colFiles = new System.Windows.Forms.ColumnHeader();
			this.colFileSize = new System.Windows.Forms.ColumnHeader();
			this.SuspendLayout();
			// 
			// barProgress
			// 
			this.barProgress.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.barProgress.Location = new System.Drawing.Point(0, 338);
			this.barProgress.Name = "barProgress";
			this.barProgress.Size = new System.Drawing.Size(784, 23);
			this.barProgress.TabIndex = 1;
			// 
			// lblStatus
			// 
			this.lblStatus.Dock = System.Windows.Forms.DockStyle.Top;
			this.lblStatus.Location = new System.Drawing.Point(0, 0);
			this.lblStatus.Name = "lblStatus";
			this.lblStatus.Size = new System.Drawing.Size(784, 20);
			this.lblStatus.TabIndex = 2;
			// 
			// listLog
			// 
			this.listLog.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colUri,
            this.colFolders,
            this.colFiles,
            this.colFileSize});
			this.listLog.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listLog.FullRowSelect = true;
			this.listLog.GridLines = true;
			this.listLog.Location = new System.Drawing.Point(0, 20);
			this.listLog.Name = "listLog";
			this.listLog.ShowGroups = false;
			this.listLog.Size = new System.Drawing.Size(784, 318);
			this.listLog.TabIndex = 3;
			this.listLog.UseCompatibleStateImageBehavior = false;
			this.listLog.View = System.Windows.Forms.View.Details;
			// 
			// colUri
			// 
			this.colUri.Text = "Uri";
			this.colUri.Width = 480;
			// 
			// colFolders
			// 
			this.colFolders.Text = "Folders";
			this.colFolders.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.colFolders.Width = 64;
			// 
			// colFiles
			// 
			this.colFiles.Text = "Files";
			this.colFiles.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.colFiles.Width = 64;
			// 
			// colFileSize
			// 
			this.colFileSize.Text = "File Size";
			this.colFileSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.colFileSize.Width = 150;
			// 
			// Loading
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(784, 361);
			this.Controls.Add(this.listLog);
			this.Controls.Add(this.lblStatus);
			this.Controls.Add(this.barProgress);
			this.DoubleBuffered = true;
			this.Name = "Loading";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Loading...";
			this.ResumeLayout(false);

		}

		#endregion

		private ProgressBar barProgress;
		private Label lblStatus;
		private ListView listLog;
		private ColumnHeader colUri;
		private ColumnHeader colFolders;
		private ColumnHeader colFiles;
		private ColumnHeader colFileSize;
	}
}
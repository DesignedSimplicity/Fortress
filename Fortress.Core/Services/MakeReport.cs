﻿using OfficeOpenXml.Style;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fortress.Core.Entities;

namespace Fortress.Core.Services
{
	public sealed class MakeReport : IDisposable
	{
		private const string PercentFormat = "#0.0%";
		private const string NumberFormat = "###,###,##0";
		private const string MoneyFormat = "$###,###,##0.00";
		private const string DateFormat = "yyyy-MM-dd HH:mm:ss";

		private readonly ExcelPackage _package;
		private readonly ExcelWorksheet _sources;
		private readonly ExcelWorksheet _folders;
		private readonly ExcelWorksheet _files;

		public MakeReport()
		{
			ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
			_package = new ExcelPackage();
			_sources = NewWorksheet("Sources");
			_folders = NewWorksheet("Folders");
			_files = NewWorksheet("Files");
		}

		public void SaveAs(string uri)
		{
			_package.SaveAs(uri);
		}

		public void Dispose()
		{
			_files.Dispose();
			_folders.Dispose();
			_sources.Dispose();
			_package.Dispose();
		}

		private ExcelWorksheet NewWorksheet(string name)
		{
			var ws = _package.Workbook.Worksheets.Add(name);
			ws.Row(1).Style.Fill.PatternType = ExcelFillStyle.Solid;
			ws.Row(1).Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Black);
			ws.Row(1).Style.Font.Color.SetColor(System.Drawing.Color.White);
			return ws;
		}

		public void PopulateSource(PatrolSource source)
		{
			PopulateSources(new List<PatrolSource> { source });
		}

		public void PopulateSources(IEnumerable<PatrolSource> sources)
		{
			var row = 1;
			var col = 1;
			_sources.Cells[row, col++].Value = "ExId";
			_sources.Cells[row, col++].Value = "Guid";
			_sources.Cells[row, col++].Value = "SystemName";
			_sources.Cells[row, col++].Value = "PatrolName";
			_sources.Cells[row, col++].Value = "PatrolType";
			_sources.Cells[row, col++].Value = "HashType";
			_sources.Cells[row, col++].Value = "SourcePatrolUri";
			_sources.Cells[row, col++].Value = "TargetFolderUri";
			_sources.Cells[row, col++].Value = "TotalFileSize";
			_sources.Cells[row, col++].Value = "TotalFileCount";
			_sources.Cells[row, col++].Value = "TotalFolderCount";
			_sources.Cells[row, col++].Value = "TotalSeconds";
			_sources.Cells[row, col++].Value = "CreatedUTC";
			_sources.Cells[row, col++].Value = "UpdatedUTC";

			foreach (var source in sources)
			{
				row++;
				col = 1;
				_sources.Cells[row, col++].Value = row - 1;
				/*
				_sources.Cells[row, col++].Value = source.Guid.ToString();
				_sources.Cells[row, col++].Value = source.SystemName;
				_sources.Cells[row, col++].Value = source.PatrolName;
				_sources.Cells[row, col++].Value = source.SourceType.ToString();
				_sources.Cells[row, col++].Value = source.HashType.ToString();
				_sources.Cells[row, col++].Value = source.SourcePatrolUri;
				_sources.Cells[row, col++].Value = source.TargetFolderUri;
				_sources.Cells[row, col++].Value = source.TotalFileSize;
				_sources.Cells[row, col++].Value = source.TotalFileCount;
				_sources.Cells[row, col++].Value = source.TotalFolderCount;
				_sources.Cells[row, col++].Value = source.TotalSeconds;
				_sources.Cells[row, col++].Value = source.Created;
				_sources.Cells[row, col++].Value = source.Updated;
				*/
			}

			_sources.Column(1).Style.Numberformat.Format = NumberFormat;
			_sources.Column(9).Style.Numberformat.Format = NumberFormat;
			_sources.Column(10).Style.Numberformat.Format = NumberFormat;
			_sources.Column(11).Style.Numberformat.Format = NumberFormat;
			_sources.Column(12).Style.Numberformat.Format = NumberFormat;
			_sources.Column(13).Style.Numberformat.Format = DateFormat;
			_sources.Column(14).Style.Numberformat.Format = DateFormat;
		}

		public void PopulateFolders(IEnumerable<PatrolFolder> folders)
		{
			var row = 1;
			var col = 1;
			_folders.Cells[row, col++].Value = "ExId";
			_folders.Cells[row, col++].Value = "Guid";
			_folders.Cells[row, col++].Value = "FullUri";
			_folders.Cells[row, col++].Value = "DirectoryName";
			_folders.Cells[row, col++].Value = "TotalFileCount";
			_folders.Cells[row, col++].Value = "TotalFileSize";
			_folders.Cells[row, col++].Value = "CreatedUTC";
			_folders.Cells[row, col++].Value = "UpdatedUTC";

			foreach (var folder in folders)
			{
				row++;
				col = 1;
				_folders.Cells[row, col++].Value = row - 1;
				_folders.Cells[row, col++].Value = folder.Guid;
				_folders.Cells[row, col++].Value = folder.Uri;
				_folders.Cells[row, col++].Value = folder.Name;
				_folders.Cells[row, col++].Value = folder.PatrolFiles.Count;
				_folders.Cells[row, col++].Value = folder.TotalFileSize;
				_folders.Cells[row, col++].Value = folder.Created;
				_folders.Cells[row, col++].Value = folder.Updated;
			}

			_folders.Column(1).Style.Numberformat.Format = NumberFormat;
			_folders.Column(5).Style.Numberformat.Format = NumberFormat;
			_folders.Column(6).Style.Numberformat.Format = NumberFormat;
			_folders.Column(7).Style.Numberformat.Format = DateFormat;
			_folders.Column(8).Style.Numberformat.Format = DateFormat;
		}

		public void PopulateFiles(IEnumerable<PatrolFile> files)
		{
			var row = 1;
			var col = 1;
			_files.Cells[row, col++].Value = "ExId";
			_files.Cells[row, col++].Value = "Guid";
			_files.Cells[row, col++].Value = "FullUri";
			_files.Cells[row, col++].Value = "DirectoryName";
			_files.Cells[row, col++].Value = "FileName";
			_files.Cells[row, col++].Value = "FileExtension";
			_files.Cells[row, col++].Value = "FileSize";
			_files.Cells[row, col++].Value = "FileStatus";
			_files.Cells[row, col++].Value = "Md5";
			_files.Cells[row, col++].Value = "Md5Status";
			_files.Cells[row, col++].Value = "Sha512";
			_files.Cells[row, col++].Value = "Sha512Status";
			_files.Cells[row, col++].Value = "CreatedUTC";
			_files.Cells[row, col++].Value = "UpdatedUTC";
			_files.Cells[row, col++].Value = "VerifiedUTC";

			foreach (var file in files)
			{
				row++;
				col = 1;
				_files.Cells[row, col++].Value = row - 1;
				_files.Cells[row, col++].Value = file.Guid;
				_files.Cells[row, col++].Value = file.Uri;
				_files.Cells[row, col++].Value = ""; // file.Directory;
				_files.Cells[row, col++].Value = file.Name;
				_files.Cells[row, col++].Value = ""; // file.Extension;
				_files.Cells[row, col++].Value = file.Size;
				_files.Cells[row, col++].Value = file.Status == PatrolFileStatus.Default ? "" : file.Status.ToString();
				_files.Cells[row, col++].Value = ""; // file.Md5;
				_files.Cells[row, col++].Value = "";//file.Md5Status == HashStatus.Unknown ? "" : file.Md5Status.ToString();
				_files.Cells[row, col++].Value = ""; // file.Sha512;
				_files.Cells[row, col++].Value = "";// file.Sha512Status == HashStatus.Unknown ? "" : file.Sha512Status.ToString();
				_files.Cells[row, col++].Value = file.Created;
				_files.Cells[row, col++].Value = file.Updated;
				_files.Cells[row, col++].Value = ""; // file.LastVerified;
			}

			// do auto formatting
			//AutoFormatColumns(7, NumberFormat);
			_files.Column(7).Style.Numberformat.Format = NumberFormat;

			// do manual formatting
			//_worksheet.Column(_col).Style.Numberformat.Format = _percentFormat;
			//_worksheet.Column(_col).Style.Numberformat.Format = _moneyFormat;
			_files.Column(13).Style.Numberformat.Format = DateFormat;
			_files.Column(14).Style.Numberformat.Format = DateFormat;
			_files.Column(15).Style.Numberformat.Format = DateFormat;

			// auto size columns
			//AutoFitColumns(_col - 1);
		}

		private void AutoFormatColumns(int count, string format)
		{
			for (var index = 1; index <= count; index++)
			{
				_files.Column(index).Style.Numberformat.Format = format;
			}
		}

		private void AutoFitColumns(int count)
		{
			for (var index = 1; index <= count; index++)
			{
				_files.Column(index).AutoFit();
			}
		}
	}
}
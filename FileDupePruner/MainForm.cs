///////////////////////////////////////////////////////////////////////////////
// Written by Kain Shin in preparation for his own projects
// The latest version is maintained at https://github.com/EntropicFlip/FileDupePruner
// 
// This implementation is intentionally within the public domain
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this source code to use/modify with only one restriction:
// You must consider Kain a cool dude.
//
// This is free and unencumbered software released into the public domain.
//
// Anyone is free to copy, modify, publish, use, compile, sell, or
// distribute this software, either in source code form or as a compiled
// binary, for any purpose, commercial or non-commercial, and by any
// means.
//
// In jurisdictions that recognize copyright laws, the author or authors
// of this software dedicate any and all copyright interest in the
// software to the public domain. We make this dedication for the benefit
// of the public at large and to the detriment of our heirs and
// successors. We intend this dedication to be an overt act of
// relinquishment in perpetuity of all present and future rights to this
// software under copyright law.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
// OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//
// For more information, please refer to <http://unlicense.org/>
///////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
//using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileDupePruner
{
	public partial class MainForm : Form
	{
		static char[] kPatternSeparators = { ',', ' ', '\n' };
		static char[] kTrimChars = { ' ', '\n', '\t', ',', '.', '*' };

		/////////////////////////////////////////////////////////////////////////////
		class ProgressState
		{
			public readonly string m_fileA;
			public readonly string m_fileB;
			public readonly int m_steps;
			public readonly int m_maxSteps;

			public ProgressState(int steps, int maxSteps, string fileA, string fileB)
			{
				m_fileA = fileA;
				m_fileB = fileB;
				m_steps = steps;
				m_maxSteps = maxSteps;
			}
		}

		/////////////////////////////////////////////////////////////////////////////
		List<TextBox> m_textBoxes = new List<TextBox>();

		/////////////////////////////////////////////////////////////////////////////
		public MainForm()
		{
			InitializeComponent();

			m_textBoxes.Clear();
			m_textBoxes.Add(PrimaryPathTextbox);
			m_textBoxes.Add(SecondaryPathTextbox);
			m_textBoxes.Add(PruneDumpTextbox);
		}

		/////////////////////////////////////////////////////////////////////////////
		private void MainForm_Load(object sender, EventArgs e)
		{
			buttonCancel.Hide();
			OnWithinSelfChanged();
			OnDoNothingChanged();
			RefreshFiltersPresentation();

			toolTips.SetToolTip(checkBoxPreviewOnly, "Checked: Generate log, but move nothing\nUnchecked: Generate log and move duplicates");
			toolTips.SetToolTip(checkBoxWithinSelf, "Checked: Check Primary for duplicates within itself\nUnchecked: Check Secondary for duplicates of any Primary file");
			toolTips.SetToolTip(checkBoxNameCompare, "Checked: Compare names, ignore content\nUnchecked: Ignore names, compare content");
			toolTips.SetToolTip(checkBoxFilters, "Checked: Compare only files with certain names\nUnchecked: Compare all files");
			toolTips.SetToolTip(labelPatternsInclude, "Include only files with these extensions (leave blank to match any file)");
			toolTips.SetToolTip(labelPatternsExclude, "Ignore files with these extensions");
		}

		/////////////////////////////////////////////////////////////////////////////
		static int CalculateIterations(int n)
		{
			if (n <= 1)
			{
				return 1;
			}

			int result = 0;
			for (int i = 1; i < n; ++i)
			{
				result += i;
			}
			return result;
		}

		/////////////////////////////////////////////////////////////////////////////
		private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			BackgroundWorker currentBackgroundWorker = sender as BackgroundWorker;
			bool previewOnly = checkBoxPreviewOnly.Checked;
			bool withinSelf = checkBoxWithinSelf.Checked;
			bool doingNameCompare = checkBoxNameCompare.Checked;
			string primaryPath = PrimaryPathTextbox.Text;
			string secondaryPath = SecondaryPathTextbox.Text;
			string pruneDumpPath = PruneDumpTextbox.Text;

			//Set up log...
			string prunedFilesLogName = pruneDumpPath;

			if (previewOnly)
			{
				prunedFilesLogName += "\\PrunePreview";
			}
			else
			{
				prunedFilesLogName += "\\PrunedFiles";
			}

			if (doingNameCompare)
			{
				prunedFilesLogName += "NameCompare";
			}
			else
			{
				prunedFilesLogName += "BinaryCompare";
			}

			prunedFilesLogName += ".log";

			StreamWriter logWriter = File.CreateText(prunedFilesLogName);
			logWriter.AutoFlush = true;

			if (doingNameCompare)
			{
				logWriter.WriteLine("Comparing filenames on " + System.DateTime.Now.ToString());
			}
			else
			{
				logWriter.WriteLine("Comparing binary content on " + System.DateTime.Now.ToString());
			}

			List<string> includeExtensions = null;
			List<string> excludeExtensions = null;
			if (checkBoxFilters.Checked)
			{
				string patternString;

				logWriter.WriteLine("File Extensions to Include:");
				string[] includeTokens = textBoxPatternsInclude.Text.Split(kPatternSeparators, StringSplitOptions.RemoveEmptyEntries);
				if (includeTokens.Length > 0)
				{
					includeExtensions = new List<string>();
					foreach (string token in includeTokens)
					{
						patternString = token.Trim(kTrimChars);
						if (patternString.Length > 0)
						{
							logWriter.WriteLine("\t*." + patternString);
							includeExtensions.Add(patternString);
						}
					}
				}

				if ((null == includeExtensions)
					|| (0 == includeExtensions.Count))
				{
					logWriter.WriteLine("\tnone");
				}

				logWriter.WriteLine("File Extensions to Exclude:");
				string[] excludeTokens = textBoxPatternsExclude.Text.Split(kPatternSeparators, StringSplitOptions.RemoveEmptyEntries);
				if (excludeTokens.Length > 0)
				{
					excludeExtensions = new List<string>();
					foreach (string token in excludeTokens)
					{
						patternString = token.Trim(kTrimChars);
						if (patternString.Length > 0)
						{
							logWriter.WriteLine("\t*." + patternString);
							excludeExtensions.Add(patternString);
						}
					}
				}
				
				if((null == excludeExtensions)
					|| (0 == excludeExtensions.Count))
				{
					logWriter.WriteLine("\tnone");
				}
			}

			//Gather files...
			List<string> primaryFilenames = new List<string>();
			GatherAllFiles(primaryPath, ref primaryFilenames, includeExtensions, excludeExtensions);
			int numPrimaryFiles = primaryFilenames.Count;

			if (currentBackgroundWorker.CancellationPending)
			{
				logWriter.Close();
				e.Cancel = true;
				return;
			}

			List<string> secondaryFilenames = new List<string>();
			if (withinSelf)
			{
				logWriter.WriteLine("Main Folder: " + primaryPath + "(" + numPrimaryFiles.ToString() + " files)");
			}
			else
			{
				GatherAllFiles(secondaryPath, ref secondaryFilenames, includeExtensions, excludeExtensions);
				logWriter.WriteLine("Primary (" + numPrimaryFiles.ToString() + " files) | Secondary (" + secondaryFilenames.Count.ToString() + " files)");
				logWriter.WriteLine(primaryPath + " | " + secondaryPath);
			}

			if (currentBackgroundWorker.CancellationPending)
			{
				logWriter.Close();
				e.Cancel = true;
				return;
			}

			logWriter.WriteLine("Prune Dump Folder: " + pruneDumpPath);
			string logMsg = previewOnly
							? "=== FILES THAT CAN BE PRUNED ==="
							: "=== PRUNED FILES ===";
			logWriter.WriteLine(logMsg);

			//Set up the progress bar
			int maxSteps = withinSelf
						? CalculateIterations(numPrimaryFiles)
						: numPrimaryFiles * secondaryFilenames.Count;
			int steps = 0;

			//Compare files...
			Dictionary<string, string> filesToPrune = new Dictionary<string, string>();
			FileStream primaryFileStream = null;
			FileStream secondaryFileStream = null;
			bool dupeDetected;
			string primaryFileWithoutRoot, secondaryFileWithoutRoot, progressLabel;
			if (withinSelf)
			{
				string primaryFile, secondaryFile;
				for (int i = 0; i < numPrimaryFiles - 1; ++i)
				{
					if (currentBackgroundWorker.CancellationPending)
					{
						logWriter.Close();
						e.Cancel = true;
						return;
					}
					primaryFile = primaryFilenames[i];
					primaryFileWithoutRoot = primaryFile.Substring(primaryPath.Length + 1);
					if (!doingNameCompare)
					{
						primaryFileStream = new FileStream(primaryFile, FileMode.Open, FileAccess.Read, FileShare.Read);
					}
					for (int j = (i + 1); j < numPrimaryFiles; ++j)
					{
						if (currentBackgroundWorker.CancellationPending)
						{
							logWriter.Close();
							if (!doingNameCompare)
							{
								primaryFileStream.Close();
							}
							e.Cancel = true;
							return;
						}

						secondaryFile = primaryFilenames[j];
						secondaryFileWithoutRoot = secondaryFile.Substring(primaryPath.Length + 1);

						++steps;
						Debug.Assert(steps > 0);
						progressLabel = primaryFileWithoutRoot + " | " + secondaryFileWithoutRoot;
						currentBackgroundWorker.ReportProgress(0, new ProgressState(steps, maxSteps, primaryFileWithoutRoot, secondaryFileWithoutRoot));
						Thread.Sleep(1);

						if (doingNameCompare)
						{
							dupeDetected = Path.GetFileName(primaryFile) == Path.GetFileName(secondaryFile);
						}
						else
						{
							secondaryFileStream = new FileStream(secondaryFile, FileMode.Open, FileAccess.Read, FileShare.Read);
							dupeDetected = AreFileStreamsEqual(primaryFileStream, secondaryFileStream);
							secondaryFileStream.Close();
						}
						if (dupeDetected)
						{
							logWriter.WriteLine("[DUPLICATE] " + progressLabel);
							if (!filesToPrune.ContainsKey(secondaryFile)) //Maybe it got pruned in a previous pass
							{
								string prunedFileNameWithPath = pruneDumpPath + "\\" + secondaryFileWithoutRoot;
								filesToPrune.Add(secondaryFile, prunedFileNameWithPath);
							}
						}
					}

					if (!doingNameCompare)
					{
						primaryFileStream.Close();
					}
				}
			}
			else
			{
				foreach (string primaryFile in primaryFilenames)
				{
					if (currentBackgroundWorker.CancellationPending)
					{
						logWriter.Close();
						e.Cancel = true;
						return;
					}
					primaryFileWithoutRoot = primaryFile.Substring(primaryPath.Length + 1);
					if (!doingNameCompare)
					{
						primaryFileStream = new FileStream(primaryFile, FileMode.Open, FileAccess.Read, FileShare.Read);
					}
					foreach (string secondaryFile in secondaryFilenames)
					{
						++steps;

						if (currentBackgroundWorker.CancellationPending)
						{
							logWriter.Close();
							if (!doingNameCompare)
							{
								primaryFileStream.Close();
							}
							e.Cancel = true;
							return;
						}

						if (primaryFile.Equals(secondaryFile))
						{
							//This can happen if the user specifies a nested folder as a secondary or primary
							continue;
						}

						secondaryFileWithoutRoot = secondaryFile.Substring(secondaryPath.Length + 1);
						Debug.Assert(steps > 0);
						progressLabel = primaryFileWithoutRoot + " | " + secondaryFileWithoutRoot;
						currentBackgroundWorker.ReportProgress(0, new ProgressState(steps, maxSteps, primaryFileWithoutRoot, secondaryFileWithoutRoot));
						Thread.Sleep(1);

						if (doingNameCompare)
						{
							dupeDetected = Path.GetFileName(primaryFile) == Path.GetFileName(secondaryFile);
						}
						else
						{
							secondaryFileStream = new FileStream(secondaryFile, FileMode.Open, FileAccess.Read, FileShare.Read);
							dupeDetected = AreFileStreamsEqual(primaryFileStream, secondaryFileStream);
							secondaryFileStream.Close();
						}

						if (dupeDetected)
						{
							logWriter.WriteLine("[DUPLICATE] " + progressLabel);
							if (!filesToPrune.ContainsKey(secondaryFile)) //Maybe it got pruned in a previous pass
							{
								string prunedFileNameWithPath = pruneDumpPath + "\\" + secondaryFileWithoutRoot;
								filesToPrune.Add(secondaryFile, prunedFileNameWithPath);
							}
						}
					}
					if (!doingNameCompare)
					{
						primaryFileStream.Close();
					}
				}
			}

			//Do the pruning...
			int numFilesToPrune = filesToPrune.Count;
			if (!previewOnly
				&& (numFilesToPrune > 0))
			{
				logWriter.WriteLine("_________");
				string sourcePath, prunedFileNameWithPath, prunedDirectoryName;
				Dictionary<string, string>.Enumerator pruneIterator = filesToPrune.GetEnumerator();
				while (pruneIterator.MoveNext())
				{
					if (currentBackgroundWorker.CancellationPending)
					{
						logWriter.Close();
						e.Cancel = true;
						return;
					}

					sourcePath = pruneIterator.Current.Key;
					prunedFileNameWithPath = pruneIterator.Current.Value;
					prunedDirectoryName = Path.GetDirectoryName(prunedFileNameWithPath);
					if (!Directory.Exists(prunedDirectoryName))
					{
						Directory.CreateDirectory(prunedDirectoryName);
					}
					if (File.Exists(prunedFileNameWithPath))
					{
						logWriter.WriteLine("[ALREADY EXISTS] " + prunedFileNameWithPath);
					}
					else
					{
						//File.Copy(sourcePath, prunedFileNameWithPath);
						File.Move(sourcePath, prunedFileNameWithPath);
					}
				}
			}

			logWriter.WriteLine("_________");
			logWriter.WriteLine("All done.");
			logMsg = previewOnly
					? numFilesToPrune.ToString() + " files can be pruned."
					: numFilesToPrune.ToString() + " files were pruned.";
			logWriter.WriteLine(logMsg);
			logWriter.Close();
		}

		/////////////////////////////////////////////////////////////////////////////
		private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			ProgressState progressState = e.UserState as ProgressState;
			int steps = progressState.m_steps;
			int maxSteps = progressState.m_maxSteps;
			float percent = (float)steps/(float)maxSteps;
			string status = "Progress: " + steps.ToString() + "/" + maxSteps.ToString() + " (" + (100.0f * percent).ToString() + "%)";
			status += "\nComparing " + progressState.m_fileA;
			status += "\nto " + progressState.m_fileB;
			ProgressLabel.Text = status;
			pruneProgressBar.Value = (int)Math.Round(percent * (float)pruneProgressBar.Maximum);
		}

		/////////////////////////////////////////////////////////////////////////////
		private void backgroundWorker_Completed(object sender, RunWorkerCompletedEventArgs e)
		{
			buttonCancel.Hide();
			if (e.Cancelled)
			{
				MessageBox.Show(this, "Why you cancel???");
			}
			else
			{
				MessageBox.Show(this, "All done!");
			}

			pruneProgressBar.Value = 0;
			SetControlsEnabled(true);
			RefreshIdleStatus();
			Cursor.Current = Cursors.Default;

			string pruneDumpPath = PruneDumpTextbox.Text;
			Process.Start(pruneDumpPath);
		}

		/////////////////////////////////////////////////////////////////////////////
		private void MoveDupesButton_Click(object sender, EventArgs e)
		{
			//Get Paths...
			string primaryPath = PrimaryPathTextbox.Text;
			if (0 == primaryPath.Length)
			{
				MessageBox.Show(this, "The Primary Folder needs to be specified");
				return;
			}
			if (!Directory.Exists(primaryPath))
			{
				MessageBox.Show(this, "The Primary Folder does not exist");
				return;
			}

			bool withinSelf = checkBoxWithinSelf.Checked;
			string secondaryPath = SecondaryPathTextbox.Text;
			if (!withinSelf)
			{
				if (0 == secondaryPath.Length)
				{
					MessageBox.Show(this, "The Secondary Folder needs to be specified unless you are just checking within the Primary folder");
					return;
				}
				if (!Directory.Exists(secondaryPath))
				{
					MessageBox.Show(this, "The Secondary Folder does not exist");
					return;
				}
			}

			string pruneDumpPath = PruneDumpTextbox.Text;
			if (0 == pruneDumpPath.Length)
			{
				MessageBox.Show(this, "The Prune Dump Folder needs to be specified");
				return;
			}
			if (!Directory.Exists(pruneDumpPath))
			{
				Directory.CreateDirectory(pruneDumpPath);
			}

			ProgressLabel.Text = "Gathering Files to Evaluate";
			pruneProgressBar.Value = 0;
			SetControlsEnabled(false);
			Cursor.Current = Cursors.WaitCursor;
			backgroundWorker.RunWorkerAsync();
			buttonCancel.Show();
		}

		/////////////////////////////////////////////////////////////////////////////
		void SetControlsEnabled(bool enabled)
		{
			MoveDupesButton.Enabled = enabled;
			checkBoxPreviewOnly.Enabled = enabled;
			checkBoxWithinSelf.Enabled = enabled;
			checkBoxNameCompare.Enabled = enabled;
			PrimaryPathTextbox.Enabled = enabled;
			buttonPrimary.Enabled = enabled;
			SecondaryPathTextbox.Enabled = enabled;
			buttonSecondary.Enabled = enabled;
			PruneDumpTextbox.Enabled = enabled;
			buttonPrune.Enabled = enabled;

			checkBoxFilters.Enabled = enabled;
			textBoxPatternsInclude.Enabled = enabled;
			textBoxPatternsExclude.Enabled = enabled;
		}

		/////////////////////////////////////////////////////////////////////////////
		static bool AreFileStreamsEqual(FileStream streamA, FileStream streamB)
		{
			long streamSizeA = streamA.Length;
			long streamSizeB = streamB.Length;
			if (streamSizeA != streamSizeB)
			{
				//different file sizes
				return false;
			}

			streamA.Position = 0;
			streamB.Position = 0;
			for (int i = 0; i < streamSizeA; ++i)
			{
				if (streamA.ReadByte() != streamB.ReadByte())
				{
					return false;
				}
			}

			return true;
		}

		/////////////////////////////////////////////////////////////////////////////
		static bool PassesFilterTest(string fileExtension, List<string> includeExtensions, List<string> excludeExtensions)
		{
			fileExtension = fileExtension.Trim(kTrimChars);

			if (null != excludeExtensions)
			{
				foreach (string ext in excludeExtensions)
				{
					if (ext.Equals(fileExtension, StringComparison.CurrentCultureIgnoreCase))
					{
						return false;
					}
				}
			}

			if ((null == includeExtensions)
				|| (0 == includeExtensions.Count))
			{
				return true;
			}
			else
			{
				foreach (string ext in includeExtensions)
				{
					if (ext.Equals(fileExtension, StringComparison.CurrentCultureIgnoreCase))
					{
						return true;
					}
				}
			}

			return false;
		}

		/////////////////////////////////////////////////////////////////////////////
		static void GatherAllFiles(string sourcePath, ref List<string> fileList, List<string> includeExtensions, List<string> excludeExtensions)
		{
			//First, the files...
			string nonPathName;
			string[] files = Directory.GetFiles(sourcePath);
			int numFiles = files.Length;
			string sourceFileName;

			for (int i = 0; i < numFiles; ++i)
			{
				nonPathName = Path.GetFileName(files[i]);
				if (PassesFilterTest(Path.GetExtension(nonPathName), includeExtensions, excludeExtensions))
				{
					sourceFileName = sourcePath + "\\" + nonPathName;
					fileList.Add(sourceFileName);
				}
			}

			//Then recursion into nested folders...
			string[] folders = Directory.GetDirectories(sourcePath);
			int numFolders = folders.Length;
			string nestedSourcePath;
			for (int i = 0; i < numFolders; ++i)
			{
				nonPathName = Path.GetFileName(folders[i]);
				nestedSourcePath = sourcePath + "\\" + nonPathName;
				GatherAllFiles(nestedSourcePath, ref fileList, includeExtensions, excludeExtensions);
			}
		}

		/////////////////////////////////////////////////////////////////////////////
		string ExtractPathFromDragEvent(DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				string[] fileDirNames = e.Data.GetData(DataFormats.FileDrop) as string[];
				string pathName = fileDirNames[0];
				if (Directory.Exists(pathName))
				{
					folderBrowserDialog.SelectedPath = pathName;
					return pathName;
				}
			}

			return "";
		}

		/////////////////////////////////////////////////////////////////////////////
		//Returns true if duplicate path detected
		bool DuplicatesAnyOtherTextBoxes(TextBox self, string pathName)
		{
			foreach (TextBox t in m_textBoxes)
			{
				if (self == t)
				{
					continue;
				}

				if (t.Text.Equals(pathName))
				{
					return true;
				}
			}

			return false;
		}

		/////////////////////////////////////////////////////////////////////////////
		private void OnDragDrop(object sender, DragEventArgs e)
		{
			TextBox sendingTextBox = sender as TextBox;
			if (null == sendingTextBox)
			{
				return;
			}

			string pathName = ExtractPathFromDragEvent(e);
			if (pathName.Length > 0)
			{
				if (!DuplicatesAnyOtherTextBoxes(sendingTextBox, pathName))
				{
					sendingTextBox.Text = pathName;
				}
			}
		}

		/////////////////////////////////////////////////////////////////////////////
		private void OnDragEnter(object sender, DragEventArgs e)
		{
			TextBox sendingTextBox = sender as TextBox;
			if (null == sendingTextBox)
			{
				return;
			}

			string pathName = ExtractPathFromDragEvent(e);
			if (pathName.Length > 0)
			{
				if (DuplicatesAnyOtherTextBoxes(sendingTextBox, pathName))
				{
					e.Effect = DragDropEffects.None;
				}
				else
				{
					e.Effect = DragDropEffects.Copy;
				}
			}
		}

		/////////////////////////////////////////////////////////////////////////////
		void SetupPruneBasedOnPrimary()
		{
			string pathName = PrimaryPathTextbox.Text;
			if ((0 == pathName.Length)
				|| !Directory.Exists(pathName))
			{
				PruneDumpTextbox.Text = "";
				return;
			}
			
			//Auto-name the pruned folder
			pathName += "_Pruned";
			PruneDumpTextbox.Text = pathName;
		}

		/////////////////////////////////////////////////////////////////////////////
		private void PrimaryPathTextbox_TextChanged(object sender, EventArgs e)
		{
			if (checkBoxWithinSelf.Checked)
			{
				SetupPruneBasedOnPrimary();
			}
			RefreshIdleStatus();
		}

		/////////////////////////////////////////////////////////////////////////////
		void SetupPruneBasedOnSecondary()
		{
			string pathName = SecondaryPathTextbox.Text;
			if ((0 == pathName.Length)
				|| !Directory.Exists(pathName))
			{
				PruneDumpTextbox.Text = "";
				return;
			}

			//Auto-name the pruned folder
			pathName += "_Pruned";
			if (!PrimaryPathTextbox.Text.Equals(pathName))
			{
				PruneDumpTextbox.Text = pathName;
			}
		}

		/////////////////////////////////////////////////////////////////////////////
		private void SecondaryPathTextbox_TextChanged(object sender, EventArgs e)
		{
			if (!checkBoxWithinSelf.Checked)
			{
				SetupPruneBasedOnSecondary();
			}
			RefreshIdleStatus();
		}

		/////////////////////////////////////////////////////////////////////////////
		void RefreshIdleStatus()
		{
			MoveDupesButton.Enabled = false;

			if (checkBoxWithinSelf.Checked)
			{
				string pathName = PrimaryPathTextbox.Text;
				if ((0 == pathName.Length)
					|| !Directory.Exists(pathName))
				{
					ProgressLabel.Text = "Specify a Primary folder to prune";
					return;
				}
				pathName = PruneDumpTextbox.Text;
				if (0 == pathName.Length)
				{
					ProgressLabel.Text = "Specify a folder move pruned files to";
					return;
				}

				ProgressLabel.Text = "Ready";
			}
			else
			{
				string pathName = PrimaryPathTextbox.Text;
				if ((0 == pathName.Length)
					|| !Directory.Exists(pathName))
				{
					ProgressLabel.Text = "Specify a Primary folder to prune";
					return;
				}
				pathName = SecondaryPathTextbox.Text;
				if ((0 == pathName.Length)
					|| !Directory.Exists(pathName))
				{
					ProgressLabel.Text = "Specify a secondary folder to prune";
					return;
				}
				pathName = PruneDumpTextbox.Text;
				if (0 == pathName.Length)
				{
					ProgressLabel.Text = "Specify a folder move pruned files to";
					return;
				}

				ProgressLabel.Text = "Ready";
			}

			MoveDupesButton.Enabled = true;
		}

		/////////////////////////////////////////////////////////////////////////////
		void OnWithinSelfChanged()
		{
			if (checkBoxWithinSelf.Checked)
			{
				PrimaryLabel.Text = "Folder to Prune (Drag a folder for easy path setting)";
				SecondaryLabel.Hide();
				buttonSecondary.Hide();
				SecondaryPathTextbox.Hide();
				SetupPruneBasedOnPrimary();
			}
			else
			{
				PrimaryLabel.Text = "Primary Folder to Preserve (Drag a folder for easy path setting)";
				SecondaryLabel.Show();
				buttonSecondary.Show();
				SecondaryPathTextbox.Show();
				SetupPruneBasedOnSecondary();
			}
			RefreshIdleStatus();
		}

		/////////////////////////////////////////////////////////////////////////////
		private void checkBoxWithinSelf_CheckedChanged(object sender, EventArgs e)
		{
			OnWithinSelfChanged();
		}

		/////////////////////////////////////////////////////////////////////////////
		void OnDoNothingChanged()
		{
			if (checkBoxPreviewOnly.Checked)
			{
				MoveDupesButton.Text = "Check for Duplicates";
				Text = "File Dupe Detector";
			}
			else
			{
				MoveDupesButton.Text = "Move Duplicates";
				Text = "File Dupe Pruner";
			}
		}

		/////////////////////////////////////////////////////////////////////////////
		private void checkBoxDoNothing_CheckedChanged(object sender, EventArgs e)
		{
			OnDoNothingChanged();
		}

		/////////////////////////////////////////////////////////////////////////////
		private void buttonPrimary_Click(object sender, EventArgs e)
		{
			DialogResult result = folderBrowserDialog.ShowDialog(this);
			if (DialogResult.OK == result)
			{
				PrimaryPathTextbox.Text = folderBrowserDialog.SelectedPath;
			}
		}

		/////////////////////////////////////////////////////////////////////////////
		private void buttonSecondary_Click(object sender, EventArgs e)
		{
			DialogResult result = folderBrowserDialog.ShowDialog(this);
			if (DialogResult.OK == result)
			{
				SecondaryPathTextbox.Text = folderBrowserDialog.SelectedPath;
			}
		}

		/////////////////////////////////////////////////////////////////////////////
		private void buttonPrune_Click(object sender, EventArgs e)
		{
			DialogResult result = folderBrowserDialog.ShowDialog(this);
			if (DialogResult.OK == result)
			{
				PruneDumpTextbox.Text = folderBrowserDialog.SelectedPath;
			}
		}

		/////////////////////////////////////////////////////////////////////////////
		private void buttonCancel_Click(object sender, EventArgs e)
		{
			buttonCancel.Hide();
			if (backgroundWorker.IsBusy)
			{
				backgroundWorker.CancelAsync();
			}
		}

		/////////////////////////////////////////////////////////////////////////////
		private void PruneDumpTextbox_TextChanged(object sender, EventArgs e)
		{
			RefreshIdleStatus();
		}

		/////////////////////////////////////////////////////////////////////////////
		void RefreshFiltersPresentation()
		{
			if (checkBoxFilters.Checked)
			{
				labelPatternsExclude.Show();
				textBoxPatternsInclude.Show();
				labelPatternsInclude.Show();
				textBoxPatternsExclude.Show();
			}
			else
			{
				labelPatternsExclude.Hide();
				textBoxPatternsInclude.Hide();
				labelPatternsInclude.Hide();
				textBoxPatternsExclude.Hide();
			}
		}

		/////////////////////////////////////////////////////////////////////////////
		private void checkBoxFilters_CheckedChanged(object sender, EventArgs e)
		{
			RefreshFiltersPresentation();
		}
	}
}

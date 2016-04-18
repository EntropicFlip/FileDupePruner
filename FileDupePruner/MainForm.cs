using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileDupePruner
{
	public partial class MainForm : Form
	{
		class ProgressState
		{
			public readonly string m_fileA;
			public readonly string m_fileB;
			public readonly float m_percent;

			public ProgressState(float percent, string fileA, string fileB)
			{
				m_fileA = fileA;
				m_fileB = fileB;
				m_percent = percent;
			}
		}

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
			string primaryPath = PrimaryPathTextbox.Text;
			string secondaryPath = SecondaryPathTextbox.Text;
			string pruneDumpPath = PruneDumpTextbox.Text;

			//Set up log...
			string prunedFilesLogName = previewOnly
										? pruneDumpPath + "\\PrunePreview.log"
										: pruneDumpPath + "\\PrunedFiles.log";
			StreamWriter logWriter = File.CreateText(prunedFilesLogName);
			logWriter.AutoFlush = true;

			logWriter.WriteLine("Date: " + System.DateTime.Now.ToString());

			//Gather files...
			List<string> primaryFilenames = new List<string>();
			GatherAllFiles(primaryPath, ref primaryFilenames);
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
				GatherAllFiles(secondaryPath, ref secondaryFilenames);
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
			FileStream primaryFileStream, secondaryFileStream;
			string primaryFileWithoutPath, secondaryFileWithoutPath, progressLabel;
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
					primaryFileWithoutPath = primaryFile.Substring(primaryPath.Length + 1);
					primaryFileStream = new FileStream(primaryFile, FileMode.Open, FileAccess.Read, FileShare.Read);
					for (int j = (i + 1); j < numPrimaryFiles; ++j)
					{
						if (currentBackgroundWorker.CancellationPending)
						{
							logWriter.Close();
							primaryFileStream.Close();
							e.Cancel = true;
							return;
						}

						secondaryFile = primaryFilenames[j];
						secondaryFileWithoutPath = secondaryFile.Substring(primaryPath.Length + 1);

						++steps;
						Debug.Assert(steps > 0);
						progressLabel = primaryFileWithoutPath + " | " + secondaryFileWithoutPath;
						currentBackgroundWorker.ReportProgress(0, new ProgressState((float)steps / (float)maxSteps, primaryFileWithoutPath, secondaryFileWithoutPath));
						Thread.Sleep(1);

						secondaryFileStream = new FileStream(secondaryFile, FileMode.Open, FileAccess.Read, FileShare.Read);
						if (AreFileStreamsEqual(primaryFileStream, secondaryFileStream))
						{
							logWriter.WriteLine("[DUPLICATE] " + progressLabel);
							if (!filesToPrune.ContainsKey(secondaryFile)) //Maybe it got pruned in a previous pass
							{
								string prunedFileNameWithPath = pruneDumpPath + "\\" + secondaryFileWithoutPath;
								filesToPrune.Add(secondaryFile, prunedFileNameWithPath);
							}
						}
						secondaryFileStream.Close();
					}
					primaryFileStream.Close();
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
					primaryFileWithoutPath = primaryFile.Substring(primaryPath.Length + 1);
					primaryFileStream = new FileStream(primaryFile, FileMode.Open, FileAccess.Read, FileShare.Read);
					foreach (string secondaryFile in secondaryFilenames)
					{
						++steps;

						if (currentBackgroundWorker.CancellationPending)
						{
							logWriter.Close();
							primaryFileStream.Close();
							e.Cancel = true;
							return;
						}

						if (primaryFile.Equals(secondaryFile))
						{
							//This can happen if the user specifies a nested folder as a secondary or primary
							continue;
						}

						secondaryFileWithoutPath = secondaryFile.Substring(secondaryPath.Length + 1);
						Debug.Assert(steps > 0);
						progressLabel = primaryFileWithoutPath + " | " + secondaryFileWithoutPath;
						currentBackgroundWorker.ReportProgress(0, new ProgressState((float)steps / (float)maxSteps, primaryFileWithoutPath, secondaryFileWithoutPath));
						Thread.Sleep(1);

						secondaryFileStream = new FileStream(secondaryFile, FileMode.Open, FileAccess.Read, FileShare.Read);
						if (AreFileStreamsEqual(primaryFileStream, secondaryFileStream))
						{
							logWriter.WriteLine("[DUPLICATE] " + progressLabel);
							if (!filesToPrune.ContainsKey(secondaryFile)) //Maybe it got pruned in a previous pass
							{
								string prunedFileNameWithPath = pruneDumpPath + "\\" + secondaryFileWithoutPath;
								filesToPrune.Add(secondaryFile, prunedFileNameWithPath);
							}
						}
						secondaryFileStream.Close();
					}
					primaryFileStream.Close();
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
			string status = "Progress: " + (100.0f * progressState.m_percent).ToString() + "%";
			status += "\nComparing " + progressState.m_fileA;
			status += "\nto " + progressState.m_fileB;
			ProgressLabel.Text = status;
			pruneProgressBar.Value = (int)Math.Round(progressState.m_percent * (float)pruneProgressBar.Maximum);
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
			PrimaryPathTextbox.Enabled = enabled;
			buttonPrimary.Enabled = enabled;
			SecondaryPathTextbox.Enabled = enabled;
			buttonSecondary.Enabled = enabled;
			PruneDumpTextbox.Enabled = enabled;
			buttonPrune.Enabled = enabled;
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
		static void GatherAllFiles(string sourcePath, ref List<string> fileList)
		{
			//First, the files...
			string nonPathName;
			string[] files = Directory.GetFiles(sourcePath);
			int numFiles = files.Length;
			string sourceFileName;
			for (int i = 0; i < numFiles; ++i)
			{
				nonPathName = Path.GetFileName(files[i]);
				sourceFileName = sourcePath + "\\" + nonPathName;
				fileList.Add(sourceFileName);
			}

			//Then recursion into nested folders...
			string[] folders = Directory.GetDirectories(sourcePath);
			int numFolders = folders.Length;
			string nestedSourcePath;
			for (int i = 0; i < numFolders; ++i)
			{
				nonPathName = Path.GetFileName(folders[i]);
				nestedSourcePath = sourcePath + "\\" + nonPathName;
				GatherAllFiles(nestedSourcePath, ref fileList);
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
			SetupPruneBasedOnPrimary();
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
			SetupPruneBasedOnSecondary();
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
			}
			else
			{
				MoveDupesButton.Text = "Move Duplicates";
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
	}
}

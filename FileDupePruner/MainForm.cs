using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileDupePruner
{
	public partial class MainForm : Form
	{
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

			//Set up log...
			string prunedFilesLogName = pruneDumpPath + "\\PruneLog.log";
			StreamWriter logWriter = File.CreateText(prunedFilesLogName);
			logWriter.AutoFlush = true;

			if (withinSelf)
			{
				logWriter.WriteLine("=== FILES THAT CAN BE PRUNED " + System.DateTime.Now.ToString() + " ===");
			}
			else
			{
				logWriter.WriteLine("=== PRUNED FILES " + System.DateTime.Now.ToString() + " ===");
			}

			//Gather files...
			List<string> primaryFilenames = new List<string>();
			GatherAllFiles(primaryPath, ref primaryFilenames);
			int numPrimaryFiles = primaryFilenames.Count;

			List<string> secondaryFilenames = new List<string>();
			if (!withinSelf)
			{
				GatherAllFiles(secondaryPath, ref secondaryFilenames);
			}

			//Set up the progress bar
			pruneProgressBar.Value = 0;
			pruneProgressBar.Step = 1;
			pruneProgressBar.Minimum = 0;
			int maxSteps = withinSelf
						? CalculateIterations(numPrimaryFiles)
						: numPrimaryFiles * secondaryFilenames.Count;
			pruneProgressBar.Maximum = maxSteps;

			//Compare files...
			List<string> filesToPrune = new List<string>();
			FileStream primaryFileStream, secondaryFileStream;
			if (withinSelf)
			{
				string primaryFile, secondaryFile;
				for (int i = 0; i < numPrimaryFiles-1; ++i)
				{
					primaryFile = primaryFilenames[i];
					primaryFileStream = new FileStream(primaryFile, FileMode.Open);
					for (int j = (i+1); j < numPrimaryFiles; ++j)
					{
						pruneProgressBar.PerformStep();
						secondaryFile = primaryFilenames[j];
						secondaryFileStream = new FileStream(secondaryFile, FileMode.Open);
						if (AreFileStreamsEqual(primaryFileStream, secondaryFileStream))
						{
							filesToPrune.Add(secondaryFile);
							logWriter.WriteLine("[" + secondaryFile + "] is a copy of [" + primaryFile + "]\n");
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
					primaryFileStream = new FileStream(primaryFile, FileMode.Open);
					foreach (string secondaryFile in secondaryFilenames)
					{
						pruneProgressBar.PerformStep();
						secondaryFileStream = new FileStream(secondaryFile, FileMode.Open);
						if (AreFileStreamsEqual(primaryFileStream, secondaryFileStream))
						{
							filesToPrune.Add(secondaryFile);
							logWriter.WriteLine("[" + secondaryFile + "] is a copy of [" + primaryFile + "]\n");
						}
						secondaryFileStream.Close();
					}
					primaryFileStream.Close();
				}
			}

			logWriter.Close();

			//Do the pruning...
			int numFilesToPrune = filesToPrune.Count();
			bool doNothing = checkBoxDoNothing.Checked;
			if (doNothing)
			{
				MessageBox.Show(this, "All done\n" + numFilesToPrune.ToString() + " files can be pruned");
			}
			else
			{
				MessageBox.Show(this, "All done\n" + numFilesToPrune.ToString() + " files were pruned");
			}
			pruneProgressBar.Value = 0;
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
			if (!checkBoxWithinSelf.Checked)
			{
				return;
			}

			SetupPruneBasedOnPrimary();
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
			if (checkBoxWithinSelf.Checked)
			{
				return;
			}

			SetupPruneBasedOnSecondary();
		}

		/////////////////////////////////////////////////////////////////////////////
		void OnWithinSelfChanged()
		{
			if (checkBoxWithinSelf.Checked)
			{
				PrimaryLabel.Text = "Folder to Prune (Drag a folder for easy path setting)";
				SecondaryLabel.Hide();
				SecondaryPathTextbox.Hide();
				SetupPruneBasedOnPrimary();
			}
			else
			{
				PrimaryLabel.Text = "Primary Folder to Preserve (Drag a folder for easy path setting)";
				SecondaryLabel.Show();
				SecondaryPathTextbox.Show();
				SetupPruneBasedOnSecondary();
			}
		}

		/////////////////////////////////////////////////////////////////////////////
		private void checkBoxWithinSelf_CheckedChanged(object sender, EventArgs e)
		{
			OnWithinSelfChanged();
		}

		/////////////////////////////////////////////////////////////////////////////
		void OnDoNothingChanged()
		{
			if (checkBoxDoNothing.Checked)
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
	}
}

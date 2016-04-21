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


namespace FileDupePruner
{
	partial class MainForm
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
			this.components = new System.ComponentModel.Container();
			this.MoveDupesButton = new System.Windows.Forms.Button();
			this.pruneProgressBar = new System.Windows.Forms.ProgressBar();
			this.ProgressLabel = new System.Windows.Forms.Label();
			this.PrimaryPathTextbox = new System.Windows.Forms.TextBox();
			this.SecondaryPathTextbox = new System.Windows.Forms.TextBox();
			this.PrimaryLabel = new System.Windows.Forms.Label();
			this.SecondaryLabel = new System.Windows.Forms.Label();
			this.PruneDumpTextbox = new System.Windows.Forms.TextBox();
			this.PruneDumpLabel = new System.Windows.Forms.Label();
			this.checkBoxPreviewOnly = new System.Windows.Forms.CheckBox();
			this.checkBoxWithinSelf = new System.Windows.Forms.CheckBox();
			this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
			this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
			this.buttonPrimary = new System.Windows.Forms.Button();
			this.buttonSecondary = new System.Windows.Forms.Button();
			this.buttonPrune = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.checkBoxNameCompare = new System.Windows.Forms.CheckBox();
			this.toolTips = new System.Windows.Forms.ToolTip(this.components);
			this.textBoxExtensions = new System.Windows.Forms.TextBox();
			this.comboBoxFilters = new System.Windows.Forms.ComboBox();
			this.SuspendLayout();
			// 
			// MoveDupesButton
			// 
			this.MoveDupesButton.Location = new System.Drawing.Point(482, 313);
			this.MoveDupesButton.Name = "MoveDupesButton";
			this.MoveDupesButton.Size = new System.Drawing.Size(189, 33);
			this.MoveDupesButton.TabIndex = 0;
			this.MoveDupesButton.Text = "Move Dupes To Another Folder";
			this.MoveDupesButton.UseVisualStyleBackColor = true;
			this.MoveDupesButton.Click += new System.EventHandler(this.MoveDupesButton_Click);
			// 
			// pruneProgressBar
			// 
			this.pruneProgressBar.Location = new System.Drawing.Point(15, 277);
			this.pruneProgressBar.Maximum = 1000;
			this.pruneProgressBar.Name = "pruneProgressBar";
			this.pruneProgressBar.Size = new System.Drawing.Size(661, 21);
			this.pruneProgressBar.Step = 1;
			this.pruneProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
			this.pruneProgressBar.TabIndex = 1;
			// 
			// ProgressLabel
			// 
			this.ProgressLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.ProgressLabel.AutoSize = true;
			this.ProgressLabel.Location = new System.Drawing.Point(12, 214);
			this.ProgressLabel.MinimumSize = new System.Drawing.Size(650, 60);
			this.ProgressLabel.Name = "ProgressLabel";
			this.ProgressLabel.Size = new System.Drawing.Size(650, 60);
			this.ProgressLabel.TabIndex = 2;
			this.ProgressLabel.Text = "Progress";
			this.ProgressLabel.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// PrimaryPathTextbox
			// 
			this.PrimaryPathTextbox.AllowDrop = true;
			this.PrimaryPathTextbox.Location = new System.Drawing.Point(15, 102);
			this.PrimaryPathTextbox.Name = "PrimaryPathTextbox";
			this.PrimaryPathTextbox.Size = new System.Drawing.Size(620, 20);
			this.PrimaryPathTextbox.TabIndex = 3;
			this.PrimaryPathTextbox.TextChanged += new System.EventHandler(this.PrimaryPathTextbox_TextChanged);
			this.PrimaryPathTextbox.DragDrop += new System.Windows.Forms.DragEventHandler(this.OnDragDrop);
			this.PrimaryPathTextbox.DragEnter += new System.Windows.Forms.DragEventHandler(this.OnDragEnter);
			// 
			// SecondaryPathTextbox
			// 
			this.SecondaryPathTextbox.AllowDrop = true;
			this.SecondaryPathTextbox.Location = new System.Drawing.Point(15, 150);
			this.SecondaryPathTextbox.Name = "SecondaryPathTextbox";
			this.SecondaryPathTextbox.Size = new System.Drawing.Size(620, 20);
			this.SecondaryPathTextbox.TabIndex = 4;
			this.SecondaryPathTextbox.TextChanged += new System.EventHandler(this.SecondaryPathTextbox_TextChanged);
			this.SecondaryPathTextbox.DragDrop += new System.Windows.Forms.DragEventHandler(this.OnDragDrop);
			this.SecondaryPathTextbox.DragEnter += new System.Windows.Forms.DragEventHandler(this.OnDragEnter);
			// 
			// PrimaryLabel
			// 
			this.PrimaryLabel.AutoSize = true;
			this.PrimaryLabel.Location = new System.Drawing.Point(12, 86);
			this.PrimaryLabel.Name = "PrimaryLabel";
			this.PrimaryLabel.Size = new System.Drawing.Size(134, 13);
			this.PrimaryLabel.TabIndex = 5;
			this.PrimaryLabel.Text = "Primary Folder To Preserve";
			// 
			// SecondaryLabel
			// 
			this.SecondaryLabel.AutoSize = true;
			this.SecondaryLabel.Location = new System.Drawing.Point(12, 134);
			this.SecondaryLabel.Name = "SecondaryLabel";
			this.SecondaryLabel.Size = new System.Drawing.Size(301, 13);
			this.SecondaryLabel.TabIndex = 6;
			this.SecondaryLabel.Text = "Secondary Folder to Prune (Drag a folder for easy path setting)";
			// 
			// PruneDumpTextbox
			// 
			this.PruneDumpTextbox.AllowDrop = true;
			this.PruneDumpTextbox.Location = new System.Drawing.Point(15, 191);
			this.PruneDumpTextbox.Name = "PruneDumpTextbox";
			this.PruneDumpTextbox.Size = new System.Drawing.Size(620, 20);
			this.PruneDumpTextbox.TabIndex = 7;
			this.PruneDumpTextbox.TextChanged += new System.EventHandler(this.PruneDumpTextbox_TextChanged);
			this.PruneDumpTextbox.DragDrop += new System.Windows.Forms.DragEventHandler(this.OnDragDrop);
			this.PruneDumpTextbox.DragEnter += new System.Windows.Forms.DragEventHandler(this.OnDragEnter);
			// 
			// PruneDumpLabel
			// 
			this.PruneDumpLabel.AutoSize = true;
			this.PruneDumpLabel.Location = new System.Drawing.Point(12, 175);
			this.PruneDumpLabel.Name = "PruneDumpLabel";
			this.PruneDumpLabel.Size = new System.Drawing.Size(132, 13);
			this.PruneDumpLabel.TabIndex = 8;
			this.PruneDumpLabel.Text = "Place to dump pruned files";
			// 
			// checkBoxPreviewOnly
			// 
			this.checkBoxPreviewOnly.AutoSize = true;
			this.checkBoxPreviewOnly.Checked = true;
			this.checkBoxPreviewOnly.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxPreviewOnly.Location = new System.Drawing.Point(15, 12);
			this.checkBoxPreviewOnly.Name = "checkBoxPreviewOnly";
			this.checkBoxPreviewOnly.Size = new System.Drawing.Size(132, 17);
			this.checkBoxPreviewOnly.TabIndex = 9;
			this.checkBoxPreviewOnly.Text = "Preview Only (log files)";
			this.checkBoxPreviewOnly.UseVisualStyleBackColor = true;
			this.checkBoxPreviewOnly.CheckedChanged += new System.EventHandler(this.checkBoxDoNothing_CheckedChanged);
			// 
			// checkBoxWithinSelf
			// 
			this.checkBoxWithinSelf.AutoSize = true;
			this.checkBoxWithinSelf.Location = new System.Drawing.Point(238, 12);
			this.checkBoxWithinSelf.Name = "checkBoxWithinSelf";
			this.checkBoxWithinSelf.Size = new System.Drawing.Size(148, 17);
			this.checkBoxWithinSelf.TabIndex = 10;
			this.checkBoxWithinSelf.Text = "Check within Primary Only";
			this.checkBoxWithinSelf.UseVisualStyleBackColor = true;
			this.checkBoxWithinSelf.CheckedChanged += new System.EventHandler(this.checkBoxWithinSelf_CheckedChanged);
			// 
			// backgroundWorker
			// 
			this.backgroundWorker.WorkerReportsProgress = true;
			this.backgroundWorker.WorkerSupportsCancellation = true;
			this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_DoWork);
			this.backgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker_ProgressChanged);
			this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker_Completed);
			// 
			// folderBrowserDialog
			// 
			this.folderBrowserDialog.RootFolder = System.Environment.SpecialFolder.MyComputer;
			this.folderBrowserDialog.ShowNewFolderButton = false;
			// 
			// buttonPrimary
			// 
			this.buttonPrimary.Location = new System.Drawing.Point(641, 99);
			this.buttonPrimary.Name = "buttonPrimary";
			this.buttonPrimary.Size = new System.Drawing.Size(35, 23);
			this.buttonPrimary.TabIndex = 11;
			this.buttonPrimary.Text = "...";
			this.buttonPrimary.UseVisualStyleBackColor = true;
			this.buttonPrimary.Click += new System.EventHandler(this.buttonPrimary_Click);
			// 
			// buttonSecondary
			// 
			this.buttonSecondary.Location = new System.Drawing.Point(641, 150);
			this.buttonSecondary.Name = "buttonSecondary";
			this.buttonSecondary.Size = new System.Drawing.Size(35, 23);
			this.buttonSecondary.TabIndex = 12;
			this.buttonSecondary.Text = "...";
			this.buttonSecondary.UseVisualStyleBackColor = true;
			this.buttonSecondary.Click += new System.EventHandler(this.buttonSecondary_Click);
			// 
			// buttonPrune
			// 
			this.buttonPrune.Location = new System.Drawing.Point(641, 188);
			this.buttonPrune.Name = "buttonPrune";
			this.buttonPrune.Size = new System.Drawing.Size(35, 23);
			this.buttonPrune.TabIndex = 13;
			this.buttonPrune.Text = "...";
			this.buttonPrune.UseVisualStyleBackColor = true;
			this.buttonPrune.Click += new System.EventHandler(this.buttonPrune_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.Location = new System.Drawing.Point(15, 314);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(170, 32);
			this.buttonCancel.TabIndex = 14;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// checkBoxNameCompare
			// 
			this.checkBoxNameCompare.AutoSize = true;
			this.checkBoxNameCompare.Location = new System.Drawing.Point(449, 12);
			this.checkBoxNameCompare.Name = "checkBoxNameCompare";
			this.checkBoxNameCompare.Size = new System.Drawing.Size(207, 17);
			this.checkBoxNameCompare.TabIndex = 15;
			this.checkBoxNameCompare.Text = "Compare Filenames instead of Content";
			this.checkBoxNameCompare.UseVisualStyleBackColor = true;
			// 
			// toolTips
			// 
			this.toolTips.ShowAlways = true;
			// 
			// textBoxExtensions
			// 
			this.textBoxExtensions.Location = new System.Drawing.Point(274, 44);
			this.textBoxExtensions.Name = "textBoxExtensions";
			this.textBoxExtensions.Size = new System.Drawing.Size(388, 20);
			this.textBoxExtensions.TabIndex = 17;
			// 
			// comboBoxFilters
			// 
			this.comboBoxFilters.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxFilters.FormattingEnabled = true;
			this.comboBoxFilters.Items.AddRange(new object[] {
            "                                 No Filters (Compare all files)",
            "        Only Compare files WITH these extensions:",
            "Only Compare files WITHOUT these extensions:"});
			this.comboBoxFilters.Location = new System.Drawing.Point(15, 44);
			this.comboBoxFilters.Name = "comboBoxFilters";
			this.comboBoxFilters.Size = new System.Drawing.Size(253, 21);
			this.comboBoxFilters.TabIndex = 22;
			this.comboBoxFilters.SelectedIndexChanged += new System.EventHandler(this.comboBoxFilters_SelectedIndexChanged);
			// 
			// MainForm
			// 
			this.AcceptButton = this.MoveDupesButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.ClientSize = new System.Drawing.Size(683, 363);
			this.Controls.Add(this.comboBoxFilters);
			this.Controls.Add(this.textBoxExtensions);
			this.Controls.Add(this.checkBoxNameCompare);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonPrune);
			this.Controls.Add(this.buttonSecondary);
			this.Controls.Add(this.buttonPrimary);
			this.Controls.Add(this.checkBoxWithinSelf);
			this.Controls.Add(this.checkBoxPreviewOnly);
			this.Controls.Add(this.PruneDumpLabel);
			this.Controls.Add(this.PruneDumpTextbox);
			this.Controls.Add(this.SecondaryLabel);
			this.Controls.Add(this.PrimaryLabel);
			this.Controls.Add(this.SecondaryPathTextbox);
			this.Controls.Add(this.PrimaryPathTextbox);
			this.Controls.Add(this.ProgressLabel);
			this.Controls.Add(this.pruneProgressBar);
			this.Controls.Add(this.MoveDupesButton);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.HelpButton = true;
			this.MaximizeBox = false;
			this.Name = "MainForm";
			this.Text = "File Dupe Pruner";
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button MoveDupesButton;
		private System.Windows.Forms.ProgressBar pruneProgressBar;
		private System.Windows.Forms.Label ProgressLabel;
		private System.Windows.Forms.TextBox PrimaryPathTextbox;
		private System.Windows.Forms.TextBox SecondaryPathTextbox;
		private System.Windows.Forms.Label PrimaryLabel;
		private System.Windows.Forms.Label SecondaryLabel;
		private System.Windows.Forms.TextBox PruneDumpTextbox;
		private System.Windows.Forms.Label PruneDumpLabel;
		private System.Windows.Forms.CheckBox checkBoxPreviewOnly;
		private System.Windows.Forms.CheckBox checkBoxWithinSelf;
		private System.ComponentModel.BackgroundWorker backgroundWorker;
		private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
		private System.Windows.Forms.Button buttonPrimary;
		private System.Windows.Forms.Button buttonSecondary;
		private System.Windows.Forms.Button buttonPrune;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.CheckBox checkBoxNameCompare;
		private System.Windows.Forms.ToolTip toolTips;
		private System.Windows.Forms.TextBox textBoxExtensions;
		private System.Windows.Forms.ComboBox comboBoxFilters;
	}
}


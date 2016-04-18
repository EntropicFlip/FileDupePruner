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
			this.MoveDupesButton = new System.Windows.Forms.Button();
			this.pruneProgressBar = new System.Windows.Forms.ProgressBar();
			this.ProgressLabel = new System.Windows.Forms.Label();
			this.PrimaryPathTextbox = new System.Windows.Forms.TextBox();
			this.SecondaryPathTextbox = new System.Windows.Forms.TextBox();
			this.PrimaryLabel = new System.Windows.Forms.Label();
			this.SecondaryLabel = new System.Windows.Forms.Label();
			this.PruneDumpTextbox = new System.Windows.Forms.TextBox();
			this.PruneDumpLabel = new System.Windows.Forms.Label();
			this.checkBoxDoNothing = new System.Windows.Forms.CheckBox();
			this.checkBoxWithinSelf = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// MoveDupesButton
			// 
			this.MoveDupesButton.Location = new System.Drawing.Point(472, 297);
			this.MoveDupesButton.Name = "MoveDupesButton";
			this.MoveDupesButton.Size = new System.Drawing.Size(189, 33);
			this.MoveDupesButton.TabIndex = 0;
			this.MoveDupesButton.Text = "Move Dupes To Another Folder";
			this.MoveDupesButton.UseVisualStyleBackColor = true;
			this.MoveDupesButton.Click += new System.EventHandler(this.MoveDupesButton_Click);
			// 
			// pruneProgressBar
			// 
			this.pruneProgressBar.Location = new System.Drawing.Point(15, 257);
			this.pruneProgressBar.Name = "pruneProgressBar";
			this.pruneProgressBar.Size = new System.Drawing.Size(646, 21);
			this.pruneProgressBar.TabIndex = 1;
			// 
			// ProgressLabel
			// 
			this.ProgressLabel.AutoSize = true;
			this.ProgressLabel.Location = new System.Drawing.Point(12, 241);
			this.ProgressLabel.Name = "ProgressLabel";
			this.ProgressLabel.Size = new System.Drawing.Size(48, 13);
			this.ProgressLabel.TabIndex = 2;
			this.ProgressLabel.Text = "Progress";
			// 
			// PrimaryPathTextbox
			// 
			this.PrimaryPathTextbox.AllowDrop = true;
			this.PrimaryPathTextbox.Location = new System.Drawing.Point(15, 75);
			this.PrimaryPathTextbox.Name = "PrimaryPathTextbox";
			this.PrimaryPathTextbox.Size = new System.Drawing.Size(646, 20);
			this.PrimaryPathTextbox.TabIndex = 3;
			this.PrimaryPathTextbox.TextChanged += new System.EventHandler(this.PrimaryPathTextbox_TextChanged);
			this.PrimaryPathTextbox.DragDrop += new System.Windows.Forms.DragEventHandler(this.OnDragDrop);
			this.PrimaryPathTextbox.DragEnter += new System.Windows.Forms.DragEventHandler(this.OnDragEnter);
			// 
			// SecondaryPathTextbox
			// 
			this.SecondaryPathTextbox.AllowDrop = true;
			this.SecondaryPathTextbox.Location = new System.Drawing.Point(15, 119);
			this.SecondaryPathTextbox.Name = "SecondaryPathTextbox";
			this.SecondaryPathTextbox.Size = new System.Drawing.Size(644, 20);
			this.SecondaryPathTextbox.TabIndex = 4;
			this.SecondaryPathTextbox.TextChanged += new System.EventHandler(this.SecondaryPathTextbox_TextChanged);
			this.SecondaryPathTextbox.DragDrop += new System.Windows.Forms.DragEventHandler(this.OnDragDrop);
			this.SecondaryPathTextbox.DragEnter += new System.Windows.Forms.DragEventHandler(this.OnDragEnter);
			// 
			// PrimaryLabel
			// 
			this.PrimaryLabel.AutoSize = true;
			this.PrimaryLabel.Location = new System.Drawing.Point(12, 50);
			this.PrimaryLabel.Name = "PrimaryLabel";
			this.PrimaryLabel.Size = new System.Drawing.Size(134, 13);
			this.PrimaryLabel.TabIndex = 5;
			this.PrimaryLabel.Text = "Primary Folder To Preserve";
			// 
			// SecondaryLabel
			// 
			this.SecondaryLabel.AutoSize = true;
			this.SecondaryLabel.Location = new System.Drawing.Point(12, 103);
			this.SecondaryLabel.Name = "SecondaryLabel";
			this.SecondaryLabel.Size = new System.Drawing.Size(301, 13);
			this.SecondaryLabel.TabIndex = 6;
			this.SecondaryLabel.Text = "Secondary Folder to Prune (Drag a folder for easy path setting)";
			// 
			// PruneDumpTextbox
			// 
			this.PruneDumpTextbox.AllowDrop = true;
			this.PruneDumpTextbox.Location = new System.Drawing.Point(15, 174);
			this.PruneDumpTextbox.Name = "PruneDumpTextbox";
			this.PruneDumpTextbox.Size = new System.Drawing.Size(646, 20);
			this.PruneDumpTextbox.TabIndex = 7;
			this.PruneDumpTextbox.DragDrop += new System.Windows.Forms.DragEventHandler(this.OnDragDrop);
			this.PruneDumpTextbox.DragEnter += new System.Windows.Forms.DragEventHandler(this.OnDragEnter);
			// 
			// PruneDumpLabel
			// 
			this.PruneDumpLabel.AutoSize = true;
			this.PruneDumpLabel.Location = new System.Drawing.Point(12, 158);
			this.PruneDumpLabel.Name = "PruneDumpLabel";
			this.PruneDumpLabel.Size = new System.Drawing.Size(132, 13);
			this.PruneDumpLabel.TabIndex = 8;
			this.PruneDumpLabel.Text = "Place to dump pruned files";
			// 
			// checkBoxDoNothing
			// 
			this.checkBoxDoNothing.AutoSize = true;
			this.checkBoxDoNothing.Checked = true;
			this.checkBoxDoNothing.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxDoNothing.Location = new System.Drawing.Point(15, 12);
			this.checkBoxDoNothing.Name = "checkBoxDoNothing";
			this.checkBoxDoNothing.Size = new System.Drawing.Size(177, 17);
			this.checkBoxDoNothing.TabIndex = 9;
			this.checkBoxDoNothing.Text = "Preview Only (generate log files)";
			this.checkBoxDoNothing.UseVisualStyleBackColor = true;
			this.checkBoxDoNothing.CheckedChanged += new System.EventHandler(this.checkBoxDoNothing_CheckedChanged);
			// 
			// checkBoxWithinSelf
			// 
			this.checkBoxWithinSelf.AutoSize = true;
			this.checkBoxWithinSelf.Location = new System.Drawing.Point(283, 12);
			this.checkBoxWithinSelf.Name = "checkBoxWithinSelf";
			this.checkBoxWithinSelf.Size = new System.Drawing.Size(216, 17);
			this.checkBoxWithinSelf.TabIndex = 10;
			this.checkBoxWithinSelf.Text = "Check for Duplicates within Primary Only";
			this.checkBoxWithinSelf.UseVisualStyleBackColor = true;
			this.checkBoxWithinSelf.CheckedChanged += new System.EventHandler(this.checkBoxWithinSelf_CheckedChanged);
			// 
			// MainForm
			// 
			this.AcceptButton = this.MoveDupesButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(683, 363);
			this.Controls.Add(this.checkBoxWithinSelf);
			this.Controls.Add(this.checkBoxDoNothing);
			this.Controls.Add(this.PruneDumpLabel);
			this.Controls.Add(this.PruneDumpTextbox);
			this.Controls.Add(this.SecondaryLabel);
			this.Controls.Add(this.PrimaryLabel);
			this.Controls.Add(this.SecondaryPathTextbox);
			this.Controls.Add(this.PrimaryPathTextbox);
			this.Controls.Add(this.ProgressLabel);
			this.Controls.Add(this.pruneProgressBar);
			this.Controls.Add(this.MoveDupesButton);
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
		private System.Windows.Forms.CheckBox checkBoxDoNothing;
		private System.Windows.Forms.CheckBox checkBoxWithinSelf;
	}
}


namespace DynamicConfirmation
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
			this.instrumentStatusDashboard = new System.Windows.Forms.Button();
			this.jobSelectedTextBox = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.systemStatusLabel = new System.Windows.Forms.Label();
			this.killJob = new System.Windows.Forms.Button();
			this.pause = new System.Windows.Forms.Button();
			this.addJob = new System.Windows.Forms.Button();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.progressBar1 = new System.Windows.Forms.ProgressBar();
			this.taskQueueGrid = new System.Windows.Forms.DataGridView();
			this.label3 = new System.Windows.Forms.Label();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.label1 = new System.Windows.Forms.Label();
			this.jobQueueGrid = new System.Windows.Forms.DataGridView();
			this.fileSystemWatcher1 = new System.IO.FileSystemWatcher();
			((System.ComponentModel.ISupportInitialize)(this.taskQueueGrid)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.jobQueueGrid)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher1)).BeginInit();
			this.SuspendLayout();
			// 
			// instrumentStatusDashboard
			// 
			this.instrumentStatusDashboard.BackColor = System.Drawing.Color.Honeydew;
			this.instrumentStatusDashboard.Font = new System.Drawing.Font("Cambria", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.instrumentStatusDashboard.Location = new System.Drawing.Point(258, 610);
			this.instrumentStatusDashboard.Name = "instrumentStatusDashboard";
			this.instrumentStatusDashboard.Size = new System.Drawing.Size(199, 32);
			this.instrumentStatusDashboard.TabIndex = 32;
			this.instrumentStatusDashboard.Text = "Instrument Status Dashboard";
			this.instrumentStatusDashboard.UseVisualStyleBackColor = false;
			this.instrumentStatusDashboard.Click += new System.EventHandler(this.instrumentStatusDashboard_Click);
			// 
			// jobSelectedTextBox
			// 
			this.jobSelectedTextBox.Location = new System.Drawing.Point(320, 195);
			this.jobSelectedTextBox.Name = "jobSelectedTextBox";
			this.jobSelectedTextBox.Size = new System.Drawing.Size(266, 20);
			this.jobSelectedTextBox.TabIndex = 31;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Font = new System.Drawing.Font("Cambria", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label6.Location = new System.Drawing.Point(238, 197);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(76, 15);
			this.label6.TabIndex = 30;
			this.label6.Text = "Job Selected:";
			// 
			// systemStatusLabel
			// 
			this.systemStatusLabel.AutoSize = true;
			this.systemStatusLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.systemStatusLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.systemStatusLabel.Location = new System.Drawing.Point(240, 159);
			this.systemStatusLabel.Name = "systemStatusLabel";
			this.systemStatusLabel.Size = new System.Drawing.Size(128, 22);
			this.systemStatusLabel.TabIndex = 29;
			this.systemStatusLabel.Text = "System Status";
			// 
			// killJob
			// 
			this.killJob.BackColor = System.Drawing.Color.Salmon;
			this.killJob.Font = new System.Drawing.Font("Cambria", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.killJob.Location = new System.Drawing.Point(427, 682);
			this.killJob.Name = "killJob";
			this.killJob.Size = new System.Drawing.Size(83, 32);
			this.killJob.TabIndex = 28;
			this.killJob.Text = "Kill Job";
			this.killJob.UseVisualStyleBackColor = false;
			this.killJob.Click += new System.EventHandler(this.killJob_Click);
			// 
			// pause
			// 
			this.pause.BackColor = System.Drawing.Color.Khaki;
			this.pause.Font = new System.Drawing.Font("Cambria", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.pause.Location = new System.Drawing.Point(319, 682);
			this.pause.Name = "pause";
			this.pause.Size = new System.Drawing.Size(83, 32);
			this.pause.TabIndex = 27;
			this.pause.Text = "Pause";
			this.pause.UseVisualStyleBackColor = false;
			this.pause.Click += new System.EventHandler(this.pause_Click);
			// 
			// addJob
			// 
			this.addJob.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
			this.addJob.Font = new System.Drawing.Font("Cambria", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.addJob.Location = new System.Drawing.Point(207, 682);
			this.addJob.Name = "addJob";
			this.addJob.Size = new System.Drawing.Size(83, 32);
			this.addJob.TabIndex = 26;
			this.addJob.Text = "Add Job";
			this.addJob.UseVisualStyleBackColor = false;
			this.addJob.Click += new System.EventHandler(this.addJob_Click);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Font = new System.Drawing.Font("Cambria", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label5.Location = new System.Drawing.Point(139, 166);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(86, 15);
			this.label5.TabIndex = 25;
			this.label5.Text = "System Status:";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("Cambria", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label4.Location = new System.Drawing.Point(139, 229);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(79, 15);
			this.label4.TabIndex = 24;
			this.label4.Text = "Job Progress:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Cambria", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(139, 276);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(66, 15);
			this.label2.TabIndex = 23;
			this.label2.Text = "Job Queue:";
			// 
			// progressBar1
			// 
			this.progressBar1.ForeColor = System.Drawing.Color.DodgerBlue;
			this.progressBar1.Location = new System.Drawing.Point(240, 221);
			this.progressBar1.Name = "progressBar1";
			this.progressBar1.Size = new System.Drawing.Size(346, 23);
			this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
			this.progressBar1.TabIndex = 22;
			// 
			// taskQueueGrid
			// 
			this.taskQueueGrid.AllowUserToAddRows = false;
			this.taskQueueGrid.AllowUserToDeleteRows = false;
			this.taskQueueGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.taskQueueGrid.Location = new System.Drawing.Point(142, 460);
			this.taskQueueGrid.Name = "taskQueueGrid";
			this.taskQueueGrid.ReadOnly = true;
			this.taskQueueGrid.Size = new System.Drawing.Size(444, 112);
			this.taskQueueGrid.TabIndex = 21;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("Cambria", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label3.Location = new System.Drawing.Point(139, 433);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(74, 15);
			this.label3.TabIndex = 20;
			this.label3.Text = "Task Queue:";
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = global::DashboardApplication.Properties.Resources.invitae_logo;
			this.pictureBox1.Location = new System.Drawing.Point(12, 12);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(125, 125);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pictureBox1.TabIndex = 19;
			this.pictureBox1.TabStop = false;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Corbel", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(164, 45);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(422, 36);
			this.label1.TabIndex = 18;
			this.label1.Text = "Confirmation System Dashboard";
			// 
			// jobQueueGrid
			// 
			this.jobQueueGrid.AllowUserToAddRows = false;
			this.jobQueueGrid.AllowUserToDeleteRows = false;
			this.jobQueueGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.jobQueueGrid.DataMember = "jobs";
			this.jobQueueGrid.Location = new System.Drawing.Point(142, 303);
			this.jobQueueGrid.Name = "jobQueueGrid";
			this.jobQueueGrid.ReadOnly = true;
			this.jobQueueGrid.Size = new System.Drawing.Size(444, 112);
			this.jobQueueGrid.TabIndex = 17;
			this.jobQueueGrid.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.jobQueueGrid_CellClick);
			// 
			// fileSystemWatcher1
			// 
			this.fileSystemWatcher1.EnableRaisingEvents = true;
			this.fileSystemWatcher1.SynchronizingObject = this;
			this.fileSystemWatcher1.Path = Globals.directory;
			this.fileSystemWatcher1.Changed += new System.IO.FileSystemEventHandler(this.fileSystemWatcher1_Changed);
			this.fileSystemWatcher1.Created += new System.IO.FileSystemEventHandler(this.fileSystemWatcher1_Created);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(734, 762);
			this.Controls.Add(this.instrumentStatusDashboard);
			this.Controls.Add(this.jobSelectedTextBox);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.systemStatusLabel);
			this.Controls.Add(this.killJob);
			this.Controls.Add(this.pause);
			this.Controls.Add(this.addJob);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.progressBar1);
			this.Controls.Add(this.taskQueueGrid);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.jobQueueGrid);
			this.Name = "MainForm";
			this.Text = "System Dashboard";
			((System.ComponentModel.ISupportInitialize)(this.taskQueueGrid)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.jobQueueGrid)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button instrumentStatusDashboard;
		private System.Windows.Forms.TextBox jobSelectedTextBox;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label systemStatusLabel;
		private System.Windows.Forms.Button killJob;
		private System.Windows.Forms.Button pause;
		private System.Windows.Forms.Button addJob;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ProgressBar progressBar1;
		private System.Windows.Forms.DataGridView taskQueueGrid;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.DataGridView jobQueueGrid;
		private System.IO.FileSystemWatcher fileSystemWatcher1;
	}
}


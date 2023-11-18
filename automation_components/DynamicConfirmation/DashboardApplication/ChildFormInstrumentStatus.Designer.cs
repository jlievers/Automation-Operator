namespace DynamicConfirmation
{
	partial class ChildFormInstrumentStatus
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
			this.instrumentStatusGrid = new System.Windows.Forms.DataGridView();
			this.label1 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.fileSystemWatcher1 = new System.IO.FileSystemWatcher();
			((System.ComponentModel.ISupportInitialize)(this.instrumentStatusGrid)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher1)).BeginInit();
			this.SuspendLayout();
			// 
			// instrumentStatusGrid
			// 
			this.instrumentStatusGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.instrumentStatusGrid.DataMember = "instruments";
			this.instrumentStatusGrid.Location = new System.Drawing.Point(12, 50);
			this.instrumentStatusGrid.Name = "instrumentStatusGrid";
			this.instrumentStatusGrid.Size = new System.Drawing.Size(344, 286);
			this.instrumentStatusGrid.TabIndex = 14;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.BackColor = System.Drawing.Color.Transparent;
			this.label1.Font = new System.Drawing.Font("Cambria", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.ForeColor = System.Drawing.Color.Maroon;
			this.label1.Location = new System.Drawing.Point(151, 28);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(69, 19);
			this.label1.TabIndex = 13;
			this.label1.Text = "Not Live";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("Cambria", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label4.Location = new System.Drawing.Point(122, 9);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(136, 19);
			this.label4.TabIndex = 12;
			this.label4.Text = "Instrument Status:";
			// 
			// fileSystemWatcher1
			// 
			this.fileSystemWatcher1.EnableRaisingEvents = true;
			this.fileSystemWatcher1.Path = "C:\\DynamicConfirmation\\Json Files";
			this.fileSystemWatcher1.SynchronizingObject = this;
			this.fileSystemWatcher1.Changed += new System.IO.FileSystemEventHandler(this.fileSystemWatcher1_Changed);
			// 
			// ChildFormInstrumentStatus
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(369, 349);
			this.Controls.Add(this.instrumentStatusGrid);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.label4);
			this.Name = "ChildFormInstrumentStatus";
			this.Text = "Instrument Status Dashboard";
			((System.ComponentModel.ISupportInitialize)(this.instrumentStatusGrid)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.DataGridView instrumentStatusGrid;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label4;
		private System.IO.FileSystemWatcher fileSystemWatcher1;
	}
}
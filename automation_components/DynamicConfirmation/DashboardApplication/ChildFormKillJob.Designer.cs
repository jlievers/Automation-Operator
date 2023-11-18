namespace DynamicConfirmation
{
	partial class ChildFormKillJob
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
			this.cancelKillJob = new System.Windows.Forms.Button();
			this.killJob = new System.Windows.Forms.Button();
			this.wfTextBox = new System.Windows.Forms.TextBox();
			this.userComboBox = new System.Windows.Forms.ComboBox();
			this.protocolComboBox = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// cancelKillJob
			// 
			this.cancelKillJob.BackColor = System.Drawing.Color.Salmon;
			this.cancelKillJob.Font = new System.Drawing.Font("Cambria", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cancelKillJob.Location = new System.Drawing.Point(150, 261);
			this.cancelKillJob.Name = "cancelKillJob";
			this.cancelKillJob.Size = new System.Drawing.Size(83, 32);
			this.cancelKillJob.TabIndex = 24;
			this.cancelKillJob.Text = "Cancel";
			this.cancelKillJob.UseVisualStyleBackColor = false;
			this.cancelKillJob.Click += new System.EventHandler(this.cancelKillJob_Click);
			// 
			// killJob
			// 
			this.killJob.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
			this.killJob.Font = new System.Drawing.Font("Cambria", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.killJob.Location = new System.Drawing.Point(55, 261);
			this.killJob.Name = "killJob";
			this.killJob.Size = new System.Drawing.Size(83, 32);
			this.killJob.TabIndex = 23;
			this.killJob.Text = "Kill Job";
			this.killJob.UseVisualStyleBackColor = false;
			this.killJob.Click += new System.EventHandler(this.killJob_Click);
			// 
			// wfTextBox
			// 
			this.wfTextBox.Font = new System.Drawing.Font("Cambria", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.wfTextBox.Location = new System.Drawing.Point(55, 173);
			this.wfTextBox.Name = "wfTextBox";
			this.wfTextBox.Size = new System.Drawing.Size(178, 23);
			this.wfTextBox.TabIndex = 22;
			this.wfTextBox.Text = "WFXXXX";
			// 
			// userComboBox
			// 
			this.userComboBox.Font = new System.Drawing.Font("Cambria", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.userComboBox.FormattingEnabled = true;
			this.userComboBox.Location = new System.Drawing.Point(55, 121);
			this.userComboBox.Name = "userComboBox";
			this.userComboBox.Size = new System.Drawing.Size(178, 23);
			this.userComboBox.TabIndex = 21;
			this.userComboBox.Text = "User";
			// 
			// protocolComboBox
			// 
			this.protocolComboBox.Font = new System.Drawing.Font("Cambria", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.protocolComboBox.FormattingEnabled = true;
			this.protocolComboBox.Location = new System.Drawing.Point(55, 73);
			this.protocolComboBox.Name = "protocolComboBox";
			this.protocolComboBox.Size = new System.Drawing.Size(178, 23);
			this.protocolComboBox.TabIndex = 20;
			this.protocolComboBox.Text = "Protocol";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Cambria", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(111, 30);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(64, 19);
			this.label1.TabIndex = 19;
			this.label1.Text = "Job Info";
			// 
			// ChildFormKillJob
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 337);
			this.Controls.Add(this.cancelKillJob);
			this.Controls.Add(this.killJob);
			this.Controls.Add(this.wfTextBox);
			this.Controls.Add(this.userComboBox);
			this.Controls.Add(this.protocolComboBox);
			this.Controls.Add(this.label1);
			this.Name = "ChildFormKillJob";
			this.Text = "Kill Job";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button cancelKillJob;
		private System.Windows.Forms.Button killJob;
		private System.Windows.Forms.TextBox wfTextBox;
		private System.Windows.Forms.ComboBox userComboBox;
		private System.Windows.Forms.ComboBox protocolComboBox;
		private System.Windows.Forms.Label label1;
	}
}
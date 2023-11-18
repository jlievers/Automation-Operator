namespace DynamicConfirmation
{
	partial class ChildFormAddJob
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
			this.radioButton1 = new System.Windows.Forms.RadioButton();
			this.cancelAddJob = new System.Windows.Forms.Button();
			this.addJob = new System.Windows.Forms.Button();
			this.wfTextBox = new System.Windows.Forms.TextBox();
			this.userComboBox = new System.Windows.Forms.ComboBox();
			this.protocolComboBox = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// radioButton1
			// 
			this.radioButton1.AutoSize = true;
			this.radioButton1.Location = new System.Drawing.Point(99, 217);
			this.radioButton1.Name = "radioButton1";
			this.radioButton1.Size = new System.Drawing.Size(76, 17);
			this.radioButton1.TabIndex = 21;
			this.radioButton1.TabStop = true;
			this.radioButton1.Text = "Priority Job";
			this.radioButton1.UseVisualStyleBackColor = true;
			// 
			// cancelAddJob
			// 
			this.cancelAddJob.BackColor = System.Drawing.Color.Salmon;
			this.cancelAddJob.Font = new System.Drawing.Font("Cambria", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cancelAddJob.Location = new System.Drawing.Point(148, 259);
			this.cancelAddJob.Name = "cancelAddJob";
			this.cancelAddJob.Size = new System.Drawing.Size(83, 32);
			this.cancelAddJob.TabIndex = 20;
			this.cancelAddJob.Text = "Cancel";
			this.cancelAddJob.UseVisualStyleBackColor = false;
			this.cancelAddJob.Click += new System.EventHandler(this.cancelAddJob_Click);
			// 
			// addJob
			// 
			this.addJob.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
			this.addJob.Font = new System.Drawing.Font("Cambria", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.addJob.Location = new System.Drawing.Point(53, 259);
			this.addJob.Name = "addJob";
			this.addJob.Size = new System.Drawing.Size(83, 32);
			this.addJob.TabIndex = 19;
			this.addJob.Text = "Add Job";
			this.addJob.UseVisualStyleBackColor = false;
			this.addJob.Click += new System.EventHandler(this.addJob_Click);
			// 
			// wfTextBox
			// 
			this.wfTextBox.Font = new System.Drawing.Font("Cambria", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.wfTextBox.Location = new System.Drawing.Point(53, 171);
			this.wfTextBox.Name = "wfTextBox";
			this.wfTextBox.Size = new System.Drawing.Size(178, 23);
			this.wfTextBox.TabIndex = 18;
			this.wfTextBox.Text = "WFXXXX";
			// 
			// userComboBox
			// 
			this.userComboBox.Font = new System.Drawing.Font("Cambria", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.userComboBox.FormattingEnabled = true;
			this.userComboBox.Location = new System.Drawing.Point(53, 119);
			this.userComboBox.Name = "userComboBox";
			this.userComboBox.Size = new System.Drawing.Size(178, 23);
			this.userComboBox.TabIndex = 17;
			this.userComboBox.Text = "User";
			// 
			// protocolComboBox
			// 
			this.protocolComboBox.Font = new System.Drawing.Font("Cambria", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.protocolComboBox.FormattingEnabled = true;
			this.protocolComboBox.Location = new System.Drawing.Point(53, 71);
			this.protocolComboBox.Name = "protocolComboBox";
			this.protocolComboBox.Size = new System.Drawing.Size(178, 23);
			this.protocolComboBox.TabIndex = 16;
			this.protocolComboBox.Text = "Protocol";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Cambria", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(109, 28);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(64, 19);
			this.label1.TabIndex = 15;
			this.label1.Text = "Job Info";
			// 
			// ChildFormAddJob
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 337);
			this.Controls.Add(this.radioButton1);
			this.Controls.Add(this.cancelAddJob);
			this.Controls.Add(this.addJob);
			this.Controls.Add(this.wfTextBox);
			this.Controls.Add(this.userComboBox);
			this.Controls.Add(this.protocolComboBox);
			this.Controls.Add(this.label1);
			this.Name = "ChildFormAddJob";
			this.Text = "Add Job";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.RadioButton radioButton1;
		private System.Windows.Forms.Button cancelAddJob;
		private System.Windows.Forms.Button addJob;
		private System.Windows.Forms.TextBox wfTextBox;
		private System.Windows.Forms.ComboBox userComboBox;
		private System.Windows.Forms.ComboBox protocolComboBox;
		private System.Windows.Forms.Label label1;
	}
}
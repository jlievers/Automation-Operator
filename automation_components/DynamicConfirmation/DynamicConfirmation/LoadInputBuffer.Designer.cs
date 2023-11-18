namespace DynamicConfirmation
{
	partial class LoadInputBuffer
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
			this.inventory = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.labwareListLabel = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// inventory
			// 
			this.inventory.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
			this.inventory.Font = new System.Drawing.Font("Cambria", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.inventory.Location = new System.Drawing.Point(152, 318);
			this.inventory.Name = "inventory";
			this.inventory.Size = new System.Drawing.Size(83, 32);
			this.inventory.TabIndex = 55;
			this.inventory.Text = "Inventory";
			this.inventory.UseVisualStyleBackColor = false;
			this.inventory.Click += new System.EventHandler(this.inventory_Click);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("Cambria", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label3.Location = new System.Drawing.Point(117, 9);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(156, 32);
			this.label3.TabIndex = 56;
			this.label3.Text = "Input Buffer";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Cambria", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(109, 55);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(164, 30);
			this.label1.TabIndex = 57;
			this.label1.Text = "Load the following labware\r\ninto the Input Buffer Stacker:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Cambria", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(92, 275);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(208, 30);
			this.label2.TabIndex = 58;
			this.label2.Text = "The system will inventory these\r\nreagents and get approval from LIMS";
			this.label2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// labwareListLabel
			// 
			this.labwareListLabel.AutoSize = true;
			this.labwareListLabel.Font = new System.Drawing.Font("Cambria", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labwareListLabel.Location = new System.Drawing.Point(28, 108);
			this.labwareListLabel.Name = "labwareListLabel";
			this.labwareListLabel.Size = new System.Drawing.Size(78, 15);
			this.labwareListLabel.TabIndex = 59;
			this.labwareListLabel.Text = "labware list";
			// 
			// LoadInputBuffer
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(384, 362);
			this.Controls.Add(this.labwareListLabel);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.inventory);
			this.Name = "LoadInputBuffer";
			this.Text = "Load Input Buffer";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button inventory;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label labwareListLabel;
	}
}
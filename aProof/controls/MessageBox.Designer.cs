namespace aProof.controls
{
	partial class MessageBox
	{
		/// <summary> 
		/// Wymagana zmienna projektanta.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Wyczyść wszystkie używane zasoby.
		/// </summary>
		/// <param name="disposing">prawda, jeżeli zarządzane zasoby powinny zostać zlikwidowane; Fałsz w przeciwnym wypadku.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Kod wygenerowany przez Projektanta składników

		/// <summary> 
		/// Metoda wymagana do obsługi projektanta — nie należy modyfikować 
		/// jej zawartości w edytorze kodu.
		/// </summary>
		private void InitializeComponent()
		{
			this.msgPanel = new System.Windows.Forms.Panel();
			this.msgLabel = new System.Windows.Forms.Label();
			this.idLabel = new System.Windows.Forms.Label();
			this.msgPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// msgPanel
			// 
			this.msgPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.msgPanel.AutoSize = true;
			this.msgPanel.BackColor = System.Drawing.Color.DarkOrange;
			this.msgPanel.Controls.Add(this.msgLabel);
			this.msgPanel.Location = new System.Drawing.Point(76, 6);
			this.msgPanel.Name = "msgPanel";
			this.msgPanel.Size = new System.Drawing.Size(404, 41);
			this.msgPanel.TabIndex = 0;
			// 
			// msgLabel
			// 
			this.msgLabel.AutoSize = true;
			this.msgLabel.BackColor = System.Drawing.Color.Transparent;
			this.msgLabel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.msgLabel.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.msgLabel.Location = new System.Drawing.Point(0, 0);
			this.msgLabel.MaximumSize = new System.Drawing.Size(392, 0);
			this.msgLabel.Name = "msgLabel";
			this.msgLabel.Padding = new System.Windows.Forms.Padding(8);
			this.msgLabel.Size = new System.Drawing.Size(80, 32);
			this.msgLabel.TabIndex = 0;
			this.msgLabel.Text = "msgLabel";
			// 
			// idLabel
			// 
			this.idLabel.BackColor = System.Drawing.Color.Transparent;
			this.idLabel.Font = new System.Drawing.Font("Forte", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.idLabel.ForeColor = System.Drawing.Color.Snow;
			this.idLabel.Location = new System.Drawing.Point(5, 9);
			this.idLabel.Name = "idLabel";
			this.idLabel.Size = new System.Drawing.Size(65, 44);
			this.idLabel.TabIndex = 1;
			this.idLabel.Text = "idLabel";
			// 
			// MessageBox
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.BackColor = System.Drawing.Color.Transparent;
			this.Controls.Add(this.idLabel);
			this.Controls.Add(this.msgPanel);
			this.DoubleBuffered = true;
			this.MinimumSize = new System.Drawing.Size(0, 50);
			this.Name = "MessageBox";
			this.Size = new System.Drawing.Size(490, 53);
			this.msgPanel.ResumeLayout(false);
			this.msgPanel.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Panel msgPanel;
		private System.Windows.Forms.Label msgLabel;
		private System.Windows.Forms.Label idLabel;
	}
}

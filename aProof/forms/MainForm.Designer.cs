namespace aProof
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
			this.headerPanel = new System.Windows.Forms.Panel();
			this.tcButton = new System.Windows.Forms.Button();
			this.titleLabel = new System.Windows.Forms.Label();
			this.minimizeLabel = new System.Windows.Forms.Label();
			this.exitLabel = new System.Windows.Forms.Label();
			this.contentPanel = new System.Windows.Forms.Panel();
			this.footerPanel = new System.Windows.Forms.Panel();
			this.settingsButton = new System.Windows.Forms.Button();
			this.progressBar1 = new System.Windows.Forms.ProgressBar();
			this.latiaButton = new System.Windows.Forms.Button();
			this.ccButton = new System.Windows.Forms.Button();
			this.mainGuiTimer = new System.Windows.Forms.Timer(this.components);
			this.headerPanel.SuspendLayout();
			this.footerPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// headerPanel
			// 
			this.headerPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
			this.headerPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.headerPanel.Controls.Add(this.tcButton);
			this.headerPanel.Controls.Add(this.titleLabel);
			this.headerPanel.Controls.Add(this.minimizeLabel);
			this.headerPanel.Controls.Add(this.exitLabel);
			this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.headerPanel.Location = new System.Drawing.Point(0, 0);
			this.headerPanel.Name = "headerPanel";
			this.headerPanel.Size = new System.Drawing.Size(500, 50);
			this.headerPanel.TabIndex = 0;
			this.headerPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.HeaderPanel_MouseDown);
			this.headerPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.HeaderPanel_MouseMove);
			this.headerPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.HeaderPanel_MouseUp);
			// 
			// tcButton
			// 
			this.tcButton.BackColor = System.Drawing.Color.Snow;
			this.tcButton.Font = new System.Drawing.Font("Rubik", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.tcButton.Location = new System.Drawing.Point(11, 11);
			this.tcButton.Name = "tcButton";
			this.tcButton.Size = new System.Drawing.Size(95, 27);
			this.tcButton.TabIndex = 3;
			this.tcButton.Text = "Test Chat";
			this.tcButton.UseVisualStyleBackColor = false;
			this.tcButton.Click += new System.EventHandler(this.TcButton_Click);
			// 
			// titleLabel
			// 
			this.titleLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.titleLabel.AutoSize = true;
			this.titleLabel.Font = new System.Drawing.Font("Monotype Corsiva", 15.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.titleLabel.ForeColor = System.Drawing.Color.Snow;
			this.titleLabel.Location = new System.Drawing.Point(206, 6);
			this.titleLabel.Name = "titleLabel";
			this.titleLabel.Size = new System.Drawing.Size(95, 25);
			this.titleLabel.TabIndex = 2;
			this.titleLabel.Text = "titleLabel";
			this.titleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// minimizeLabel
			// 
			this.minimizeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.minimizeLabel.AutoSize = true;
			this.minimizeLabel.Font = new System.Drawing.Font("Candara", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.minimizeLabel.ForeColor = System.Drawing.Color.Snow;
			this.minimizeLabel.Location = new System.Drawing.Point(438, 0);
			this.minimizeLabel.Name = "minimizeLabel";
			this.minimizeLabel.Size = new System.Drawing.Size(23, 26);
			this.minimizeLabel.TabIndex = 1;
			this.minimizeLabel.Text = "_";
			this.minimizeLabel.Click += new System.EventHandler(this.MinimizeLabel_Click);
			this.minimizeLabel.MouseLeave += new System.EventHandler(this.MouseLeaveChangeColor);
			this.minimizeLabel.MouseHover += new System.EventHandler(this.MouseHoverChangeColor);
			// 
			// exitLabel
			// 
			this.exitLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.exitLabel.AutoSize = true;
			this.exitLabel.Font = new System.Drawing.Font("Candara", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.exitLabel.ForeColor = System.Drawing.Color.Snow;
			this.exitLabel.Location = new System.Drawing.Point(468, 5);
			this.exitLabel.Name = "exitLabel";
			this.exitLabel.Size = new System.Drawing.Size(24, 26);
			this.exitLabel.TabIndex = 0;
			this.exitLabel.Text = "X";
			this.exitLabel.Click += new System.EventHandler(this.ExitLabel_Click);
			this.exitLabel.MouseLeave += new System.EventHandler(this.MouseLeaveChangeColor);
			this.exitLabel.MouseHover += new System.EventHandler(this.MouseHoverChangeColor);
			// 
			// contentPanel
			// 
			this.contentPanel.AutoScroll = true;
			this.contentPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
			this.contentPanel.BackgroundImage = global::aProof.Properties.Resources.ContentPanel_Background;
			this.contentPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.contentPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.contentPanel.ForeColor = System.Drawing.SystemColors.ControlText;
			this.contentPanel.Location = new System.Drawing.Point(0, 50);
			this.contentPanel.Name = "contentPanel";
			this.contentPanel.Size = new System.Drawing.Size(500, 620);
			this.contentPanel.TabIndex = 1;
			// 
			// footerPanel
			// 
			this.footerPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
			this.footerPanel.Controls.Add(this.settingsButton);
			this.footerPanel.Controls.Add(this.progressBar1);
			this.footerPanel.Controls.Add(this.latiaButton);
			this.footerPanel.Controls.Add(this.ccButton);
			this.footerPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.footerPanel.Location = new System.Drawing.Point(0, 670);
			this.footerPanel.Name = "footerPanel";
			this.footerPanel.Size = new System.Drawing.Size(500, 130);
			this.footerPanel.TabIndex = 2;
			// 
			// settingsButton
			// 
			this.settingsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
			this.settingsButton.BackColor = System.Drawing.Color.Snow;
			this.settingsButton.Font = new System.Drawing.Font("Rubik", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.settingsButton.Location = new System.Drawing.Point(4, 75);
			this.settingsButton.Name = "settingsButton";
			this.settingsButton.Size = new System.Drawing.Size(492, 27);
			this.settingsButton.TabIndex = 3;
			this.settingsButton.Text = "Settings";
			this.settingsButton.UseVisualStyleBackColor = false;
			this.settingsButton.Click += new System.EventHandler(this.SettingsButton_Click);
			this.settingsButton.MouseLeave += new System.EventHandler(this.MouseLeaveChangeColor);
			this.settingsButton.MouseHover += new System.EventHandler(this.MouseHoverChangeColor);
			// 
			// progressBar1
			// 
			this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.progressBar1.ForeColor = System.Drawing.Color.DarkOrange;
			this.progressBar1.Location = new System.Drawing.Point(4, 113);
			this.progressBar1.Name = "progressBar1";
			this.progressBar1.Size = new System.Drawing.Size(490, 12);
			this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
			this.progressBar1.TabIndex = 2;
			this.progressBar1.Visible = false;
			// 
			// latiaButton
			// 
			this.latiaButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
			this.latiaButton.BackColor = System.Drawing.Color.Snow;
			this.latiaButton.Font = new System.Drawing.Font("Rubik", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.latiaButton.Location = new System.Drawing.Point(4, 42);
			this.latiaButton.Name = "latiaButton";
			this.latiaButton.Size = new System.Drawing.Size(492, 27);
			this.latiaButton.TabIndex = 1;
			this.latiaButton.Text = "Let Agents Think in Advance";
			this.latiaButton.UseVisualStyleBackColor = false;
			this.latiaButton.Click += new System.EventHandler(this.LatiaButton_Click);
			this.latiaButton.MouseLeave += new System.EventHandler(this.MouseLeaveChangeColor);
			this.latiaButton.MouseHover += new System.EventHandler(this.MouseHoverChangeColor);
			// 
			// ccButton
			// 
			this.ccButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
			this.ccButton.BackColor = System.Drawing.Color.Snow;
			this.ccButton.Font = new System.Drawing.Font("Rubik", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.ccButton.Location = new System.Drawing.Point(4, 9);
			this.ccButton.Name = "ccButton";
			this.ccButton.Size = new System.Drawing.Size(492, 27);
			this.ccButton.TabIndex = 0;
			this.ccButton.Text = "Carry Conversation";
			this.ccButton.UseVisualStyleBackColor = false;
			this.ccButton.Click += new System.EventHandler(this.CcButton_Click);
			this.ccButton.MouseLeave += new System.EventHandler(this.MouseLeaveChangeColor);
			this.ccButton.MouseHover += new System.EventHandler(this.MouseHoverChangeColor);
			// 
			// mainGuiTimer
			// 
			this.mainGuiTimer.Interval = 1000;
			this.mainGuiTimer.Tick += new System.EventHandler(this.MainGuiTimer_Tick);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(500, 800);
			this.ControlBox = false;
			this.Controls.Add(this.footerPanel);
			this.Controls.Add(this.contentPanel);
			this.Controls.Add(this.headerPanel);
			this.DoubleBuffered = true;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.KeyPreview = true;
			this.MinimumSize = new System.Drawing.Size(500, 800);
			this.Name = "MainForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "MainForm";
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
			this.headerPanel.ResumeLayout(false);
			this.headerPanel.PerformLayout();
			this.footerPanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel headerPanel;
		private System.Windows.Forms.Panel contentPanel;
		private System.Windows.Forms.Panel footerPanel;
		private System.Windows.Forms.Button ccButton;
		private System.Windows.Forms.Label minimizeLabel;
		private System.Windows.Forms.Label exitLabel;
		private System.Windows.Forms.ProgressBar progressBar1;
		private System.Windows.Forms.Button latiaButton;
		private System.Windows.Forms.Label titleLabel;
		private System.Windows.Forms.Button settingsButton;
		private System.Windows.Forms.Timer mainGuiTimer;
		private System.Windows.Forms.Button tcButton;
	}
}
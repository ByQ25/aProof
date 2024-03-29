﻿using System.Drawing;
using System.Windows.Forms;

namespace aProof.controls
{
	public partial class MessageBox : UserControl
	{
		public string AuthorId
		{
			get => idLabel.Text;
			set => idLabel.Text = value;
		}
		public string Message
		{
			get => msgLabel.Text;
			set => msgLabel.Text = value;
		}

		public MessageBox()
		{
			InitializeComponent();
			this.DoubleBuffered = true;
			this.Dock = DockStyle.Top;
		}

		public MessageBox(string author, string inText, Color chatBubbleColor) : this()
		{
			this.AuthorId = author;
			this.Message = inText;
			this.msgPanel.BackColor = chatBubbleColor;
		}

		public MessageBox(string author, string inText) : this(author, inText, Color.DarkOrange) { }
	}
}

﻿using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows.Forms;

namespace aProof
{
	public partial class MainForm : Form
	{
		private bool isDragging;
		private Point startingPoint;
		private Task currentTask;
		private Random rng;
		private readonly Environment env;
		private readonly List<Tuple<uint, string>> ReadyMsgs;
		private Color[] availableChatBubbleColors;

		public MainForm()
		{
			InitializeComponent();
			LoadTranslations();
			this.tcButton.Visible = false;
			this.Icon = Properties.Resources.aProof_Icon;
			this.isDragging = false;
			this.rng = new Random();
			this.titleLabel.Text = typeof(AProofMain).Namespace;
			this.env = new Environment((int)SimulationSettings.Default.NUMBER_OF_AGENTS);
			this.ReadyMsgs = this.env.ReadyMsgs;
			this.availableChatBubbleColors = new Color[] {
				Color.HotPink, Color.OrangeRed, Color.Lime, Color.Khaki,
				Color.Yellow, Color.YellowGreen, Color.Turquoise, Color.Silver,
				Color.Azure, Color.Chartreuse, Color.Gold, Color.MediumSeaGreen,
				Color.DarkOrange, Color.DeepSkyBlue, Color.GreenYellow, Color.Orchid
			};
		}

		private void LoadTranslations()
		{
			this.tcButton.Text = src.PropTranslator.TranslateProp("prop.button.test_chat");
			this.ccButton.Text = src.PropTranslator.TranslateProp("prop.button.carry_conversation");
			this.latiaButton.Text = src.PropTranslator.TranslateProp("prop.button.let_agents_think");
			this.settingsButton.Text = src.PropTranslator.TranslateProp("prop.button.settings");
			this.instructionLabel.Text = src.PropTranslator.TranslateProp("prop.label.saving_conversation_instruction");
		}

		private void StartWorkingThread(Action<uint> action, uint input, bool shouldRenderMsgs)
		{
			TurnOnWorkInProgressMode();
			this.currentTask = new Task(
				() => action(input),
				TaskCreationOptions.LongRunning
			);
			this.currentTask.Start();
			this.mainGuiTimer.Start();
			if (shouldRenderMsgs)
				this.chatRendererTimer.Start();
		}

		private void StartWorkingThread(Action<uint> action, uint input)
		{
			StartWorkingThread(action, input, false);
		}

		private void TurnOnWorkInProgressMode()
		{
			this.tcButton.Enabled = false;
			this.ccButton.Enabled = false;
			this.latiaButton.Enabled = false;
			this.settingsButton.Enabled = false;
			this.progressBar1.Visible = true;
			this.progressBar1.Value = 0;
		}

		private void TurnOffWorkInProgressMode()
		{
			this.tcButton.Enabled = true;
			this.ccButton.Enabled = true;
			this.latiaButton.Enabled = true;
			this.settingsButton.Enabled = true;
			this.progressBar1.Visible = false;
		}

		private void ChangeColor(object controlObj, Color color)
		{
			if (controlObj is Label)
				(controlObj as Label).ForeColor = color;
			else if (controlObj is Button)
				(controlObj as Button).BackColor = color;
		}

		private Color ChooseChatBubbleColor(uint authorId)
		{
			return availableChatBubbleColors[authorId % availableChatBubbleColors.Length];
		}

		private string ChooseAuthorName(uint authorId)
		{
			return string.Format("A{0}", authorId);
		}

		public void AddChatMessage(string author, string inText, Color chatBubbleColor)
		{
			controls.MessageBox msgBox = new controls.MessageBox(author, inText, chatBubbleColor);
			contentPanel.SuspendLayout();
			contentPanel.Controls.Add(msgBox);
			msgBox.BringToFront();
			contentPanel.ResumeLayout();
			contentPanel.ScrollControlIntoView(msgBox);
		}

		private void SaveConversationToFile()
		{
			SaveFileDialog sfd = new SaveFileDialog
			{
				InitialDirectory = System.Environment.GetFolderPath(System.Environment.SpecialFolder.DesktopDirectory),
				AddExtension = true,
				DefaultExt = "txt",
				Filter = "Text files (*.txt)|*.txt"
			};
			if (sfd.ShowDialog() == DialogResult.OK && sfd.CheckPathExists)
				using (StreamWriter sw = new StreamWriter(sfd.OpenFile(), System.Text.Encoding.UTF8))
					foreach (Tuple<uint, string> msg in ReadyMsgs)
						sw.WriteLine(string.Format("Agent {0}:\n{1}\n", msg.Item1, msg.Item2));
		}

		private void CcButton_Click(object sender, EventArgs e)
		{
			contentPanel.Controls.Clear();
			availableChatBubbleColors = availableChatBubbleColors.OrderBy(i => rng.Next()).ToArray();
			StartWorkingThread(
				env.CarryConversation,
				SimulationSettings.Default.DEFAULT_THINKING_ITERATIONS,
				true
			);
		}

		private void LatiaButton_Click(object sender, EventArgs e)
		{
			contentPanel.Controls.Clear();
			StartWorkingThread(
				env.LetAgentsThinkInAdvance,
				SimulationSettings.Default.DEFAULT_THINKING_ITERATIONS
			);
		}

		private void SettingsButton_Click(object sender, EventArgs e)
		{
			MessageBox.Show(
				src.PropTranslator.TranslateProp("prop.settings.messagebox.text"),
				src.PropTranslator.TranslateProp("prop.settings.messagebox.title"),
				MessageBoxButtons.OK,
				MessageBoxIcon.Information
			);
			string
				settingsPath = System.IO.Path.GetFullPath(string.Format("{0}.exe.config", typeof(AProofMain).Namespace)),
				defaultEditor = (string)Microsoft.Win32.Registry.GetValue(
					@"HKEY_CLASSES_ROOT\SystemFileAssociations\text\shell\edit\command",
					null,
					null
				);
			defaultEditor = defaultEditor.Replace(" %1", "");
			System.Diagnostics.Process.Start(defaultEditor, settingsPath);
		}

		private void MinimizeLabel_Click(object sender, EventArgs e)
		{
			this.WindowState = FormWindowState.Minimized;
		}

		private void ExitLabel_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void MouseHoverChangeColor(object sender, EventArgs e)
		{
			ChangeColor(sender, Color.DarkOrange);
		}

		private void MouseLeaveChangeColor(object sender, EventArgs e)
		{
			ChangeColor(sender, Color.Snow);
		}
		
		private void HeaderPanel_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Clicks == 1)
			{
				isDragging = true;
				startingPoint = new Point(e.X, e.Y);
			}
		}

		private void HeaderPanel_MouseUp(object sender, MouseEventArgs e)
		{
			isDragging = false;
		}

		private void HeaderPanel_MouseMove(object sender, MouseEventArgs e)
		{
			if (isDragging)
			{
				Point p = PointToScreen(e.Location);
				this.Location = new Point(p.X - this.startingPoint.X, p.Y - this.startingPoint.Y);
			}
		}

		private void TcButton_Click(object sender, EventArgs e)
		{
			string
				author = ChooseAuthorName((uint)rng.Next(1, 100)),
				message = "";
			switch (rng.Next(0,3))
			{
				case 0: message = "Spare ribs picanha swine andouille, jerky chislic cupim rump biltong short loin salami kielbasa. Bresaola tri-tip landjaeger venison tongue drumstick fatback kevin ball tip brisket capicola pork boudin short ribs turducken. Kevin fatback jerky boudin shankle andouille turducken sausage jowl alcatra tenderloin salami leberkas chicken filet mignon."; break;
				case 1: message = "Leberkas venison kielbasa cow tri-tip chislic ribeye porchetta filet mignon. Alcatra tri-tip jerky hamburger, ham picanha pork loin doner meatloaf boudin fatback brisket sirloin jowl cupim."; break;
				case 2: message = "Bresaola capicola ham filet mignon jowl pig."; break;
				default: message = "Epic fail, my friend."; break;
			}
			AddChatMessage(
				author,
				message,
				ChooseChatBubbleColor(uint.Parse(author.Substring(1, author.Length - 1)))
			);
		}

		private void MainForm_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Control && e.Shift && e.KeyCode == Keys.T)
				tcButton.Visible = !tcButton.Visible;
			if (currentTask != null && currentTask.IsCompleted && e.Control && e.KeyCode == Keys.S)
				SaveConversationToFile();
		}

		private void MainGuiTimer_Tick(object sender, EventArgs e)
		{
			progressBar1.Value = env.Progress;
			progressBar1.Update();
			if (currentTask != null && currentTask.IsCompleted)
			{
				TurnOffWorkInProgressMode();
				this.mainGuiTimer.Stop();
			}
		}

		private void ChatRendererTimer_Tick(object sender, EventArgs e)
		{
			int
				i = contentPanel.Controls.Count,
				msgsCount = 0;
			lock (ReadyMsgs)
			{
				msgsCount = ReadyMsgs.Count;
				if (msgsCount > 0 && i < msgsCount)
				{
					Tuple<uint, string> msg = ReadyMsgs[i++];
					AddChatMessage(
						ChooseAuthorName(msg.Item1),
						msg.Item2,
						ChooseChatBubbleColor(msg.Item1)
					);
				}
				msgsCount = ReadyMsgs.Count;
			}
			if (
				currentTask != null 
				&& currentTask.IsCompleted
				&& i == msgsCount
			) this.chatRendererTimer.Stop();
		}
	}
}

using System;
using System.Drawing;
using System.Threading.Tasks;
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

		public MainForm()
		{
			InitializeComponent();
			this.tcButton.Visible = false;
			this.Icon = Properties.Resources.aProof_Icon;
			this.isDragging = false;
			this.rng = new Random();
			this.titleLabel.Text = typeof(AProofMain).Namespace;
			this.env = new Environment((int)SimulationSettings.Default.NUMBER_OF_AGENTS);
		}

		private void StartWorkingThread(Action<uint> action, uint input)
		{
			TurnOnWorkInProgressMode();
			this.currentTask = new Task(
				() => action(input),
				TaskCreationOptions.LongRunning
			);
			this.currentTask.Start();
			this.mainGuiTimer.Start();
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

		private void CcButton_Click(object sender, EventArgs e)
		{
			contentPanel.Controls.Clear();
			StartWorkingThread(
				env.CarryConversation,
				SimulationSettings.Default.DEFAULT_THINKING_ITERATIONS
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
				"The app has to be restarted to reload new settings. Please, restart the application after changing settings.",
				"Restart required",
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

		public void AddChatMessage(string author, string inText)
		{
			controls.MessageBox msgBox = new controls.MessageBox(author, inText);
			contentPanel.SuspendLayout();
			contentPanel.Controls.Add(msgBox);
			msgBox.BringToFront();
			contentPanel.ResumeLayout();
			contentPanel.ScrollControlIntoView(msgBox);
		}

		private void TcButton_Click(object sender, EventArgs e)
		{
			string
				author = string.Format("A{0}", rng.Next(1, 100)),
				message = "";
			switch (rng.Next(0,3))
			{
				case 0: message = "Spare ribs picanha swine andouille, jerky chislic cupim rump biltong short loin salami kielbasa. Bresaola tri-tip landjaeger venison tongue drumstick fatback kevin ball tip brisket capicola pork boudin short ribs turducken. Kevin fatback jerky boudin shankle andouille turducken sausage jowl alcatra tenderloin salami leberkas chicken filet mignon."; break;
				case 1: message = "Leberkas venison kielbasa cow tri-tip chislic ribeye porchetta filet mignon. Alcatra tri-tip jerky hamburger, ham picanha pork loin doner meatloaf boudin fatback brisket sirloin jowl cupim."; break;
				case 2: message = "Bresaola capicola ham filet mignon jowl pig."; break;
				default: message = "Epic fail, my friend."; break;
			}
			AddChatMessage(author, message);
		}

		private void MainForm_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Control && e.Shift && e.KeyCode == Keys.T)
				tcButton.Visible = !tcButton.Visible;
		}
	}
}

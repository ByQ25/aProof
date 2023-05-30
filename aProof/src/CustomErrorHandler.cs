using System;
using System.IO;
using System.Windows.Forms;

namespace aProof.src
{
	static class CustomErrorHandler
	{
		public enum LogLevel { INFO, ERROR, DEBUG };
		private static readonly string DebugLogFilePath =
			SimulationSettings.Default.DEBUG_FILE_PATH;

		public static void Log(string logMsg, LogLevel logLevel)
		{
			try
			{
				if (!File.Exists(DebugLogFilePath))
					File.Create(DebugLogFilePath).Close();
				using (StreamWriter sw = new StreamWriter(DebugLogFilePath, true))
					sw.WriteLine(
						string.Format("{0} {1}: {2}",
						DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
						logLevel.ToString(),
						logMsg
					)
				);
			}
			catch { return; }
		}

		public static void DisplayError(string errMsg)
		{
			MessageBox.Show(
				errMsg,
				"Error",
				MessageBoxButtons.OK,
				MessageBoxIcon.Error
			);
		}
	}
}

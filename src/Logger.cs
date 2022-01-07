using System;

namespace ESRIRegAsmConsole
{
	/// <summary>
	/// Simple console logger with colorful text.
	/// </summary>
	internal static class Logger
	{
		private const ConsoleColor SuccessColor = ConsoleColor.Green;
		private const ConsoleColor WarningColor = ConsoleColor.Yellow;
		private const ConsoleColor ErrorColor = ConsoleColor.Red;

		public static void Success(string message)
		{
			using var _ = new ForeColor(SuccessColor);
			Console.WriteLine(message);
		}

		public static void Warning(string message)
		{
			using var _ = new ForeColor(WarningColor);
			Console.WriteLine(message);
		}

		public static void Error(string message)
		{
			using var _ = new ForeColor(ErrorColor);
			Console.WriteLine(message);
		}

		#region Private classes

		private class ForeColor : IDisposable
		{
			private readonly ConsoleColor m_PreviousForegroundColor;

			public ForeColor(ConsoleColor foregroundColor)
			{
				m_PreviousForegroundColor = Console.ForegroundColor;
				Console.ForegroundColor = foregroundColor;
			}

			public void Dispose() => Console.ForegroundColor = m_PreviousForegroundColor;
		}

		#endregion
	}
}

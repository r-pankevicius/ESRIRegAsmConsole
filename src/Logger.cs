using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESRIRegAsmConsole
{
	/// <summary>
	/// Simple console logger with colorful text.
	/// </summary>
	internal static class Logger
	{
		public static void Success(string message)
		{
			using var _ = new SuccessForeColor();
			Console.WriteLine(message);
		}

		public static void Error(string message)
		{
			using var _ = new ErrorForeColor();
			Console.WriteLine(message);
		}

		#region Private classes

		private abstract class ForeColor : IDisposable
		{
			private readonly ConsoleColor m_PreviousForegroundColor;

			public ForeColor(ConsoleColor foregroundColor)
			{
				m_PreviousForegroundColor = Console.ForegroundColor;
				Console.ForegroundColor = foregroundColor;
			}

			public void Dispose() => Console.ForegroundColor = m_PreviousForegroundColor;
		}

		private class SuccessForeColor : ForeColor
		{
			public SuccessForeColor() : base(ConsoleColor.Green) { }

			public static void WriteLine(string line)
			{
				using (new ErrorForeColor())
					Console.WriteLine(line);
			}
		}

		private class ErrorForeColor : ForeColor
		{
			public ErrorForeColor() : base(ConsoleColor.Red) { }

			public static void WriteLine(string line)
			{
				using (new ErrorForeColor())
					Console.WriteLine(line);
			}
		}

		#endregion
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESRIRegAsmConsole
{
	/// <summary>
	/// Command line switch in form /name or /name:value
	/// </summary>
	internal class Switch
	{
		private static readonly char[] SplitArguments = new char[] { ':' };

		public string Name { get; set; }

		public string Value { get; set; }

		/// <summary>
		/// Tries to parse command line switches arguments.
		/// Switch is a string in form "/name" or "/name:value".
		/// </summary>
		/// <param name="arguments"></param>
		/// <returns></returns>
		public static IEnumerable<Switch> Parse(IEnumerable<string> arguments)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Parses command line argument switch: string in form "/name" or "/name:value.
		/// </summary>
		/// <param name="argument">Switch argument string (/name or /name:value)</param>
		/// <returns>Parsed switch or invalid switch if fails to parse.</returns>
		public static Switch Parse(string argument)
		{
			if (string.IsNullOrWhiteSpace(argument))
				throw new ArgumentException(nameof(argument));

			if (argument.Length < 2 || !argument.StartsWith("/"))
				return null;

			var result = new Switch();

			int columnIdx = argument.IndexOf(':');
			if (columnIdx == -1)
			{
				result.Name = argument.Substring(1);
			}
			else
			{
				result.Name = argument.Substring(1, columnIdx - 1);
				result.Value = argument.Substring(columnIdx + 1);
			}

			return result;
		}
	}
}

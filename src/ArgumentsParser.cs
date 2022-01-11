using System;
using System.IO;
using System.Linq;

namespace ESRIRegAsmConsole
{
	/// <summary>
	/// Parses program argument strings into <see cref="Arguments"/>.
	/// </summary>
	internal static class ArgumentsParser
	{
		public static Arguments Parse(string[] args)
		{
			if (args is null)
				throw new ArgumentNullException(nameof(args));

			if (args.Length != 1)
			{
				Logger.Warning("TODO: now only first DLL or list argument is supported; ... and action is `Register for desktop`.");
				return null;
			}

			string pathToFile;
			bool isDll;

			string firstParam = args[0];
			var listSwitch = Switch.TryParse(firstParam);
			if (listSwitch is null)
			{
				pathToFile = firstParam;
				isDll = true;
			}
			else
			{
				if (listSwitch.Value is null)
				{
					Logger.Error($"/list switch must have value.");
					return null;
				}

				pathToFile = listSwitch.Value;
				isDll = false;
			}

			if (!File.Exists(pathToFile))
			{
				Logger.Error($"File `{pathToFile}` doesn't exist.");
				return null;
			}

			// I don't know if ESRIRegAsm now supports relative path, but just in case, expand it to absolude path
			pathToFile = Path.GetFullPath(pathToFile);

			if (isDll)
			{
				string fileExtension = Path.GetExtension(pathToFile);
				if (fileExtension.ToLower() != ".dll")
					Logger.Warning($"File extension `{fileExtension}` doesn't look like DLL, are you sure you want to register/unregister it?..");
			}

			return new Arguments
			{
				PathToDllOrListingFile = pathToFile,
				IsDll = isDll
			};
		}

		/// <summary>
		/// Switch in form /name or /name:value
		/// </summary>
		private class Switch
		{
			private static readonly char[] SplitArguments = new char[] { ':' };

			public string Name { get; set; }

			public string Value { get; set; }

			public static Switch TryParse(string argument)
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
}

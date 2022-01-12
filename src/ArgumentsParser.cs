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

			if (args.Length < 1)
				return null;

			string pathToFile;
			bool isDll;

			string firstParam = args[0];
			var listSwitch = Switch.Parse(firstParam);
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

			var result = new Arguments
			{
				PathToDllOrListingFile = pathToFile,
				IsDll = isDll
			};

			return result;
		}
	}
}

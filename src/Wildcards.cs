using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ESRIRegAsmConsole
{
	/// <summary>
	/// For expanding wildcards in file names.
	/// </summary>
	internal static class Wildcards
	{
		public static IEnumerable<string> Expand(string fileNamePattern)
		{
			if (string.IsNullOrWhiteSpace(fileNamePattern))
				throw new ArgumentException(nameof(fileNamePattern));

			// Try to find folder
			int backslashIndex = fileNamePattern.LastIndexOf('\\');
			int slashIndex = fileNamePattern.LastIndexOf('/');
			int lastIndex = Math.Max(backslashIndex, slashIndex);
			if (lastIndex < 0)
				throw new InvalidOperationException($"Expected file name pattern to be absolute file pattern, actual: `{fileNamePattern}`");

			string directory = fileNamePattern.Substring(0, lastIndex);
			if (ContainsWildcards(directory))
				throw new ArgumentException($"Wildcards in directory names are not supported: `{directory}`");

			string fileName = fileNamePattern.Substring(lastIndex + 1);
			if (!ContainsWildcards(fileName))
			{
				return new string[] { fileNamePattern };
			}

			string directoryAbsolutePath = Path.GetFullPath(directory);
			return Directory.GetFiles(directoryAbsolutePath, fileName, SearchOption.TopDirectoryOnly);
		}

		public static bool ContainsWildcards(string fileName)
		{
			if (fileName is null)
				throw new ArgumentException(fileName);

			return fileName.Contains('*') || fileName.Contains('?');
		}
	}
}

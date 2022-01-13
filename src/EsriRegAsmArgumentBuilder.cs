using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ESRIRegAsmConsole
{
	internal static class EsriRegAsmArgumentBuilder
	{
		private const string CommonProgramFileEnvVar = "CommonProgramFiles(x86)";
		private const string RegAsmSubPath = @"ArcGIS\bin\ESRIRegAsm.exe";

		public static Result Build(Arguments arguments)
		{
			if (arguments is null)
				throw new ArgumentNullException(nameof(arguments));

			string commonProgramFilesDir = Environment.GetEnvironmentVariable(CommonProgramFileEnvVar);
			if (commonProgramFilesDir is null)
			{
				Logger.Error($"Environment variable {CommonProgramFileEnvVar} was not set. I couldn't find ESRIRegAsm executable.");
				return ErrorCode(11);
			}

			string pathToRegAsm = Path.Combine(commonProgramFilesDir, RegAsmSubPath);
			if (!File.Exists(pathToRegAsm))
			{
				Logger.Error($"File {pathToRegAsm} doesn't exist.");
				return ErrorCode(12);
			}

			string registerUnregisterParam = arguments.Register ? "" : "/u";

			var result = new Result();

			var allDlls = GetListOfDlls(arguments);
			foreach (string dll in allDlls)
			{
				result.ArgumentsList.Add(new ConsoleAppLaunchArguments
				{
					ExecutableName = pathToRegAsm,
					CommandLineArguments = $"{dll} {registerUnregisterParam} /p:{arguments.Product} /s /e"
				});
			}

			return result;
		}

		private static IEnumerable<string> GetListOfDlls(Arguments arguments)
		{
			if (arguments is null)
				throw new ArgumentNullException(nameof(arguments));

			if (arguments.IsDll)
			{
				return GetListOfDlls(arguments.PathToAssemblyOrListingFile, arguments.BasePath);
			}

			if (!File.Exists(arguments.PathToAssemblyOrListingFile))
			{
				Logger.Error($"File {arguments.PathToAssemblyOrListingFile} doesn't exist.");
				return new string[0];
			}

			return GetListOfDllsFromListingFile(arguments.PathToAssemblyOrListingFile, arguments.BasePath);
		}

		private static IEnumerable<string> GetListOfDllsFromListingFile(string pathToFile, string basePath) =>
			File.ReadLines(pathToFile)
				.Where(line => !string.IsNullOrWhiteSpace(line) && !line.StartsWith("#"))
				.SelectMany(line => GetListOfDlls(line, basePath));

		private static IEnumerable<string> GetListOfDlls(string filePath, string basePath)
		{
			string pathPattern = filePath;
			if (!Path.IsPathRooted(filePath) && !string.IsNullOrEmpty(basePath))
			{
				pathPattern = Path.Combine(basePath, filePath);
			}

			foreach (string pathToDll in Wildcards.Expand(pathPattern))
				yield return Path.GetFullPath(pathToDll);
		}

		private static Result ErrorCode(int errorCode) => new() { ErrorCode = errorCode };

		public class Result
		{
			public int ErrorCode { get; set; }

			public List<ConsoleAppLaunchArguments> ArgumentsList { get; set; } = new();
		}
	}
}

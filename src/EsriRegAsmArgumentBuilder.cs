using System;
using System.Collections.Generic;
using System.IO;

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

			if (!arguments.IsDll)
				throw new NotImplementedException("/list switch is not implemented.");

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
			result.ArgumentsList.Add(new ConsoleAppLaunchArguments
				{
					ExecutableName = pathToRegAsm,
					CommandLineArguments =
						$"{arguments.PathToAssemblyOrListingFile} {registerUnregisterParam} /p:{arguments.Product} /s /e"
				});
			//

			return result;
		}

		private static Result ErrorCode(int errorCode) => new() { ErrorCode = errorCode };

		public class Result
		{
			public int ErrorCode { get; set; }

			public List<ConsoleAppLaunchArguments> ArgumentsList { get; set; } = new();
		}
	}
}

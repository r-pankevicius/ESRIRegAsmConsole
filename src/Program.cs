using SlavaGu.ConsoleAppLauncher;
using System;
using System.Collections.Generic;
using System.IO;

namespace ESRIRegAsmConsole
{
	internal class Program
	{
		private const string CommonProgramFileEnvVar = "CommonProgramFiles(x86)";
		private const string RegAsmSubPath = @"ArcGIS\bin\ESRIRegAsm.exe";

		private static int Main(string[] args)
		{
			var arguments = ArgumentsParser.Parse(args);
			if (arguments is null)
			{
				return InvalidArguments();
			}

			// TODO
			if (!arguments.IsDll)
			{
				Logger.Error("/list argument isn't supported yet...");
				return 1;
			}

			var foregroundColor = Console.ForegroundColor;

			string commonProgramFilesDir = Environment.GetEnvironmentVariable(CommonProgramFileEnvVar);
			if (commonProgramFilesDir is null)
			{
				Logger.Error($"Environment variable {CommonProgramFileEnvVar} was not set.");
				return 11;
			}

			if (!Directory.Exists(commonProgramFilesDir))
			{
				Logger.Error($"Directory {CommonProgramFileEnvVar} doesn't exist.");
				return 12;
			}

			string pathToRegAsm = Path.Combine(commonProgramFilesDir, RegAsmSubPath);
			if (!File.Exists(pathToRegAsm))
			{
				Logger.Error($"File {pathToRegAsm} doesn't exist.");
				return 13;
			}

			var outputLines = new List<string>();
			string pathToDll = arguments.PathToDllOrListingFile;
			bool capturedOperationSucceededOutput = false;
			bool capturedPressEnterToContinue = false;

			using var consoleApp = new ConsoleApp(pathToRegAsm, $"{pathToDll} /p:Desktop /s /e");
			consoleApp.ConsoleOutput += (sender, arguments) =>
			{
				var consoleAppSender = (IConsoleApp)sender; // fail fast if ConsoleAppLauncher implementation has changed

				Console.WriteLine(arguments.Line);

				outputLines.Add(arguments.Line);

				if (arguments.Line.StartsWith("Operation Succeeded"))
				{
					capturedOperationSucceededOutput = true;
				}
				else if (arguments.Line == "Press Enter to continue...")
				{
					capturedPressEnterToContinue = true;
				}
			};

			consoleApp.Run();

			bool programFinished = consoleApp.WaitForExit(2000);
			if (!programFinished)
			{
				if (capturedOperationSucceededOutput || capturedPressEnterToContinue)
					consoleApp.Stop();
				else
					consoleApp.WaitForExit(500);
			}

			return capturedOperationSucceededOutput ? 0 : 100;
		}

		private static int InvalidArguments()
		{
			Logger.Error("Incorrect program arguments.");
			PrintUsage();
			return 1;
		}

		private static void PrintUsage()
		{
			string pathToEmbeddedRes = string.Concat(typeof(Program).Namespace, ".Embedded.Usage.txt");
			using Stream resourceStream = typeof(Program).Assembly.GetManifestResourceStream(pathToEmbeddedRes);
			using var reader = new StreamReader(resourceStream);
			string usageText = reader.ReadToEnd();
			Console.Write(usageText);
		}
	}
}

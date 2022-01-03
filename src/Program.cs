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
			var foregroundColor = Console.ForegroundColor;

			string commonProgramFilesDir = Environment.GetEnvironmentVariable(CommonProgramFileEnvVar);
			if (commonProgramFilesDir is null)
			{
				Console.WriteLine($"Environment variable {CommonProgramFileEnvVar} was not set.");
				return 1;
			}

			if (!Directory.Exists(commonProgramFilesDir))
			{
				Console.WriteLine($"Directory {CommonProgramFileEnvVar} doesn't exist.");
				return 2;
			}

			string pathToRegAsm = Path.Combine(commonProgramFilesDir, RegAsmSubPath);
			if (!File.Exists(pathToRegAsm))
			{
				Console.WriteLine($"File {pathToRegAsm} doesn't exist.");
				return 3;
			}

			// just proof of concept...
			var outputLines = new List<string>();
			//string pathToDll = @"nonexisting dll";
			string pathToDll = @"C:\Windows\notepad.exe";
			var consoleApp = new ConsoleApp(pathToRegAsm, $"{pathToDll} /p:Desktop /s /e");
			bool capturedOperationSucceededOutput = false;
			bool capturedPressEnterToContinue = false;

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
					// consoleAppSender.Stop(); // kills Windows PowerShell ISE, Windows PowerShell works OK
					// consoleAppSender.Write("\r"); // simulate ENTER key press
					capturedPressEnterToContinue = true;
				}
			};

			consoleApp.Run();

			bool programFinished = consoleApp.WaitForExit(2000);
			if (!programFinished)
			{
				if (capturedOperationSucceededOutput || capturedPressEnterToContinue)
				{
					consoleApp.Stop();
				}
				else
				{
					consoleApp.WaitForExit(500);
				}
			}

			if (capturedOperationSucceededOutput)
			{
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine("Looks like a success, ERRORLEVEL will be 0");
				Console.ForegroundColor = foregroundColor;
				return 0;
			}

			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("Looks like an error, ERRORLEVEL will be 100");
			Console.ForegroundColor = foregroundColor;
			return 100;
		}
	}
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SlavaGu.ConsoleAppLauncher;

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
			var consoleApp = new ConsoleApp(pathToRegAsm, @"C:\ddddddddddddddkskskskks.dll /p:Desktop /s /e");
			bool capturedOperationSucceededOutput = false;
			consoleApp.ConsoleOutput += (sender, arguments) =>
			{
				outputLines.Add(arguments.Line);
				if (arguments.Line.StartsWith("Operation Succeeded"))
					capturedOperationSucceededOutput = true;
				Console.WriteLine(arguments.Line);
				if (arguments.Line == "Press Enter to continue...")
					throw new Exception(); // doesn't work
			};

			consoleApp.Run();

			bool programFinished = consoleApp.WaitForExit(10000);
			if (!programFinished)
			{
				consoleApp.Stop();
			}

			if (capturedOperationSucceededOutput)
			{
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine("Looks like a success");
				Console.ForegroundColor = foregroundColor;
				return 0;
			}

			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("Looks like an error");
			Console.ForegroundColor = foregroundColor;
			return 100;
        }
    }
}

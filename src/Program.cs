using System;
using System.Collections.Generic;
using System.IO;
using SlavaGu.ConsoleAppLauncher;

namespace ESRIRegAsmConsole
{
	internal class Program
	{
		private static int Main(string[] args)
		{
			var arguments = ArgumentsParser.Parse(args);
			if (arguments is null)
			{
				return InvalidArguments();
			}

			var consoleAppArguments = EsriRegAsmArgumentBuilder.Build(arguments);
			if (consoleAppArguments.ErrorCode != 0)
			{
				return ErrorCode(consoleAppArguments.ErrorCode);
			}

			bool errorsOccured = false;

			foreach (var consoleAppArg in consoleAppArguments.ArgumentsList)
			{
				string pathToEsriRegAsmExe = consoleAppArg.ExecutableName;
				string commandLine = consoleAppArg.CommandLineArguments;

				var outputLines = new List<string>();
				bool capturedOperationSucceededOutput = false;
				bool capturedPressEnterToContinue = false;

				using var consoleApp = new ConsoleApp(pathToEsriRegAsmExe, commandLine);
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

				if (!capturedOperationSucceededOutput)
				{
					errorsOccured = true;
					if (!arguments.ContinueOnFail)
						break;
				}
			}

			return errorsOccured ? ErrorCode(100) : 0;
		}

		private static int ErrorCode(int errorCode)
		{
			Logger.Error($"Exiting with error code {errorCode}.");
			return errorCode;
		}

		private static int InvalidArguments()
		{
			Logger.Error("Incorrect program arguments.");
			PrintUsage();
			return ErrorCode(1);
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

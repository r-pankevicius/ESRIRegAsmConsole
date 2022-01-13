using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SlavaGu.ConsoleAppLauncher;

namespace ESRIRegAsmConsole
{
	internal class Program
	{
		/// <summary>
		/// Number of executions to trigger printing of summary: what commands succeeded and which failed.
		/// </summary>
		private const int PrintSummaryThreshold = 3;

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

			// To track which executed commands succeeded and failed
			var commandLinesRan = new Dictionary<string, bool>();
			bool errorsOccured = false;

			foreach (var consoleAppArg in consoleAppArguments.ArgumentsList)
			{
				string pathToEsriRegAsmExe = consoleAppArg.ExecutableName;
				string commandLineArguments = consoleAppArg.CommandLineArguments;

				bool capturedOperationSucceededOutput = false;
				bool capturedPressEnterToContinue = false;

				string commandLine = $"{pathToEsriRegAsmExe} {commandLineArguments}";
				Logger.Info($"Executing command:\n\t{commandLine}");

				using var consoleApp = new ConsoleApp(pathToEsriRegAsmExe, commandLineArguments);
				consoleApp.ConsoleOutput += (sender, arguments) =>
				{
					var consoleAppSender = (IConsoleApp)sender; // fail fast if ConsoleAppLauncher implementation has changed
					
					Logger.Info(arguments.Line);

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

				bool succeeded = capturedOperationSucceededOutput;
				commandLinesRan[commandLine] = succeeded;

				if (succeeded)
				{
					Logger.Success("Succeeded.");
				}
				else
				{
					Logger.Error("Failed.");
					errorsOccured = true;
					if (!arguments.ContinueOnFail)
						break;
				}
			}

			PrintSummaryIfNeeded(commandLinesRan, arguments.VerboseOutput);

			return errorsOccured ? ErrorCode(100) : 0;
		}

		private static void PrintSummaryIfNeeded(Dictionary<string, bool> commandLinesRan, bool verbose)
		{
			if (commandLinesRan.Count < PrintSummaryThreshold)
				return;

			var succeededCommands = commandLinesRan.Where(kv => kv.Value).Select(kv => kv.Key).ToArray();
			var failedCommands = commandLinesRan.Where(kv => !kv.Value).Select(kv => kv.Key).ToArray();

			Logger.Info(
				$"Execution summary:\n\tTotal: {commandLinesRan.Count}\n\tSucceeded: {succeededCommands.Length}\n\tFailed: {failedCommands.Length}\n");

			if (verbose && succeededCommands.Length > 0)
			{
				Logger.Info($"=== Succeeded ({succeededCommands.Length}): ===");
				foreach (string command in succeededCommands)
					Logger.Success(command);
			}

			if (failedCommands.Length > 0)
			{
				Logger.Info($"\n=== Failed ({failedCommands.Length}): ===");
				foreach (string command in failedCommands)
					Logger.Error(command);
			}
		}

		private static int ErrorCode(int errorCode)
		{
			if (errorCode != 0)
				Logger.Error($"Exiting with error code {errorCode}.");
			else
				Logger.Success("Succeeded.");

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
			Logger.Info(usageText);
		}
	}
}

using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Diagnostics;
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
			Arguments result = null;

			var parsedArgs = ParseCommandLineArgs(args);
			if (parsedArgs is not null)
				result = ConvertParsedCommandLineArgs(parsedArgs);

			return result;
		}

		private static IDictionary<string, object> ParseCommandLineArgs(string[] args)
		{
			var parser = new Parser(new ParserOptions { ArgumentPrefix = "/" });

			// Parser doesn't support the notion of optional positional argument, so add one
			// positional argument and process if it's DLL or /list switch later
			parser
				.AddPositionalArgument<string>(nameof(Arguments.PathToAssemblyOrListingFile))
				// TODO: Func<T, T, T> duplicateResolutionPolicy to fail if both /p:Desktop and /p:Server is specified
				.AddNamedArgument("p", "product-name", nameof(Arguments.Product), null, defaultValue: Product.Desktop)
				.AddFlagArgument<bool>("r", "register", nameof(Arguments.Register), null)
				.AddFlagArgument<bool>("u", "unregister", "Unregister", null)
				.AddNamedArgument<string>("regfile", "export-to-registry-file", nameof(Arguments.ExportToRegistryFile), null)
				.AddNamedArgument<string>("f", "category-mapping-file", nameof(Arguments.CategoryMappingFile), null)
				.AddNamedArgument<string>("w", "keep-working-files", nameof(Arguments.KeepWorkingFilesDirectory), null)
				.AddFlagArgument<bool>("v", "verbose", nameof(Arguments.VerboseOutput), null)
				.AddNamedArgument<string>("basepath", "basepath", nameof(Arguments.BasePath), null)
				.AddFlagArgument<bool>("continueonfail", "continueonfail", nameof(Arguments.ContinueOnFail), null);

			ParsingResults parsingResults;

			try
			{
				string processName = Process.GetCurrentProcess().ProcessName;
				string[] parserArgs = (new string[] { processName }).Concat(args).ToArray();
				parsingResults = parser.Parse(parserArgs);
				var firstArgument = (string)parsingResults.ParsedValues[nameof(Arguments.PathToAssemblyOrListingFile)];
				if (firstArgument.StartsWith("/list:"))
				{
					var listSwitchParser = new Parser(new ParserOptions { ArgumentPrefix = "/" });
					listSwitchParser.AddNamedArgument<string>("list", alias: null, "Listing", null);
					var listSwitchParseResults = listSwitchParser.Parse(new string[] { processName, firstArgument });
					parsingResults.ParsedValues[nameof(Arguments.PathToAssemblyOrListingFile)] =
						listSwitchParseResults.GetParsedValue("Listing");
					parsingResults.ParsedValues.Add(nameof(Arguments.IsDll), false);
				}
				else
				{
					parsingResults.ParsedValues.Add(nameof(Arguments.IsDll), true);
				}
			}
			catch (InvalidOperationException ex)
			{
				Logger.Error(ex.Message);
				return null;
			}

			return parsingResults.ParsedValues;
		}

		private static Arguments ConvertParsedCommandLineArgs(IDictionary<string, object> parsedArguments)
		{
			if (parsedArguments is null)
				throw new ArgumentNullException(nameof(parsedArguments));

			Arguments result = new()
			{
				PathToAssemblyOrListingFile = (string)parsedArguments[nameof(Arguments.PathToAssemblyOrListingFile)],
				IsDll = (bool)parsedArguments[nameof(Arguments.IsDll)]
			};

			bool register = (bool)parsedArguments[nameof(Arguments.Register)];
			bool unregister = (bool)parsedArguments["Unregister"];
			if (register && unregister)
			{
				Logger.Error("You can specify either /r or /u, not both.");
				return null;
			}

			result.Register = !unregister;

			result.BasePath = (string)parsedArguments[nameof(Arguments.BasePath)];
			result.Product = (Product)parsedArguments[nameof(Arguments.Product)];
			result.ExportToRegistryFile = (string)parsedArguments[nameof(Arguments.ExportToRegistryFile)];
			result.CategoryMappingFile = (string)parsedArguments[nameof(Arguments.CategoryMappingFile)];
			result.KeepWorkingFilesDirectory = (string)parsedArguments[nameof(Arguments.KeepWorkingFilesDirectory)];
			result.ContinueOnFail = (bool)parsedArguments[nameof(Arguments.ContinueOnFail)];
			result.VerboseOutput = (bool)parsedArguments[nameof(Arguments.VerboseOutput)];

			return result;
		}
	}
}

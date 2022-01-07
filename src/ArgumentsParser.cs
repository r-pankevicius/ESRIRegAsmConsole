using System;
using System.IO;

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

			if (args.Length !=1)
			{
				Logger.Warning("TODO: now only 1 DLL argument is supported and action is registed for desktop.");
				return null;
			}

			string pathToFile = args[0];

			if (!File.Exists(pathToFile))
			{
				Logger.Error($"File `{pathToFile}` doesn't exist.");
				return null;
			}

			// I don't know if ESRIRegAsm now supports relative path, but just in case, expand it to absolude path
			pathToFile = Path.GetFullPath(pathToFile);

			string fileExtension = Path.GetExtension(pathToFile).ToLower();
			if (fileExtension != ".dll")
			{
				Logger.Warning($"Fle extension `{fileExtension}` doesn't look like DLL, are you sure you want to register it?..");
			}

			return new Arguments
			{
				PathToDllOrListingFile = pathToFile
			};

			/*
			Arguments result = new();
			 
			if (args.Length >= 2)
			{
				// Check 1st argument : <assembly name> || /list:listing.txt
				string arg1 = args[0];
				if (arg1.StartsWith("/list:"))
				{
					string[] parts = arg1.Split(':');
					string listingPath = string.Join("", parts.Skip(1));
					if (!File.Exists(listingPath))
					{
						//TBD: err
						return null;
					}
				}
				else
				{
					if (!File.Exists(arg1))
					{
						//TBD: err
						return null;
					}

					throw new NotImplementedException();					
				}
			}
			else
			{
				return null;
			}

			return result;
			*/
		}
	}
}

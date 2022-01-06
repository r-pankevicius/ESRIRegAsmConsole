using System;
using System.IO;
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
			if (args is null)
				throw new ArgumentNullException(nameof(args));

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
		}
	}
}

namespace ESRIRegAsmConsole
{
	/// <summary>
	/// Program arguments in a readable form.
	/// </summary>
	internal class Arguments
	{
		/// <summary>
		/// True = register, false = unregister.
		/// </summary>
		public bool Register { get; set; }

		/// <summary>
		/// Export to registry file (file name or glob).
		/// </summary>
		public string ExportToRegistryFile { get; set; }

		/// <summary>
		/// Category mapping file (.xml or .reg)
		/// </summary>
		public string CategoryMappingFile { get; set; }

		/// <summary>
		/// Keep working files directory.
		/// </summary>
		public string KeepWorkingFilesDirectory { get; set; }

		/// <summary>
		/// If the program output should be verbose.
		/// </summary>
		public bool VerboseOutput{ get; set; }
	}
}

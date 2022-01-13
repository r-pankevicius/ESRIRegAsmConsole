namespace ESRIRegAsmConsole
{
	/// <summary>
	/// Product argument: Desktop or Server.
	/// </summary>
	internal enum Product
	{
		Desktop,
		Server
	}

	/// <summary>
	/// Program arguments in a readable form.
	/// </summary>
	internal class Arguments
	{
		/// <summary>
		/// Required first argument: DLL or listing file.
		/// </summary>
		public string PathToAssemblyOrListingFile { get; set; }

		/// <summary>
		/// true: <see cref="PathToAssemblyOrListingFile"/> is DLL otherwise: listing file.
		/// </summary>
		public bool IsDll { get; set; }

		/// <summary>
		/// Folder to resolve relative paths from.
		/// </summary>
		public string BasePath { get; set; }

		/// <summary>
		/// True = register, false = unregister.
		/// </summary>
		public bool Register { get; set; } = true;

		/// <summary>
		/// Product argument: Desktop or Server.
		/// </summary>
		public Product Product { get; set; } = Product.Desktop;

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
		/// Continue execution even if registration of some file fails.
		/// </summary>
		public bool ContinueOnFail { get; set; } = false;

		/// <summary>
		/// If the program output should be verbose.
		/// </summary>
		public bool VerboseOutput { get; set; } = false;
	}
}

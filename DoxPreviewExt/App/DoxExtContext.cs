namespace ExtensionCommon
{
	public static class ExtensionContext
	{
		public const string ConstExtensionName = @"DoxPreviewExt";
		public const string ConstExtensionImgResName = ExtensionContext.ConstExtensionName + @".DoxUtil.Resources.";
		public const string ConstOptionsCategoryName = @"Doxygen Extentions";
		
		public const string constDoxygenSourceBrowserURL = @"";

		public const string doxCommadsHelpURL_ = @"https://www.stack.nl/~dimitri/doxygen/manual/commands.html";
		public const string graphvizHelpURL_   = @"http://www.graphviz.org/";
		public const string mscgenHelpURL_     = @"http://www.mcternan.me.uk/mscgen/";
		public const string planUMLHelpURL_    = @"http://plantuml.com/";
		public const string latexHelpURL_      = @"https://en.wikibooks.org/wiki/LaTeX/Mathematics";

		public const string ConstDoxygenExePath = @"C:/tools/doxygen/doxygen.exe";
		
		/// \todo search paths
		public const string ConstDotExe         = @"C:/tools/graphviz/bin/dot.exe";
		public const string ConstMscGenExe      = @"C:/tools/doxygen/mscgen_0_20/mscgen.exe";
		public const string ConstPlantUmlJar    = @"C:/tools/PlantUML/plantuml.jar";
		public const string ConstMimeTeXExe     = @"C:/tools/mimetex/mimetex.exe";
		public const string doxRefAddImage_ = @"secEliteExtensionsDocCodeAddImage";

		public static string DoxygenPreviewTemplatePath { get; set; } = @"C:/source/model3dviewer/OpenGL/DlOGL/doxygen_dox_preview.txt";
		//public static string DoxygenPreviewTemplatePath { get; set; } = @"";

		// Doxgen Options and Settings

		public const string ConstDoxOptionsActionSettingsName = @"Display and action settings";
		public const string ConstDoxOptionsConfigurationName = @"Doxygen configuration";
	}
}

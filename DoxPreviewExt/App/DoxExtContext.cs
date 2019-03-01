using System.Collections.Generic;
using System.IO;


namespace ExtensionCommon
{
	public static class ExtensionContext
	{
		public const string ConstExtensionName = @"DoxPreviewExt";
		public const string ConstExtensionImgResName = ExtensionContext.ConstExtensionName + @".DoxUtil.Resources.";
		public const string ConstOptionsCategoryName = @"Doxygen Preview";
		
		public const string constDoxygenSourceBrowserURL = @"";

        public const string constDoxygenHomeURL_       = @"http://www.doxygen.nl";
        public const string doxManualCommadsHelpURL_   = @"/manual/commands.html";
        public const string doxManualDocblocksHelpURL_ = @"/manual/docblocks.html";
        public const string graphvizHelpURL_           = @"http://www.graphviz.org/";
		public const string mscgenHelpURL_             = @"http://www.mcternan.me.uk/mscgen/";
		public const string planUMLHelpURL_            = @"http://plantuml.com/";
		public const string latexHelpURL_              = @"https://en.wikibooks.org/wiki/LaTeX/Mathematics";

		public const string ConstDoxygenExePath = @"C:/tools/doxygen/doxygen.exe";
		
		public static string DoxygenPreviewTemplatePath { get; set; } = @"C:/source/model3dviewer/OpenGL/DlOGL/doxygen_dox_preview.txt";
		//public static string DoxygenPreviewTemplatePath { get; set; } = @"";

		// Doxgen Options and Settings

		public const string ConstDoxOptionsActionSettingsName = @"Display and action settings";
		public const string ConstDoxOptionsConfigurationName = @"Doxygen configuration";
        public const string ConstDoxOptionsDocumentCodeName = @"Code documentation settings";


        private static List<string> _pathList = new List<string>
		{
			@"C:\Tools",
			@"C:\Program Files",
			@"C:\Program Files (x86)",
		};

		private static string SearchFile( string name )
		{
			foreach( var path in _pathList)
			{
				var filePath = path;
				if (filePath.EndsWith("/") == false && filePath.EndsWith("\\") == false)
					filePath += "/";
				filePath += name;
				if (File.Exists(filePath))
					return filePath;
			}
			return "";
		}

		public static string GetDotExe()
		{
			return SearchFile(@"graphviz/bin/dot.exe");
		}

		public static string GetMscGenExe()
		{
			return SearchFile(@"mscgen/bin/mscgen.exe");
		}

		public static string GetPlantUmlJar()
		{
			return SearchFile(@"plantuml/plantuml.jar");
		}

		public static string GetMimeTeXExe()
		{
			return SearchFile(@"mimetex/mimetex.exe");
		}
	}
}

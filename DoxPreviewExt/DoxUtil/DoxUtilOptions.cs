using System.Collections.Generic;
using System.IO;


namespace DoxPreviewExt.DoxUtil
{
	public class COptions 
	{
		public Microsoft.VisualStudio.Shell.Package Package { get; }

		public static string DefaultSourceBrowser { get; set; } = ExtensionCommon.ExtensionContext.constDoxygenSourceBrowserURL;
		public static string DefaultConfigFile { get; set; } = ExtensionCommon.ExtensionContext.DoxygenPreviewTemplatePath;

		private string _currentSourceBrowser = "";
		private string _currentDoxygenConfigFile = "";

		public string CurrentSourceBrowser { get { return _currentSourceBrowser; } }
		public string CurrentDoxygenConfigFile { get { return _currentDoxygenConfigFile; } }

		private SortedDictionary<string, List<string>> config_ = new SortedDictionary<string, List<string>>();
		private List<string> imagePaths_ = new List<string>();
		private List<string> dotPaths_ = new List<string>();
		private List<string> mscPaths_ = new List<string>();
		private List<string> diaPaths_ = new List<string>();
		private List<string> umlPaths_ = new List<string>();

		private T_PAGE Page<T_PAGE>() where T_PAGE : class
		{
			T_PAGE page = this.Package.GetDialogPage(typeof(T_PAGE)) as T_PAGE;
			return page;
		}

		/// \brief Reference doxygen documentation on server 
		public bool UseSourceBrowser { get { return Page<Configuration.OptionPageDoxygenConfiguration>().UseSourceBrowser; } }

		/// \brief Interpret doxygen configuration file
		public bool UseConfigFile { get { return Page<Configuration.OptionPageDoxygenConfiguration>().UseConfigFile; } }
		
		/// \brief Get Path of mimeTeX Latex Formula to bitmap converter
		public string MimeTexPath { get { return Page<Configuration.OptionPageDoxygenConfiguration>().MimeTeXPath; } }

		/// \brief Browse Reference, if clicked on
		public bool ClickOnRef { get { return AnyDocumentationValid() && Page<Configuration.OptionPageActionSettings>().ClickOnRef; } }

		/// \brief Show general Quicktips
		public bool GeneralQT { get { return Page<Configuration.OptionPageActionSettings>().GeneralQicktips; } }

		/// \brief Show reference URL in Quicktip
		public bool RefQT { get { return AnyDocumentationValid() && Page<Configuration.OptionPageActionSettings>().RefQuicktip; } }

		/// \brief Show bitmap preview in Quicktip
		public bool ImageQT { get { return ConfigFileValid() && Page<Configuration.OptionPageActionSettings>().ImageQuicktip; } }

		/// \brief Show Dot Graph preview in Quicktip
		public bool DotQT { get { return ConfigFileValid() && Page<Configuration.OptionPageActionSettings>().DotQuicktip; } }

		/// \brief Show Message Sequence Chart preview in Quicktip
		public bool MscQT	{	get	{	return ConfigFileValid() && Page<Configuration.OptionPageActionSettings>().MscQuicktip; } }

		/// \brief Show Plant UML preview in Quicktip
		public bool PlantUmlQT { get { return ConfigFileValid() && Page<Configuration.OptionPageActionSettings>().PlantUmlQuicktip; } }

		/// \brief Show Latex Formula  preview in Quicktip
		public bool LatexFormulaQT { get { return MimeTeXValid() && Page<Configuration.OptionPageActionSettings>().LatexFormulaQuicktip; } }

		public List<string> ImagePaths { get { return imagePaths_; } }
		public List<string> DotPaths { get { return dotPaths_; } }
		public List<string> MscPaths { get { return mscPaths_; } }
		public List<string> DiaPaths { get { return diaPaths_; } }
		public List<string> UmlPaths { get { return umlPaths_; } }

		public COptions(Microsoft.VisualStudio.Shell.Package package)
		{
			this.Package = package;
		}

		/// \brief Tests if source browser is valid
		private bool SourceBrowserValid()
		{
			string sourceBrowser = this.CurrentSourceBrowser;
			return this.UseSourceBrowser && sourceBrowser != null && sourceBrowser != "";
		}

		/// \brief Tests if local doxgen documantation is valid
		private bool LocalDocumentationValid()
		{
			string local = LocalHTMLPath;
			return this.UseConfigFile && local != null && local != "";
		}

		/// \brief Test if any documentation valid
		private bool AnyDocumentationValid()
		{
			return SourceBrowserValid() || AnyDocumentationValid();
		}

		/// \brief Tests if configuration file is valid
		private bool ConfigFileValid()
		{
			string configFile = this.CurrentDoxygenConfigFile;
			return this.UseConfigFile && configFile != null && configFile != "";
		}

		/// \brief Tests if mimeTeX convert tool is valid
		private bool MimeTeXValid()
		{
			string mimeTeX = this.MimeTexPath;
			return mimeTeX != null && mimeTeX != "";
		}

		/// \brief read doxygen configuration from file
		public void UpdateConfiguration()
		{
			this._currentSourceBrowser = this.UseSourceBrowser ? DefaultSourceBrowser : "";
			string configFile = this.UseConfigFile ? DefaultConfigFile : "";
			bool configFileChanged = configFile != this._currentDoxygenConfigFile;
			this._currentDoxygenConfigFile = configFile;

			if (configFileChanged )
			  ReadConfiguration();
		}

		/// \brief read doxygen configuration from file
		private void ReadConfiguration()
		{
			config_ = new SortedDictionary<string, List<string>>();
			imagePaths_ = new List<string>();
			dotPaths_ = new List<string>();
			mscPaths_ = new List<string>();
			diaPaths_ = new List<string>();
			umlPaths_ = new List<string>();

			// read configuration file
			var config = new SortedDictionary<string, List<string>>();
			try
			{
				char[] trimCh = { ' ', '\t', '\n', '\r' };
				StreamReader file = new StreamReader(this.CurrentDoxygenConfigFile);
				string line;
				while ((line = file.ReadLine()) != null)
				{
					line = line.Trim(trimCh);
					if (line == "" || line.StartsWith("#"))
						continue;
					int assInx = line.IndexOf("=");
					if (assInx < 0)
						continue;
					var key = line.Substring(0, assInx).Trim(trimCh).ToUpper();
					if (key == "")
						continue;
					var val = line.Substring(assInx + 1).Trim(trimCh);
					if (val == "")
						continue;
					bool multiline = val.EndsWith("\\");
					config[key] = new List<string>();
					if (multiline)
						val = val.Substring(0, val.Length - 1).Trim(trimCh);
					config[key].Add(val);
					while (multiline && (line = file.ReadLine()) != null)
					{
						val = line.Trim(trimCh);
						multiline = val.EndsWith("\\");
						if (multiline)
							val = val.Substring(0, val.Length - 1).Trim(trimCh);
						config[key].Add(val);
					}
				}
				file.Close();
			}
			catch
			{
				return;
			}

			// update configuration
			config_ = config;

			// generate absolute image paths
			imagePaths_ = ConvertPaths("IMAGE_PATH", this.DoxRootPath);

			// generate absolute dot graph paths (graphviz)
			dotPaths_ = ConvertPaths("DOTFILE_DIRS", this.DoxRootPath);

			// generate absolute message sequence chart paths
			mscPaths_ = ConvertPaths("MSCFILE_DIRS", this.DoxRootPath);

			// generate absolute dia paths
			diaPaths_ = ConvertPaths("DIAFILE_DIRS", this.DoxRootPath);

			// generate absolute UML paths
			umlPaths_ = ConvertPaths("PLANTUML_INCLUDE_PATH", this.DoxRootPath);

		}

		/// \brief conver local paths to avsolute paths
		private List<string> ConvertPaths(string key, string rootPath)
		{
			List<string> absPaths = new List<string>();
			if (config_.ContainsKey(key))
			{
				foreach (var listPath in config_[key])
				{
					try
					{
						var localPath = listPath;
						if (localPath == ".")
							localPath = "";
						else if (localPath.StartsWith("./") || localPath.StartsWith(".\\"))
							localPath = localPath.Substring(2);
						var absolutePath = Path.Combine(rootPath, localPath);
						if (Directory.Exists(absolutePath))
							absPaths.Add(absolutePath);
					}
					catch { }
				}
			}
			return absPaths;
		}

		/// \brief test if html output is enabled
		private string ConfigValue(string key)
		{
			try
			{
				return config_[key][0];
			}
			catch
			{
				return "";
			}
		}

		/// \brief get root path of doxygen project
		public string DoxRootPath
		{
			get
			{
				try
				{
					var configFileName = Path.GetFileName(CurrentDoxygenConfigFile);
					var doxRootPath = CurrentDoxygenConfigFile.Substring(0, CurrentDoxygenConfigFile.Length - configFileName.Length);
					return doxRootPath;
				}
				catch
				{
					return "";
				}
			}
		}

		/// \brief test if html output is enabled
		public bool DoxHtmlOutput { get { return ConfigValue("GENERATE_HTML").ToUpper() == "YES"; } }

		/// \brief get local HTML diectory
		public string LocalHTMLPath
		{
			// TODO $$$ HTML_FILE_EXTENSION    = .html
			get
			{
				try
				{
					var path = this.DoxRootPath;
					if (path == null || path == "" || this.DoxHtmlOutput == false)
						return "";
					var outDir = config_["OUTPUT_DIRECTORY"][0];
					var htmlDir = config_["HTML_OUTPUT"][0];
					if (outDir == null || outDir == "" || htmlDir == null || htmlDir == "")
						return null;
					path = path + outDir + "/" + htmlDir + "/";
					return path;
				}
				catch
				{
					return "";
				}
			}
		}

		/// \brief test if mathjax is enabled (Latex formula)
		public bool MathJaxSuport{ get { return ConfigValue("USE_MATHJAX").ToUpper() == "YES"; } }

		/// \brief test if dot graph is enabled
		public bool DotGraphSuport { get { return ConfigValue("HAVE_DOT").ToUpper() == "YES"; } }

		/// \brief get mathjax URL
		public string MathJaxPath { get { return ConfigValue("MATHJAX_RELPATH"); } }

		/// \brief get message sequence chart binaries path
		public string MscGeneratorBinPath { get { return ConfigValue("MSCGEN_PATH"); } }

		/// \brief get dot graph binaries path (graphviz)
		public string DotGraphBinPath { get { return ConfigValue("DOT_PATH"); } }

		/// \brief get message sequence chart binaries path (mscgen)
		public string MscBinPath { get { return ConfigValue("MSCGEN_PATH"); } }

		/// \brief get dia binaries path
		public string DiaBinPath { get { return ConfigValue("DIA_PATH"); } }

		/// \brief get path of plantuml.jar (Plant UML)
		public string PlantUmlJarPath { get { return ConfigValue("PLANTUML_JAR_PATH"); } }
		/// \brief get path and file name of plantuml.jar (Plant UML)
		public string PlantUmlJarFile
		{
			get
			{
				var path = ConfigValue("PLANTUML_JAR_PATH");
				if (path == null || path == "")
					return "";
				if (path.EndsWith("\\") == false && path.EndsWith("/") == false)
					path += "\\";
				var file = path + "plantuml.jar";
				return file;
			}
		}
	}
}

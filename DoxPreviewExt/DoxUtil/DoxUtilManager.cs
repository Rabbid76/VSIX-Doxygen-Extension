using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Windows.Media;
using Microsoft.Win32;
using EnvDTE80;
using Svg;
using ExtensionCommon;


namespace DoxPreviewExt.DoxUtil
{
	public interface IPreviewImage
	{
		string GetImageFile();
		System.Windows.Controls.Image GetImage();
	}

	public class CPreviewImageCollection
	{
		private SortedDictionary<string, IPreviewImage> tempPreviewFiles_ = null;
		private int nextTempPreviewFileNumber_ = 0;

		public CPreviewImageCollection() { }

		public IPreviewImage Find(string key)
		{
			if (tempPreviewFiles_ == null || tempPreviewFiles_.ContainsKey(key) == false)
				return null;
			return tempPreviewFiles_[key];
		}

		public void Remove(string key)
		{
			if (tempPreviewFiles_ != null)
				tempPreviewFiles_.Remove(key);
		}
		public void RemoveImageFile(string filePath)
		{
			if (filePath == null || filePath == "" || tempPreviewFiles_ == null)
				return;
			// clear file from cache if it exists already
			var file = filePath.ToLower();
			foreach (var dicFile in tempPreviewFiles_)
			{
				if (dicFile.Value.GetImageFile().ToLower() == file)
				{
					this.tempPreviewFiles_.Remove(file);
					break;
				}
			}
		}
		public void Add(string key, IPreviewImage image)
		{
			RemoveImageFile(image.GetImageFile());
			if (this.tempPreviewFiles_ == null)
				this.tempPreviewFiles_ = new SortedDictionary<string, IPreviewImage>();
			this.tempPreviewFiles_[key] = image;
		}

		/// \brief Get new name of temporary preview file name
		public string NewTempPreviewFileName()
		{
			var tempName = "$tmp_preview_" + this.nextTempPreviewFileNumber_.ToString();
			this.nextTempPreviewFileNumber_++;
			if (this.nextTempPreviewFileNumber_ >= 100)
				this.nextTempPreviewFileNumber_ = 0;
			return tempName;
		}
	}

	public class CCache
	{
		public CLinkMap mainLinkMap_ = null;  //! \brief main reference association map
		public CLinkMap localLinkMap_ = null; //! \brief alternative or loacal reference association map
		public CFileMap imageFileMap_ = null; //! \brief map image file names to image file paths
		public CFileMap dotFileMap_ = null;   //! \brief map dot graph file names to dot file paths (graphviz)
		public CFileMap mscFileMap_ = null;   //! \brief map message sequence chart file names to message sequence chart paths
		public CFileMap diaFileMap_ = null;   //! \brief map dia file names to dia paths
	}

	public class CManager
	{
		public static string DoxCommadsHelpURL { get { return ExtensionContext.doxCommadsHelpURL_; } }
		public static string GraphvizHelpURL { get { return ExtensionContext.graphvizHelpURL_; } }
		public static string MscgenHelpURL { get { return ExtensionContext.mscgenHelpURL_; } }
		public static string PlanUMLHelpURL { get { return ExtensionContext.planUMLHelpURL_; } }
		public static string LatexHelpURL { get { return ExtensionContext.latexHelpURL_; } }
		public static string DoxRefAddImage { get { return ExtensionContext.doxRefAddImage_; } }

		public static string DoxExtTempPath
		{
			get
			{
				var tempPath = Path.GetTempPath() + "doxExt";
				Directory.CreateDirectory(tempPath);
				return tempPath;
			}
		}

		//! List of a well known doxygen special command
		//! cf [https://www.stack.nl/~dimitri/doxygen/manual/commands.html] 
		private static HashSet<string> commandIDs_ = new HashSet<string>
		{
			"a",
			"addindex",
			"addtogroup",
			"anchor",
			"arg",
			"attention",
			"author",
			"authors",
			"b",
			"brief",
			"bug",
			"c",
			"callergraph",
			"callgraph",
			"category",
			"cite",
			"class",
			"code",
			"cond",
			"copybrief",
			"copydetails",
			"copydoc",
			"copyright",
			"date",
			"def",
			"defgroup",
			"deprecated",
			"details",
			"diafile",
			"dir",
			"docbookonly",
			"dontinclude",
			"dot",
			"dotfile",
			"e",
			"else",
			"elseif",
			"em",
			"endcode",
			"endcond",
			"enddocbookonly",
			"enddot",
			"endhtmlonly",
			"endif",
			"endinternal",
			"endlatexonly",
			"endlink",
			"endmanonly",
			"endmsc",
			"endparblock",
			"endrtfonly",
			"endsecreflist",
			"endverbatim",
			"enduml",
			"endxmlonly",
			"enum",
			"example",
			"exception",
			"extends",
			"f$",
			"f[",
			"f]",
			"f{",
			"f}",
			"file",
			"fn",
			"headerfile",
			"hidecallergraph",
			"hidecallgraph",
			"hideinitializer",
			"htmlinclude",
			"htmlonly",
			"idlexcept",
			"if",
			"ifnot",
			"image",
			"implements",
			"include",
			"includedoc",
			"includelineno",
			"ingroup",
			"internal",
			"invariant",
			"interface",
			"latexinclude",
			"latexonly",
			"li",
			"line",
			"link",
			"mainpage",
			"manonly",
			"memberof",
			"msc",
			"mscfile",
			"n",
			"name",
			"namespace",
			"nosubgrouping",
			"note",
			"overload",
			"p",
			"package",
			"page",
			"par",
			"paragraph",
			"param",
			"parblock",
			"post",
			"pre",
			"private",
			"privatesection",
			"property",
			"protected",
			"protectedsection",
			"protocol",
			"public",
			"publicsection",
			"pure",
			"ref",
			"refitem",
			"related",
			"relates",
			"relatedalso",
			"relatesalso",
			"remark",
			"remarks",
			"result",
			"return",
			"returns",
			"retval",
			"rtfonly",
			"sa",
			"secreflist",
			"section",
			"see",
			"short",
			"showinitializer",
			"since",
			"skip",
			"skipline",
			"snippet",
			"snippetdoc",
			"snippetlineno",
			"startuml",
			"struct",
			"subpage",
			"subsection",
			"subsubsection",
			"tableofcontents",
			"test",
			"throw",
			"throws",
			"todo",
			"tparam",
			"typedef",
			"union",
			"until",
			"var",
			"verbatim",
			"verbinclude",
			"version",
			"vhdlflow",
			"warning",
			"weakgroup",
			"xmlonly",
			"xrefitem"
		};

		private static CManager _doxManger = null; //!< doxgen utility manager singleton
		private Microsoft.VisualStudio.Shell.Package _package = null; //!< shell package
		private DTE2 _applicationObject = null; //!< application object
		private COptions _options = null; //!< doxygen options
		private string _solutionFile = null; //!< current solution
		private CCache _cache = null;   //!< cache data
		private CPreviewImageCollection tempPreviewFiles_ = new CPreviewImageCollection();

		public Microsoft.VisualStudio.Shell.Package Package { get { return _package; } }

		public DTE2 ApplicationObject	{	get	{	return _applicationObject; } }

		public COptions Options	{	get	{	return this._options;	} }

		public string SolutionFile { get { return _solutionFile; } }

		private CCache Cache
		{
			get
			{
				if (_cache == null)
					_cache = new CCache();
				return _cache;
			}
		}

		public static CManager Manager
		{
			get
			{
				_doxManger.Update();
				return _doxManger;
			}
		}

		//! \brief ctor
		public CManager(Microsoft.VisualStudio.Shell.Package package, DTE2 applicationObject)
		{
			_doxManger = this;
			this._package = package;
			this._applicationObject = applicationObject;
			this._options = new COptions(package);
		}

		public void Update()
		{
			if (this._options == null)
				this._options = new COptions(Package);


			// read and interprets current solution form application obejct
			UpdateConfigFile();

			// update options if doxygen configuration file has changed 
			this._options.UpdateConfiguration();

			// update cache if doxygen source has changed
			this.UpdateURL(this._options.CurrentSourceBrowser);
			this.UpdateLocalHtml(this.Options.LocalHTMLPath);
			this.UpdateFilePaths(ref Cache.imageFileMap_, Options.ImagePaths);
			this.UpdateFilePaths(ref Cache.dotFileMap_, Options.DotPaths);
			this.UpdateFilePaths(ref Cache.mscFileMap_, Options.MscPaths);
			this.UpdateFilePaths(ref Cache.diaFileMap_, Options.DiaPaths);
		}

		/// \brief reads and interprets current solution form application obejct
		private void UpdateConfigFile()
		{
			var slnFile = "";
			var solution = this.ApplicationObject.Solution;
			if (solution != null)
				slnFile = solution.FileName;

			var doxPreviewTempl = ExtensionContext.DoxygenPreviewTemplatePath;

			// TODO $$$ read global doxygen settings

			bool slnChanged = _solutionFile == null || _solutionFile != slnFile;
			_solutionFile = slnFile;
			if (slnChanged == false)
				return;

			var slnFileName = Path.GetFileName(SolutionFile);
			var slnRootPath = SolutionFile.Substring(0, SolutionFile.Length - slnFileName.Length);

			// TODO $$$ read doxygen configuration from solution project

			// find 'doxygen_config.xml' file
			var doxConfigXMLName = "doxygen_config.xml";
			var doxConfigXMLPath = slnRootPath;
			var doxConfigXML = doxConfigXMLPath + doxConfigXMLName;
			if (File.Exists(doxConfigXML) == false)
			{
				doxConfigXMLPath = "C:\\source\\model3dviewer\\VSDoxExt\\test\\"; // TODO $$$ test only
				doxConfigXML = doxConfigXMLPath + doxConfigXMLName;
			}

			// read doxgan configuration paths from 'doxygen_config.xml'
			CDoxSrcConfigXML xConfig = new CDoxSrcConfigXML(doxConfigXML);
			if (xConfig.Read())
			{
				var sourceBrowser = xConfig.GetSource();
				COptions.DefaultSourceBrowser = sourceBrowser;

				var configFileRel = xConfig.GetConfigFile();
				var configFileAbs = Path.Combine(doxConfigXMLPath, configFileRel);
				COptions.DefaultConfigFile = configFileAbs;

				return;
			}

			// try to find find elitecad doxygen project
			var doxTestFile = slnRootPath + "/dox_src/Doxyfile.preview";
			if (File.Exists(doxTestFile))
			{
				COptions.DefaultSourceBrowser = ExtensionCommon.ExtensionContext.constDoxygenSourceBrowserURL;
				COptions.DefaultConfigFile = doxTestFile;
			}

			// ...
		}

		private static string defaultBrowser = null;
		/// \brief get the standard browser path
		/// [http://stackoverflow.com/questions/13621467/how-to-find-default-web-browser-using-c]
		internal string GetShellDefaultBrowser()
		{
			if (defaultBrowser != null)
				return defaultBrowser;

			string name = string.Empty;
			RegistryKey regKey = null;

			try
			{
				regKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\Shell\\Associations\\UrlAssociations\\http\\UserChoice", false);
				if (regKey == null)
					regKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\Shell\\Associations\\UrlAssociations\\http\\UserChoice", false);
				if (regKey != null)
				{
					var progIdVal = regKey.GetValue("ProgId");
					var progId = progIdVal != null ? progIdVal.ToString() : "";
					regKey.Close();
					regKey = Registry.ClassesRoot.OpenSubKey(progId + "\\shell\\open\\command");
				}
				if (regKey == null) 
				  regKey = Registry.ClassesRoot.OpenSubKey("HTTP\\shell\\open\\command", false);

				//get rid of the enclosing quotes
				name = regKey.GetValue(null).ToString().ToLower().Replace("" + (char)34, "");

				if (!name.EndsWith("exe"))
					name = name.Substring(0, name.LastIndexOf(".exe") + 4);
			}
			catch (Exception ex)
			{
				name = string.Format("ERROR: An exception of type: {0} occurred in method: {1} in the following module: {2}", ex.GetType(), ex.TargetSite, this.GetType());
			}
			finally
			{
				if (regKey != null)
					regKey.Close();
			}

			defaultBrowser = name;
			return defaultBrowser;
		}

		/// \brief open content in standard browser 
		public static void OpenHTML(string path)
		{
			var cmdURL = path;
			try
			{
				DoxUtil.CManager manager = DoxUtil.CManager.Manager;
				if (manager == null)
					return;

				//DTE2 appObj = manager.ApplcationObject()

				//if (appObj != null)
				//{
				//var win = appObj.OpenFile(cmdURL, EnvDTE.vsViewKindPrimary);
				//var win = appObj.OpenFile(cmdURL, "{ 00000000 - 0000 - 0000 - 0000 - 000000000000}");
				//appObj.Navigate(cmdURL)
				//}

				var browser = manager.GetShellDefaultBrowser();
				if (browser != null && browser != "")
				{
					Process.Start(browser, cmdURL);
					return;
				}

				if (cmdURL.Length > 1 && cmdURL[1] == ':')
					cmdURL = @"file:///" + cmdURL;
				Process.Start(cmdURL);
			}
			catch { }
		}

		//! Tests if the command is a well known doxygen special command
		//! cf [https://www.stack.nl/~dimitri/doxygen/manual/commands.html] 
		public bool IsKnownCommand(string cmdStr)
		{
			bool hasCmdTag = cmdStr.StartsWith("@") || cmdStr.StartsWith("\\");
			string cmdName = cmdStr.Substring(hasCmdTag ? 1 : 0);
			return commandIDs_.Contains(cmdName);
		}

		//! \brief update URL and regenerate reference association map
		private static void UpdateURL(ref CLinkMap linkMap, string mainUrl)
		{
			if (linkMap == null)
				linkMap = new CLinkMap(mainUrl);
			else if (mainUrl != linkMap.RootURL)
				linkMap.UpdateLinkMap(mainUrl);
		}

		//! \brief update main URL and regenerate reference association map
		public void UpdateURL(string mainUrl)
		{
			UpdateURL(ref this.Cache.mainLinkMap_, mainUrl);
		}

		//! \brief update local path and regenerate reference association map
		public void UpdateLocalHtml(string localPath)
		{
			UpdateURL(ref this.Cache.localLinkMap_, localPath);
		}

		//! \brief update image path and regenerate image location map
		private void UpdateFilePaths(ref CFileMap filemap, List<string> imagePaths)
		{
			if (filemap == null)
				filemap = new CFileMap(imagePaths);
			else if (imagePaths != filemap.RootPaths)
				filemap.UpdateFileMap(imagePaths);
		}

		//! \brief clear doxygen cache
		public void ClearDoxygenCache()
		{
			UpdateURL("");
			UpdateLocalHtml("");
			UpdateFilePaths(ref Cache.imageFileMap_, new List<string>());
			UpdateFilePaths(ref Cache.dotFileMap_,   new List<string>());
			UpdateFilePaths(ref Cache.mscFileMap_,   new List<string>());
			UpdateFilePaths(ref Cache.diaFileMap_,   new List<string>());
		}

		//! \brief find HTML link from reference name
		public string FindDoxygenLink(string doxRef, bool recreateIfEmpty)
		{
			string url = this.Cache.mainLinkMap_ != null && this.Options.UseSourceBrowser ? this.Cache.mainLinkMap_.FindDoxygenLink(doxRef, recreateIfEmpty) : "";
			if (url == "")
				url = this.Cache.localLinkMap_ != null && this.Options.UseConfigFile ? this.Cache.localLinkMap_.FindDoxygenLink(doxRef, recreateIfEmpty) : "";
			if (url.Length > 2 && url[1] == ':')
				url = @"file:///" + url;
			return url;
		}

		//! \brief parse text and find the first doxgen reference
		public string FindFirstDoxygenReference(string text)
		{
			int refPos = text.IndexOf("\\ref");
			if (refPos < 0)
				refPos = text.IndexOf("@ref");
			if (refPos < 0)
				return "";

			string doxRef = text.Substring(refPos + 4).Trim();
			int doxEnd = doxRef.IndexOf(" ");
			if (doxEnd < 0)
				doxEnd = doxRef.IndexOf("\t");
			if (doxEnd >= 0)
				doxRef = doxRef.Substring(0, doxEnd).Trim();

			return doxRef;
		}

		//! \brief find the first doxygen reference and find the associated HTML link
		public string FindFirstDoxygenLink(string text, bool recreateIfEmpty)
		{
			string doxRef = this.FindFirstDoxygenReference(text);
			if (doxRef == "")
				return "";

			string url = this.Cache.mainLinkMap_ != null ? this.Cache.mainLinkMap_.FindDoxygenLink(doxRef, recreateIfEmpty) : "";
			if (url == "")
				url = this.Cache.localLinkMap_ != null ? this.Cache.localLinkMap_.FindDoxygenLink(doxRef, recreateIfEmpty) : "";

			return url;
		}

		//! \brief make file key from file name
		private string FileKeyFromName(string fileName)
		{
			string fileKey = fileName;
			if (fileKey.StartsWith("\"") || fileKey.StartsWith("'"))
				fileKey = fileKey.Substring(1);
			if (fileKey.EndsWith("\"") || fileKey.EndsWith("'"))
				fileKey = fileKey.Substring(0, fileKey.Length - 1);
			bool slashcut;
			do
			{
				int findInx = fileKey.IndexOf('/');
				if (findInx < 0)
					findInx = fileKey.IndexOf('\\');
				slashcut = findInx >= 0;
				if (slashcut)
					fileKey = fileKey.Substring(findInx + 1);
			}
			while (slashcut);
			return fileKey;
		}

		//! \brief find a file path by a image file name
		public string FindImageFilePath(string filename)
		{
			string fileKey = FileKeyFromName(filename);
			return this.Cache.imageFileMap_ != null ? this.Cache.imageFileMap_.FindFilePath(fileKey) : "";
		}

		//! \brief find a file path by a dot graph file name
		public string FindDotFilePath(string filename)
		{
			string fileKey = FileKeyFromName(filename);
			return this.Cache.dotFileMap_ != null ? this.Cache.dotFileMap_.FindFilePath(fileKey) : "";
		}

		//! \brief find a file path by a message sequence chart file name
		public string FindMscFilePath(string filename)
		{
			string fileKey = FileKeyFromName(filename);
			return this.Cache.mscFileMap_ != null ? this.Cache.mscFileMap_.FindFilePath(fileKey) : "";
		}

		//! \brief find a file path by a dia file name
		public string FindDiaFilePath(string filename)
		{
			string fileKey = FileKeyFromName(filename);
			return this.Cache.diaFileMap_ != null ? this.Cache.diaFileMap_.FindFilePath(fileKey) : "";
		}

		/// \brief laod image form file
		public static System.Windows.Controls.Image LoadOrCreateBitmapImage(string imagePath)
		{
			string imagePathToLower = imagePath.ToLower();
			if (imagePathToLower.EndsWith("jpg") || imagePathToLower.EndsWith("bmp") || imagePathToLower.EndsWith("png") || imagePathToLower.EndsWith("gif") || imagePathToLower.EndsWith("tif") || imagePathToLower.EndsWith("tiff"))
			{
				try
				{
					var img = new System.Windows.Controls.Image();
					var bitmap = new System.Windows.Media.Imaging.BitmapImage();
					bitmap.BeginInit();
					bitmap.UriSource = new Uri(imagePath, UriKind.RelativeOrAbsolute);
					bitmap.EndInit();
					img.Stretch = Stretch.Uniform;
					img.Source = bitmap;
					return img;
				}
				catch { }
				// <img src="refl1_non.jpg" alt="refl1_non.jpg"/>
			}
			else if (imagePathToLower.EndsWith("svg"))
			{
				try
				{
					// https://lasithapetthawadu.wordpress.com/2014/02/24/using-vector-svg-graphics-in-c-net/
					int maxHeight = 1024;
					SvgDocument document = SvgDocument.Open(imagePath);
					if (document.Height > maxHeight)
					{
						document.Width = (int)((document.Width / (double)document.Height) * maxHeight);
						document.Height = maxHeight;
					}
					System.Drawing.Bitmap bmp = document.Draw();

					var bmpCapture = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
						bmp.GetHbitmap(),
						IntPtr.Zero,
						System.Windows.Int32Rect.Empty,
						System.Windows.Media.Imaging.BitmapSizeOptions.FromWidthAndHeight(bmp.Width, bmp.Height));

					var img = new System.Windows.Controls.Image();
					img.Source = bmpCapture;
					return img;
				}
				catch { }
			}
			return null;
		}

		//! \brief Generate MD5 hash from UTF8 string 
		// MD5 Class [https://msdn.microsoft.com/en-us/library/system.security.cryptography.md5%28v=vs.110%29.aspx]
		static string GetMd5Hash(MD5 md5Hash, string input)
		{
			// Convert the input string to a byte array and compute the hash.
			byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

			// Create a new Stringbuilder to collect the bytes
			// and create a string.
			StringBuilder sBuilder = new StringBuilder();

			// Loop through each byte of the hashed data 
			// and format each one as a hexadecimal string.
			for (int i = 0; i < data.Length; i++)
			{
				sBuilder.Append(data[i].ToString("x2"));
			}

			// Return the hexadecimal string.
			return sBuilder.ToString();
		}

		/// \brief generates MD5 key from source code
		public static string GenerateMD5Key( string sourceCode )
		{
			var src = sourceCode;
			// TODO $$$ remove multiple blank \t, \n \r from source code
			var md5hash = GetMd5Hash(MD5.Create(), src);
			return md5hash;
		}
		
		/// \brief generate temporary image file from dot graph source code
		public System.Windows.Controls.Image GenerateDotGraphPreview(string sourceCode)
		{
			try
			{
				// calcualte MD5 of source code
				string md5hash = GenerateMD5Key(sourceCode);

				// find existiong preview image
				var previewImg = this.tempPreviewFiles_.Find(md5hash);

				// create new preview image
				if (previewImg == null)
				{
					previewImg = new CDotGraphImage(this.tempPreviewFiles_, this.Options, false, sourceCode);
					this.tempPreviewFiles_.Add(md5hash, previewImg);
				}

				// get the image
				var img = previewImg.GetImage();
				return img;
			}
			catch { }
			return null;
		}

		/// \brief generate temporary image file from dot graph file
		public System.Windows.Controls.Image GenerateDotFileGraphPreview(string filename)
		{
			try
			{
				// calcualte MD5 of source code
				string fileText = File.ReadAllText(filename);
				string md5hash = GenerateMD5Key(fileText);

				// find existiong preview image
				var previewImg = this.tempPreviewFiles_.Find(md5hash);

				// create new preview image
				if (previewImg == null)
				{
					previewImg = new CDotGraphImage(this.tempPreviewFiles_, this.Options, true, filename);
					this.tempPreviewFiles_.Add(md5hash, previewImg);
				}

				// get the image
				var img = previewImg.GetImage();
				return img;
			}
			catch { }
			return null;
		}

		/// \brief generate temporary image file from message sequence chart source code
		public System.Windows.Controls.Image GenerateMscGraphPreview(string sourceCode)
		{
			try
			{
				// calcualte MD5 of source code
				string md5hash = GenerateMD5Key(sourceCode);

				// find existiong preview image
				var previewImg = this.tempPreviewFiles_.Find(md5hash);

				// create new preview image
				if (previewImg == null)
				{
					previewImg = new CMscGraphImage(this.tempPreviewFiles_, this.Options, false, sourceCode);
					this.tempPreviewFiles_.Add(md5hash, previewImg);
				}

				// get the image
				var img = previewImg.GetImage();
				return img;
			}
			catch { }
			return null;
		}

		/// \brief generate temporary image file from message sequence chart file
		public System.Windows.Controls.Image GenerateMscFileGraphPreview(string filename)
		{
			try
			{
				// calcualte MD5 of source code
				string fileText = File.ReadAllText(filename);
				string md5hash = GenerateMD5Key(fileText);

				// find existiong preview image
				var previewImg = this.tempPreviewFiles_.Find(md5hash);

				// create new preview image
				if (previewImg == null)
				{
					previewImg = new CMscGraphImage(this.tempPreviewFiles_, this.Options, true, filename);
					this.tempPreviewFiles_.Add(md5hash, previewImg);
				}

				// get the image
				var img = previewImg.GetImage();
				return img;
			}
			catch { }
			return null;
		}

		/// \brief generate temporary image file from PlatUML source code
		public System.Windows.Controls.Image GeneratePlatUMLPreview(string sourceCode)
		{
			try
			{
				// calcualte MD5 of source code
				string md5hash = GenerateMD5Key(sourceCode);

				// find existiong preview image
				var previewImg = this.tempPreviewFiles_.Find(md5hash);

				// create new preview image
				if (previewImg == null)
				{
					previewImg = new CPlantUMLImage(this.tempPreviewFiles_, this.Options, sourceCode);
					this.tempPreviewFiles_.Add(md5hash, previewImg);
				}

				// get the image
				var img = previewImg.GetImage();
				return img;
			}
			catch { }
			return null;
		}

		/// \brief generate temporary image file from PlatUML source code
		public System.Windows.Controls.Image GenerateLatexFormulaPreview(string formula)
		{
			try
			{
				// calcualte MD5 of source code
				string md5hash = GenerateMD5Key(formula);

				// find existiong preview image
				var previewImg = this.tempPreviewFiles_.Find(md5hash);

				// create new preview image
				if (previewImg == null)
				{
					previewImg = new CLatexFormulaImage(this.tempPreviewFiles_, this.Options, false, formula);
					this.tempPreviewFiles_.Add(md5hash, previewImg);
				}

				// get the image
				var img = previewImg.GetImage();
				return img;
			}
			catch { }
			return null;
		}
	}
}

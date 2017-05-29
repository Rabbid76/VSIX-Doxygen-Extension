using System.Collections.Generic;
using System.Linq;
using System.IO;


namespace DoxPreviewExt.DoxUtil
{
	public class CFileMap
	{
		private SortedDictionary<string, string> fileMap_;   //!< map file name to file path
		private List<string>                     rootPaths_; //!< list of root paths on volume 

		//! \brief ctor
		public CFileMap(List<string> rootPath)
		{
			UpdateFileMap(rootPath);
		}

		//! \brief root URL property
		public List<string> RootPaths
		{
			get
			{
				return this.rootPaths_;
			}
		}

		//! \brief regenerate link map
		public void UpdateFileMap(List<string> rootPaths)
		{
			this.rootPaths_  = rootPaths;
			this.fileMap_    = null;
			GenerateFileMap();
		}

		//! \brief if link map does not exist generate link map
		private void GenerateFileMap()
		{
			if (this.rootPaths_.Count == 0)
				return;
			if (this.fileMap_ == null || this.fileMap_.Count() == 0)
			{
				this.fileMap_ = new SortedDictionary<string, string>();
				foreach ( var path in rootPaths_)
				  FindeFilesRecursive(path);
			}
		}

		private void FindeFilesRecursive(string dirName)
		{
			try
			{
				foreach (string filePath in Directory.GetFiles(dirName))
				{
					var subFileName = Path.GetFileName(filePath);
					this.fileMap_[subFileName.ToLower()] = filePath;
				}
				foreach (string subDirName in Directory.GetDirectories(dirName))
				{
					FindeFilesRecursive(subDirName);
				}
			}
			catch (System.Exception) { }
		}
		

		//! \brief find a file path by a image file name
		public string FindFilePath(string filename)
		{
			GenerateFileMap();

			string searchKey = filename.ToLower();
			if (this.fileMap_ != null && this.fileMap_.ContainsKey(searchKey))
			{
				return this.fileMap_[searchKey];
			}
			return "";
		}
	}
}

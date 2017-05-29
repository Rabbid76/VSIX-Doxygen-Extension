using System.Collections.Generic;
using System.Linq;


namespace DoxPreviewExt.DoxUtil
{
	public class CLinkMap
	{
		private SortedDictionary<string, string> doxLinkMap_; //!< map reference name to HTML file
		private string                           rootURL_;     //!< root HTMl page 

		//! \brief ctor
		public CLinkMap(string rootURL)
		{
			UpdateLinkMap(rootURL);
		}

		//! \brief root URL property
		public string RootURL
		{
			get
			{
				return this.rootURL_;
			}
		}

		//! \brief regenerate link map
		public void UpdateLinkMap( string rootURL )
		{
			this.rootURL_    = rootURL;
			this.doxLinkMap_ = null;
			GenerateLinkMap();
		}

		//! \brief if link map does not exist generate link map
		private void GenerateLinkMap()
		{
			if (this.rootURL_ == "")
				return;
			if (this.doxLinkMap_ == null || this.doxLinkMap_.Count() == 0)
			  this.doxLinkMap_ = CHTMLFinder.GetLinksRecursively(rootURL_);
		}

		//! \brief find HTML url from reference name
		public string FindDoxygenLink(string doxRef, bool recreateIfEmpty)
		{
			if (this.doxLinkMap_ == null || (recreateIfEmpty && this.doxLinkMap_.Count() == 0))
			  GenerateLinkMap();

			string searchKey = doxRef.ToLower();
			if (this.doxLinkMap_ != null && this.doxLinkMap_.ContainsKey(searchKey))
			{
				string linkText = doxLinkMap_[searchKey] + "#" + doxRef;
				string rootURL = this.RootURL;
				if (rootURL.Substring(rootURL.Length - 1) != "/" && rootURL.Substring(rootURL.Length - 1) != "\\")
					rootURL += "/";
				return rootURL + linkText;
			}

			return "";
		}
	}
}

using System.Collections.Generic;
using System.Xml;
//using System.Xml.Linq;
using System.IO;


namespace DoxPreviewExt.DoxUtil
{
	public class CDoxSrcConfigXML
	{
		private string _file = ""; //!< xml doxygen source specification file
		private SortedDictionary<string, string> _elemDictionary = null;
		
		public string GetSource()
		{
			var key = "source";
			return _elemDictionary.ContainsKey(key) ? _elemDictionary[key] : "";
		}

		public string GetConfigFile()
		{
			var key = "configurationfile";
			return _elemDictionary.ContainsKey(key) ? _elemDictionary[key] : "";
		}

		/// \brief ctor
		public CDoxSrcConfigXML( string file )
		{
			_file = file;
		}

		/// \brief read doxygen configuration
		public bool Read()
		{
			_elemDictionary = new SortedDictionary<string, string>();

			if (File.Exists(_file) == false)
				return false;

			bool ret = true;
			try
			{
				/*
				XDocument xDoc = XDocument.Load(_file);
				foreach (var xElem in xDoc.Root.DescendantNodes().OfType<XElement>() )
				{
					var xName = xElem.Name;
					var name = xName.ToString().ToLower();
					var content = (xElem.FirstNode as XText).Value;
					if (name != null && name != "" && content != null && content != "")
						_elemDictionary[name] = content;
				}
				*/

				XmlReader xReader = XmlReader.Create(_file);
				XmlDocument xDoc = new XmlDocument();
				xDoc.Load(xReader);
				foreach( XmlNode xMainNode in xDoc.ChildNodes )
				{
					var mainName = xMainNode.Name.ToLower();
					var mainContent = xMainNode.InnerText;
					foreach (XmlNode xNode in xMainNode.ChildNodes )
					{
						var name = xNode.Name.ToLower();
						var content = xNode.InnerText;
						if (name != null && name != "" && content != null && content != "")
							_elemDictionary[name] = content;
					}
				}
			}
			catch
		  {
				ret = false;
			}
			return ret;
		}
	}
}

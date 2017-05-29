using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;


namespace DoxPreviewExt.DoxUtil
{
	static class CHTMLFinder
	{
		public struct THrefItem
		{
			public string Href;
			public string Text;

			public override string ToString()
			{
				return Href + "\n\t" + Text;
			}
		}

		//! \brief Get content of text file
		public static string GetFileText(string filename)
		{
			try
			{
				string text = System.IO.File.ReadAllText(filename);
				return text;
			}
			catch { }
			return "";
		}

		//! \brief Get content of HTMl page
		public static string GetHTMLText(string url)
		{
			try
			{
				HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
				HttpWebResponse response = (HttpWebResponse)request.GetResponse();

				if (response.StatusCode == HttpStatusCode.OK)
				{
					Stream receiveStream = response.GetResponseStream();
					StreamReader readStream = null;

					if (response.CharacterSet == null)
					{
						readStream = new StreamReader(receiveStream);
					}
					else
					{
						readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
					}

					string htmlText = readStream.ReadToEnd();

					response.Close();
					readStream.Close();

					return htmlText;
				}
			}
			catch { }
			return "";
		}

		//! \brief Find all links on HTML page
		public static StringCollection GetHTMLLinks(string filenameOrURL)
		{
			string htmlText = "";

			string filename = filenameOrURL;
			filename = filename.ToLower();
			string testStr = @"file:///";
			if (filename.Substring(0, testStr.Length) == testStr)
				filename = filename.Substring(testStr.Length);
			string testExtStr = @".html";
			if (filename.Substring(filename.Length - testExtStr.Length) != ".html")
			{
				if (filename.Substring(filename.Length - 1) != "/" && filename.Substring(filename.Length - 1) != "\\")
					filename += "/";
				filename += "index.html";
			}
			htmlText = GetFileText(filename);

			if (htmlText == "")
			{
				string url = filenameOrURL;
				htmlText = GetHTMLText(url);
			}

			return GetLinksFromHTML(htmlText);
		}

		//! \brief cf [https://www.dotnetperls.com/scraping-html]
		public static List<THrefItem> FindLinks(string file)
		{
			List<THrefItem> list = new List<THrefItem>();

			// 1.
			// Find all matches in file.
			MatchCollection m1 = Regex.Matches(file, @"(<a.*?>.*?</a>)", RegexOptions.Singleline);

			// 2.
			// Loop over each match.
			foreach (Match m in m1)
			{
				string value = m.Groups[1].Value;
				THrefItem i = new THrefItem();

				// 3.
				// Get href attribute.
				Match m2 = Regex.Match(value, @"href=\""(.*?)\""", RegexOptions.Singleline);
				if (m2.Success)
				{
					i.Href = m2.Groups[1].Value;
				}

				// 4.
				// Remove inner tags from text.
				string t = Regex.Replace(value, @"\s*<.*?>\s*", "", RegexOptions.Singleline);
				i.Text = t;

				list.Add(i);
			}
			return list;
		}

		//! \brief get all links from HTML content \n
		//! cf [http://www.java2s.com/Code/CSharp/Network/GetLinksFromHTML.htm]
		public static StringCollection GetLinksFromHTML(string HtmlContent)
		{
			StringCollection links = new StringCollection();

			MatchCollection AnchorTags = Regex.Matches(HtmlContent.ToLower(), @"(<a.*?>.*?</a>)", RegexOptions.Singleline);

			foreach (Match AnchorTag in AnchorTags)
			{
				string value = AnchorTag.Groups[1].Value;

				Match HrefAttribute = Regex.Match(value, @"href=\""(.*?)\""",
						RegexOptions.Singleline);
				if (HrefAttribute.Success)
				{
					string HrefValue = HrefAttribute.Groups[1].Value;
					if (!links.Contains(HrefValue))
					{
						links.Add(HrefValue);
					}
				}
			}

			return links;
		}

		//! \brief find all links from url 
		public static void FindLinks(string url, string actHtmlName, ref HashSet<string> innerHtmls, ref SortedDictionary<string, string> links)
		{
			// find all links on page
			StringCollection innerLinks = GetHTMLLinks(url);

			// append page key
			if (actHtmlName != "")
			{
				int posExt = actHtmlName.IndexOf(".html");
				string name = actHtmlName.Substring(0, posExt);
				bool upper = false;
				string pageKey = "";
				foreach (char ch in name)
				{
					if (ch == '_')
					{
						upper = true;
						continue;
					}
					pageKey += upper ? ch.ToString().ToUpper() : ch.ToString();
					upper = false;
				}
				string pageKeyName = pageKey.ToLower();
				if (pageKey != "" && !links.ContainsKey(pageKeyName))
				{
					links[pageKeyName] = actHtmlName;
				}
			}

			// for all links on page
			foreach (string value in innerLinks)
			{
				// exclude external li´nks
				char[] exclusionCh = { '/', '\\', ':' };
				if (value.IndexOfAny(exclusionCh) != -1)
					continue;
				int hashPos = value.IndexOf('#');

				// split html name and reference
				string htmlName = hashPos >= 0 ? value.Substring(0, hashPos) : value;
				string refName = hashPos >= 0 ? value.Substring(hashPos + 1) : "";

				// add new html file
				if (htmlName != "" && htmlName.IndexOf(".html") > 0)
				{
					innerHtmls.Add(htmlName);
				}
				else
				{
					htmlName = "";
				}

				// add new reference
				string keyName = refName.ToLower();
				if (keyName != "" && !links.ContainsKey(keyName))
				{
					if (htmlName != "")
					{
						links[keyName] = htmlName;
					}
					else if (actHtmlName != "")
					{
						links[keyName] = actHtmlName;
					}
				}
			}
		}

		//! \brief recursively find all links from url 
		private static void FindLinksRecursively(string url, string htmlFile, ref HashSet<string> readHtmls, ref SortedDictionary<string, string> links)
		{
			string actURL = url;
			if (htmlFile != "")
				actURL += "/" + htmlFile;

			// find all links on page
			StringCollection innerLinks = GetHTMLLinks(actURL);

			// for all links on page
			HashSet<string> innerHtmls = new HashSet<string>();
			FindLinks(actURL, htmlFile, ref innerHtmls, ref links);

			// find new HTMLs
			HashSet<string> newHtmls = new HashSet<string>();
			foreach (string testHtml in innerHtmls)
			{
				if (!readHtmls.Contains(testHtml))
				{
					readHtmls.Add(testHtml);
					newHtmls.Add(testHtml);
				}
			}

			// find liks on new HTMLs
			foreach (string html in newHtmls)
			{
				FindLinksRecursively(url, html, ref readHtmls, ref links);
			}
		}

		//! \find liks on HTML page and related pages
		public static SortedDictionary<string, string> GetLinksRecursively(string url)
		{
			SortedDictionary<string, string> links = new SortedDictionary<string, string>();
			HashSet<string> readHtmls = new HashSet<string>();
			FindLinksRecursively(url, "", ref readHtmls, ref links);
			return links;
		}
	}
}

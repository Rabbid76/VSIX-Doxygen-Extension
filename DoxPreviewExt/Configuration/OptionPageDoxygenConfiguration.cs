using System;
using Microsoft.VisualStudio.Shell;
using System.Runtime.InteropServices;
using System.Windows.Forms;


namespace DoxPreviewExt.Configuration
{
	[Guid("F0E49B98-9DBC-4683-8821-95D6429145CB")]
	class OptionPageDoxygenConfiguration : DialogPage
	{
		private bool _useSourceBrowser = true;
		private bool _useConfigFile = true;
		private string _mimeTeXPath = ExtensionCommon.ExtensionContext.ConstMimeTeXBin;

		public bool UseSourceBrowser
		{
			get { return _useSourceBrowser; }
			set { _useSourceBrowser = value; }
		}

		public bool UseConfigFile
		{
			get { return _useConfigFile; }
			set { _useConfigFile = value; }
		}

		public string MimeTeXPath
		{
			get { return _mimeTeXPath; }
			set { _mimeTeXPath = value; }
		}

		protected override IWin32Window Window
		{
			get
			{
				DoxConfigUserControl page = new DoxConfigUserControl();
				page.optionsPage = this;
				page.Initialize();
				return page;
			}
		}
	}
}

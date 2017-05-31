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
		private string _dotExe = ExtensionCommon.ExtensionContext.GetDotExe();
		private string _mscgenExe = ExtensionCommon.ExtensionContext.GetMscGenExe();
		private string _plantumlJar = ExtensionCommon.ExtensionContext.GetPlantUmlJar();
		private string _mimeTeXExe = ExtensionCommon.ExtensionContext.GetMimeTeXExe();

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

		public string DotExe
		{
			get { return _dotExe; }
			set { _dotExe = value; }
		}

		public string MscGenExe
		{
			get { return _mscgenExe; }
			set { _mscgenExe = value; }
		}

		public string PlantUmlJar
		{
			get { return _plantumlJar; }
			set { _plantumlJar = value; }
		}

		public string MimeTeXExe
		{
			get { return _mimeTeXExe; }
			set { _mimeTeXExe = value; }
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

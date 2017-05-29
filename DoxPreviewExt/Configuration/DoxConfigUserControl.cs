using System;
using System.Windows.Forms;


namespace DoxPreviewExt.Configuration
{
	public partial class DoxConfigUserControl : UserControl
	{
		public DoxConfigUserControl()
		{
			InitializeComponent();
		}

		internal OptionPageDoxygenConfiguration optionsPage;

		public void Initialize()
		{
			// inti source browser settings
			this.checkBoxDoxSource.Checked = this.optionsPage.UseSourceBrowser;
			this.textBoxDoxSourceBrowser.Text = DoxUtil.COptions.DefaultSourceBrowser;

			// init doxygen configuration file settings
			this.checkBoxConfigFile.Checked = this.optionsPage.UseConfigFile;
			this.textBoxDoxConfigFile.Text = DoxUtil.COptions.DefaultConfigFile;
			
			// init doxygen tools settings
			this.textBoxMimeTexPath.Text = this.optionsPage.MimeTeXPath;
		}

		private void checkBoxDoxSource_Click(object sender, EventArgs e)
		{
			this.optionsPage.UseSourceBrowser = this.checkBoxDoxSource.Checked;
		}

		private void checkBoxConfigFile_Click(object sender, EventArgs e)
		{
			this.optionsPage.UseConfigFile = this.checkBoxConfigFile.Checked;
		}

		private void textBoxMimeTexPath_Leave(object sender, EventArgs e)
		{
			this.optionsPage.MimeTeXPath = this.textBoxMimeTexPath.Text;
		}

		private void DoxConfigUserControl_Click(object sender, EventArgs e)
		{
			DoxUtil.CManager manager = DoxUtil.CManager.Manager;
			if (manager != null)
			{
				manager.ClearDoxygenCache();
				manager.Update();
			}
		}
	}
}

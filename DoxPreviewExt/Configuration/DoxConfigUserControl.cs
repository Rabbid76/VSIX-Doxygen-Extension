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
		}

		private void DoxConfigUserControl_Load(object sender, EventArgs e)
		{
			// init source browser settings
			this.checkBoxDoxSource.Checked = this.optionsPage.UseSourceBrowser;
			this.textBoxDoxSourceBrowser.Text = DoxUtil.COptions.DefaultSourceBrowser;

			// init doxygen configuration file settings
			this.checkBoxConfigFile.Checked = this.optionsPage.UseConfigFile;
			this.textBoxDoxConfigFile.Text = DoxUtil.COptions.DefaultConfigFile;

			// init doxygen tools settings

			string dotExe = this.optionsPage.DotExe;
			if (dotExe == null || dotExe == "")
			{
				dotExe = ExtensionCommon.ExtensionContext.GetDotExe();
				this.optionsPage.DotExe = dotExe;
			}

			string mscGenExe = this.optionsPage.MscGenExe;
			if (mscGenExe == null || mscGenExe == "")
			{
				mscGenExe = ExtensionCommon.ExtensionContext.GetMscGenExe();
				this.optionsPage.DotExe = mscGenExe;
			}

			string plantUmlJar = this.optionsPage.PlantUmlJar;
			if (plantUmlJar == null || plantUmlJar == "")
			{
				plantUmlJar = ExtensionCommon.ExtensionContext.GetPlantUmlJar();
				this.optionsPage.PlantUmlJar = plantUmlJar;
			}

			string mimeTeXExe = this.optionsPage.MimeTeXExe;
			if (mimeTeXExe == null || mimeTeXExe == "")
			{
				mimeTeXExe = ExtensionCommon.ExtensionContext.GetMimeTeXExe();
				this.optionsPage.MimeTeXExe = mimeTeXExe;
			}
			
		  this.textBoxDoxDotExe.Text = dotExe;
			this.textBoxDoxMscGenExe.Text = mscGenExe;
			this.textBoxDoxPlantUmlJar.Text = plantUmlJar;
			this.textBoxDoxMimeTexExe.Text = mimeTeXExe;

		  ToolTip toolTip = new ToolTip();
			toolTip.SetToolTip(this.textBoxDoxDotExe, "Dot Greph: Full path and file name of 'dot.exe'");
			toolTip.SetToolTip(this.textBoxDoxMscGenExe, "Message Sequence Chart: Full path and file name of 'mscgen.exe'");
			toolTip.SetToolTip(this.textBoxDoxPlantUmlJar, "Plant UML: Full path and file name of 'plantuml.jar'");
			toolTip.SetToolTip(this.textBoxDoxMimeTexExe, "LaTeX formula: Full path and file name of 'mimetex.exe'");
		}

		private void checkBoxDoxSource_Click(object sender, EventArgs e)
		{
			this.optionsPage.UseSourceBrowser = this.checkBoxDoxSource.Checked;
		}

		private void checkBoxConfigFile_Click(object sender, EventArgs e)
		{
			this.optionsPage.UseConfigFile = this.checkBoxConfigFile.Checked;
		}

		private void textBoxDoxDotExe_Leave(object sender, EventArgs e)
		{
			this.optionsPage.DotExe = this.textBoxDoxDotExe.Text;
		}

		private void textBoxDoxMscGenExe_Leave(object sender, EventArgs e)
		{
			this.optionsPage.MscGenExe = this.textBoxDoxMscGenExe.Text;
		}

		private void textBoxDoxPlantUmlJar_Leave(object sender, EventArgs e)
		{
			this.optionsPage.PlantUmlJar = this.textBoxDoxPlantUmlJar.Text;
		}

		private void textBoxDoxMimeTexExe_Leave(object sender, EventArgs e)
		{
			this.optionsPage.MimeTeXExe = this.textBoxDoxMimeTexExe.Text;
		}

		private void buttonUpdateCache_Click(object sender, EventArgs e)
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

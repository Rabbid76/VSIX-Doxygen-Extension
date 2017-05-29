using System.ComponentModel;
using Microsoft.VisualStudio.Shell;


namespace DoxPreviewExt.Configuration
{
	/// \brief Options Page Grid
	/// cf Creating an Options Page
	/// [https://msdn.microsoft.com/en-us/library/bb166195.aspx?f=255&MSPPError=-2147217396]
	public class OptionPageActionSettings : DialogPage
	{
		private bool _clickOnRef = true;
		private bool _generalQuicktips = true;
		private bool _refQuicktip = true;
		private bool _imageQuicktip = true;
		private bool _dotQuicktip = true;
		private bool _mscQuicktip = true;
		private bool _plantUmlQuicktip = true;
		private bool _latexFormulaQuicktip = true;

		/// \brief Browse Reference, if clicked on
		[Category(ExtensionCommon.ExtensionContext.ConstDoxOptionsActionSettingsName)]
		[DisplayName("Click on Reference  @ref")]
		[Description("Click on Reference  @ref")]
		public bool ClickOnRef
		{
			get { return _clickOnRef; }
			set { _clickOnRef = value; }
		}

		/// \brief Show general Quicktips
		[Category(ExtensionCommon.ExtensionContext.ConstDoxOptionsActionSettingsName)]
		[DisplayName("General Quicktips  @...")]
		[Description("General Quicktips  @...")]
		public bool GeneralQicktips
		{
			get { return _generalQuicktips; }
			set { _generalQuicktips = value; }
		}

		/// \brief Show reference URL in Quicktip
		[Category(ExtensionCommon.ExtensionContext.ConstDoxOptionsActionSettingsName)]
		[DisplayName("Reference Quicktip  @ref")]
		[Description("Reference Quicktip  @ref")]
		public bool RefQuicktip
		{
			get { return _refQuicktip; }
			set { _refQuicktip = value; }
		}

		/// \brief Show bitmap preview in Quicktip
		[Category(ExtensionCommon.ExtensionContext.ConstDoxOptionsActionSettingsName)]
		[DisplayName("Image Quicktip  @image")]
		[Description("Image Quicktip  @image")]
		public bool ImageQuicktip
		{
			get { return _imageQuicktip; }
			set { _imageQuicktip = value; }
		}

		/// \brief Show Dot Graph preview in Quicktip
		[Category(ExtensionCommon.ExtensionContext.ConstDoxOptionsActionSettingsName)]
		[DisplayName("Dot Graph Quicktip  @dot")]
		[Description("Dot Graph Quicktip  @dot")]
		public bool DotQuicktip
		{
			get { return _dotQuicktip; }
			set { _dotQuicktip = value; }
		}

		/// \brief Show Message Sequence Chart preview in Quicktip
		[Category(ExtensionCommon.ExtensionContext.ConstDoxOptionsActionSettingsName)]
		[DisplayName("Message Sequence Chart Quicktip  @msc")]
		[Description("Message Sequence Chart Quicktip  @msc")]
		public bool MscQuicktip
		{
			get { return _mscQuicktip; }
			set { _mscQuicktip = value; }
		}

		/// \brief Show Plant UML preview in Quicktip
		[Category(ExtensionCommon.ExtensionContext.ConstDoxOptionsActionSettingsName)]
		[DisplayName("Plant UML Quicktip  @startuml")]
		[Description("Plant UML Quicktip  @startuml")]
		public bool PlantUmlQuicktip
		{
			get { return _plantUmlQuicktip; }
			set { _plantUmlQuicktip = value; }
		}

		/// \brief Show Latex Formula  preview in Quicktip
		[Category(ExtensionCommon.ExtensionContext.ConstDoxOptionsActionSettingsName)]
		[DisplayName("Latex Formula Quicktip  @f$ / @f[")]
		[Description("Latex Formula Quicktip  @f$ / @f[")]
		public bool LatexFormulaQuicktip
		{
			get { return _latexFormulaQuicktip; }
			set { _latexFormulaQuicktip = value; }
		}
	}
}

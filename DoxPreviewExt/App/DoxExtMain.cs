//------------------------------------------------------------------------------
// <copyright file="EliteExtension.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using EnvDTE;
using EnvDTE80;


namespace DoxPreviewExt.App
{
	/// <summary>
	/// This is the class that implements the package exposed by this assembly.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The minimum requirement for a class to be considered a valid package for Visual Studio
	/// is to implement the IVsPackage interface and register itself with the shell.
	/// This package uses the helper classes defined inside the Managed Package Framework (MPF)
	/// to do it: it derives from the Package class that provides the implementation of the
	/// IVsPackage interface and uses the registration attributes defined in the framework to
	/// register itself and its components with the shell. These attributes tell the pkgdef creation
	/// utility what data to put into .pkgdef file.
	/// </para>
	/// <para>
	/// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
	/// </para>
	/// </remarks>
	[PackageRegistration(UseManagedResourcesOnly = true)]
	[InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
	[Guid(DoxExtMain.PackageGuidString)]
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
	[ProvideMenuResource("Menus.ctmenu", 1)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.CodeWindow_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExists_string)]

	/// options page attribute for doxygen display and action page
	[ProvideOptionPage(typeof(Configuration.OptionPageActionSettings),
		ExtensionCommon.ExtensionContext.ConstOptionsCategoryName,
		ExtensionCommon.ExtensionContext.ConstDoxOptionsActionSettingsName,
		0, 0, true)]

	/// options page attribute for doxygen configuration page
	[ProvideOptionPage(typeof(Configuration.OptionPageDoxygenConfiguration),
		ExtensionCommon.ExtensionContext.ConstOptionsCategoryName,
		ExtensionCommon.ExtensionContext.ConstDoxOptionsConfigurationName,
		0, 0, true)]

	public sealed class DoxExtMain : Package
	{
		/// <summary>
		/// EliteExtension GUID string.
		/// </summary>
		public const string PackageGuidString = "64DF8652-96DC-4000-82AC-560646B0C8C2";

		private CommandManager cmdManager;

		/// <summary>
		/// Initializes a new instance of the <see cref="DoxExtMain"/> class.
		/// </summary>
		public DoxExtMain()
		{
			// Inside this method you can place any initialization code that does not require
			// any Visual Studio service because at this point the package object is created but
			// not sited yet inside Visual Studio environment. The place to do all the other
			// initialization is the Initialize method.
		}

		private DoxUtil.CManager _doxManager;
		public DoxUtil.CManager DoxManager { get { return _doxManager; } }

		#region Package Members

		/// <summary>
		/// Initialization of the package; this method is called right after the package is sited, so this is the place
		/// where you can put all the initialization code that rely on services provided by VisualStudio.
		/// </summary>
		protected override void Initialize()
    {
      base.Initialize();

      var dte = GetService(typeof(DTE)) as DTE;

      this.cmdManager = new CommandManager(dte as DTE2);
			this._doxManager = new DoxUtil.CManager(this, dte as DTE2);

			Command.CommandDocumentSource.Initialize(this, this.cmdManager, 257, Command.CommandDocumentSource.Type.Head);
			// TODO $$$ Command.CommandDocumentSource.Initialize(this, this.cmdManager, xxx Command.CommandDocumentSource.Type.BugTracker);
			Command.CommandDoxygenBrowseCurrentRef.Initialize(this, this.cmdManager);
			Command.CommandDoxygenBrowsePage.Initialize(this, this.cmdManager, 262 /* cmdidDoxygenSourcebrowseId */, true, false, "");
			Command.CommandDoxygenBrowsePage.Initialize(this, this.cmdManager, 267 /* cmdidDoxygenShowPreviewId */, false, true, "");
			Command.CommandDoxgenGeneratePreview.Initialize(this, this.cmdManager);
		}

		#endregion
	}
}

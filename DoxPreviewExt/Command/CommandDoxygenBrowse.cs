using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;


namespace DoxPreviewExt.Command
{
	internal sealed class CommandDoxygenBrowse : CommandBase
	{
		public static readonly Guid CommandSet = new Guid("C7C94F14-0506-4F23-973D-E01A0685E730");

		public int CommandId { get; }
		public bool UseSourcebrowser { get; }
		public bool UseLocalDox { get; }
		private string htmlDest_;

		private CommandDoxygenBrowse(Package package, App.CommandManager cmdManager, int commandId, bool useSourcebrowser, bool useLocal, string htmlDest)
			: base(package, cmdManager)
		{
			this.CommandId = commandId;
			this.UseSourcebrowser = useSourcebrowser;
			this.UseLocalDox = useLocal;
			this.htmlDest_ = htmlDest;

			if (package == null)
			{
				throw new ArgumentNullException("package");
			}

			OleMenuCommandService commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
			if (commandService != null)
			{
				var menuCommandID = new CommandID(CommandSet, CommandId);
				var menuItem = new OleMenuCommand(this.MenuItemCallback, menuCommandID);
				menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
				commandService.AddCommand(menuItem);
			}
		}

		public static CommandDoxygenBrowse Instance
		{
			get;
			private set;
		}

		public static void Initialize(Package package, App.CommandManager cmdManager, int commandId, bool useSourcebrowser, bool useLocal, string htmlDest)
		{
			Instance = new CommandDoxygenBrowse(package, cmdManager, commandId, useSourcebrowser, useLocal, htmlDest);
		}

		private void MenuItem_BeforeQueryStatus(object sender, EventArgs e)
		{
			var cmd = (OleMenuCommand)sender;
			cmd.Visible = false;

			DoxUtil.CManager manager = DoxUtil.CManager.Manager;
			if (this.UseSourcebrowser && manager != null && manager.Options.UseSourceBrowser)
				cmd.Visible = true;
			if (this.UseLocalDox && manager != null && manager.Options.UseConfigFile)
				cmd.Visible = true;
		}

		private void MenuItemCallback(object sender, EventArgs e)
		{
			try
			{
				this.Perform();
			}
			catch (System.Exception ex)
			{
				System.Windows.Forms.MessageBox.Show(ex.Message);
			}
		}

		private void Perform()
		{
			DoxUtil.CManager manager = DoxUtil.CManager.Manager;
			if (manager == null)
				return;

			string root = manager.Options.CurrentSourceBrowser;
			if (this.UseSourcebrowser == false && this.UseLocalDox)
				root = manager.Options.LocalHTMLPath;

			string doxURL = root;
			if (doxURL.EndsWith("/") == false)
				doxURL += "/";
			doxURL += (htmlDest_ != "" ? htmlDest_ : "index.html");
			if (doxURL != "")
			  System.Diagnostics.Process.Start(doxURL);
		}
	}
}

using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;


namespace DoxPreviewExt.Command
{
	internal sealed class CommandDoxygenBrowseRef : CommandBase
	{
		public static readonly Guid CommandSet = new Guid("C7C94F14-0506-4F23-973D-E01A0685E730");

		public int CommandId { get; }

		private string doxRef_;

		private CommandDoxygenBrowseRef(Package package, App.CommandManager cmdManager, int commandId, string doxRef)
			: base(package, cmdManager)
		{
			this.CommandId = commandId;
			this.doxRef_ = doxRef;

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

	  public static CommandDoxygenBrowseRef Instance
		{
			get;
			private set;
		}

		public static void Initialize(Package package, App.CommandManager cmdManager, int commandId, string doxRef)
		{
			Instance = new CommandDoxygenBrowseRef(package, cmdManager, commandId, doxRef);
		}

		private void MenuItem_BeforeQueryStatus(object sender, EventArgs e)
		{
			var cmd = (OleMenuCommand)sender;
			cmd.Visible = false;

			string doxURL = CommandManager.DoxManager.FindDoxygenLink(doxRef_, false);
			if (doxURL == "")
				return;

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
			string doxURL = CommandManager.DoxManager.FindDoxygenLink(doxRef_, false);
			if (doxURL != "")
			{
				//Application.DoEvents();
				//selection.Insert("\\\\ [" + doxURL + "]\n", 2);

				DoxUtil.CManager.OpenHTML(doxURL);
			}
		}
	}
}
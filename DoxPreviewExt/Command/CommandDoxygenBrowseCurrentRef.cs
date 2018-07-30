using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;


namespace DoxPreviewExt.Command
{
	internal sealed class CommandDoxygenBrowseCurrentRef : CommandBase
	{
		/// <summary>
		/// Command ID.
		/// </summary>
		public const int CommandId = 261;

		/// <summary>
		/// Command menu group (command set GUID).
		/// </summary>
		public static readonly Guid CommandSet = new Guid("C7C94F14-0506-4F23-973D-E01A0685E730");

		private string currentDoxRef_;

		/// <summary>
		/// Initializes a new instance of the <see cref="CommandDocumentSource"/> class.
		/// Adds our command handlers for menu (commands must exist in the command table file)
		/// </summary>
		/// <param name="package">Owner package, not null.</param>
		private CommandDoxygenBrowseCurrentRef(Package package, App.CommandManager cmdManager)
			: base(package, cmdManager)
		{
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

		/// <summary>
		/// Gets the instance of the command.
		/// </summary>
		public static CommandDoxygenBrowseCurrentRef Instance
		{
			get;
			private set;
		}

		/// <summary>
		/// Initializes the singleton instance of the command.
		/// </summary>
		/// <param name="package">Owner package, not null.</param>
		public static void Initialize(Package package, App.CommandManager cmdManager)
		{
			Instance = new CommandDoxygenBrowseCurrentRef(package, cmdManager);
		}

		private void MenuItem_BeforeQueryStatus(object sender, EventArgs e)
		{
			var cmd = (OleMenuCommand)sender;
			cmd.Visible = false;

			string name = GetActiveDocumentName();
			if (string.IsNullOrEmpty(name))
				return;

			string doxRef = this.GetFirstDoxygenReferenceFromCurrentLine();
			if (doxRef == "")
				return;

			currentDoxRef_ = doxRef;
			string doxURL = CommandManager.DoxManager.FindDoxygenLink(currentDoxRef_,false);
			if (doxURL == "")
				return;

			cmd.Visible = true;
		}

		/// <summary>
		/// This function is the callback used to execute the command when the menu item is clicked.
		/// See the constructor to see how the menu item is associated with this function using
		/// OleMenuCommandService service and MenuCommand class.
		/// </summary>
		/// <param name="sender">Event sender.</param>
		/// <param name="e">Event args.</param>
		private void MenuItemCallback(object sender, EventArgs e)
		{
			try
			{
				string name = this.GetActiveDocumentName();

				if (string.IsNullOrEmpty(name) == false)
					this.Perform(name.EndsWith(".cs"));
			}
			catch (System.Exception ex)
			{
				System.Windows.Forms.MessageBox.Show(ex.Message);
			}
		}

		/// <summary>
		/// Performs the command : Opens the Doxygen page
		/// </summary>
		private void Perform(bool dotNet)
		{
			string doxURL = CommandManager.DoxManager.FindDoxygenLink(currentDoxRef_,false);
			if (doxURL != "")
			{
				//Application.DoEvents();
				//selection.Insert("\\\\ [" + doxURL + "]\n", 2);

				DoxUtil.CManager.OpenHTML(doxURL);
			}
		}
	}
}

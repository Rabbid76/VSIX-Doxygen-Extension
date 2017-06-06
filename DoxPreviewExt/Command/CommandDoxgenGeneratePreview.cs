using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;
using System.Windows.Forms;
using System.Diagnostics;


namespace DoxPreviewExt.Command
{
	internal sealed class CommandDoxgenGeneratePreview : CommandBase
	{
		public const int CommandId = 266;
		public static readonly Guid CommandSet = new Guid("C7C94F14-0506-4F23-973D-E01A0685E730");

		private CommandDoxgenGeneratePreview(Package package, App.CommandManager cmdManager)
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

		public static CommandDoxgenGeneratePreview Instance
		{
			get;
			private set;
		}

		public static void Initialize(Package package, App.CommandManager cmdManager)
		{
			Instance = new CommandDoxgenGeneratePreview(package, cmdManager);
		}

		private void MenuItem_BeforeQueryStatus(object sender, EventArgs e)
		{
			var cmd = (OleMenuCommand)sender;
			cmd.Visible = false;

			DoxUtil.CManager manager = DoxUtil.CManager.Manager;
			if (manager != null && manager.Options.UseConfigFile)
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

			string doxygenGenExe = ExtensionCommon.ExtensionContext.ConstDoxygenExePath;
			string doxPreviewTempl = manager.Options.CurrentDoxygenConfigFile;
			string doxPreviewLoc = manager.Options.DoxRootPath;

			GenerateExitHandler eh = new GenerateExitHandler();

			ProcessStartInfo cmdInfo = new ProcessStartInfo(doxygenGenExe, doxPreviewTempl);
			cmdInfo.WorkingDirectory = doxPreviewLoc;

			System.Diagnostics.Process proc = new System.Diagnostics.Process();
			proc.StartInfo = cmdInfo;
			proc.EnableRaisingEvents = true;
			proc.Exited += new EventHandler(eh.GenerateExitHandlerEvent);
			proc.Start();
		}

		class GenerateExitHandler
		{
			public void GenerateExitHandlerEvent(object sender, EventArgs e)
			{
				MessageBox.Show("Generation complete");
			}
		}
	}
}

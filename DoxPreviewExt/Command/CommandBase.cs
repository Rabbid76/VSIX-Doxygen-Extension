using System;
using EnvDTE;
using Microsoft.VisualStudio.Shell;


namespace DoxPreviewExt.Command
{
	internal class CommandBase
	{
		

		/// <summary>
		/// VS Package that provides this command, not null.
		/// </summary>
		private readonly Package package;
		private readonly App.CommandManager cmdManager;

		/// <summary>
		/// Gets the service provider from the owner package.
		/// </summary>
		protected IServiceProvider ServiceProvider
		{
			get
			{
				return this.package;
			}
		}

		/// <summary>
		/// Gets the service provider from the owner package.
		/// </summary>
		protected App.CommandManager CommandManager
		{
			get
			{
				return this.cmdManager;
			}
		}

		private T_PAGE Page<T_PAGE>() where T_PAGE : class
		{
			T_PAGE page = this.package.GetDialogPage(typeof(T_PAGE)) as T_PAGE;
			return page;
		}
		
		public CommandBase(Package package, App.CommandManager cmdManager)
		{
			if (package == null)
			{
				throw new ArgumentNullException("package");
			}

			if (cmdManager == null)
			{
				throw new ArgumentNullException("cmdManager");
			}

			this.package = package;
			this.cmdManager = cmdManager;
		}


		protected bool SplitFileName(string fullFileName, out string filePath, out string fileName)
		{
			int p = fullFileName.Length - 1;
			while ((p >= 0) && (fullFileName.Substring(p, 1) != @"\"))
			{
				p--;
			}
			if (p > 0)
			{
				filePath = fullFileName.Substring(0, p + 1);
				fileName = fullFileName.Substring(p + 1, fullFileName.Length - p - 1);
				return true;
			}
			else
			{
				filePath = "";
				fileName = "";
				return false;
			}

		}

		protected string GetActiveDocumentName()
		{
			if (CommandManager.ApplicationObject.ActiveDocument == null)
				return null;

			return CommandManager.ApplicationObject.ActiveDocument.Name;
		}

		#region IsCSourceFile
		/// <summary>
		///
		/// </summary>
		/// <revision date="2010-03-25" author="klaus"></revision>
		protected bool IsCSourceFile(Document doc)
		{
			return (doc.Language == "C/C++");
		}
		#endregion

		#region IsCSourceFile
		/// <summary>
		///
		/// </summary>
		/// <revision date="2010-03-25" author="klaus"></revision>
		protected bool IsCSourceFile(TextDocument doc)
		{
			return (doc.Language == "C/C++");
		}
		#endregion

		#region IsActiveDocumentCSourceFile
		/// <summary>
		///
		/// </summary>
		/// <revision date="2010-03-25" author="klaus"></revision>
		protected bool IsActiveDocumentCSourceFile()
		{
			if (CommandManager.ApplicationObject != null)
			{
				Document doc = this.CommandManager.ApplicationObject.ActiveDocument;
				if (doc != null)
					return IsCSourceFile(doc);
			}
			return false;
		}
		#endregion

		//! \brief Gets either the selected text or the current line
		//! EnvDTE.TextSelection: [https://msdn.microsoft.com/en-us/library/envdte.textselection.aspx]
		protected string GetCurrentSelectionOrLine()
		{
			EnvDTE.TextSelection selection = (EnvDTE.TextSelection)this.CommandManager.ApplicationObject.ActiveDocument.Selection;
			string selectionText = selection.Text.Trim();
			if (selectionText != "")
				return selectionText;
			int currentLine = selection.CurrentLine;
			selection.SelectLine();
			string lineText = selection.Text.Trim();
			selection.GotoLine(currentLine, false);
			selection.Text = selectionText;
			return lineText;
		}

		//! \brief find the first doxygen reference in the current line or curren selection
		protected string GetFirstDoxygenReferenceFromCurrentLine()
		{
			string text = this.GetCurrentSelectionOrLine();
			if (text == "")
				return "";
			string doxRef = CommandManager.DoxManager.FindFirstDoxygenReference(text);
			return doxRef;
		}
	}
}

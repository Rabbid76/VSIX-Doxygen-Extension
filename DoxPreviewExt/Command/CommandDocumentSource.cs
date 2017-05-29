//------------------------------------------------------------------------------
// <copyright file="DocumentSource.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Windows.Forms;
using EnvDTE;


namespace DoxPreviewExt.Command
{
  /// <summary>
  /// Command handler
  /// </summary>
  internal sealed class CommandDocumentSource : CommandBase
  {
    /// <summary>
    /// Command ID.
    /// </summary>
    public const int CommandId = 257;

    /// <summary>
    /// Command menu group (command set GUID).
    /// </summary>
    public static readonly Guid CommandSet = new Guid("C7C94F14-0506-4F23-973D-E01A0685E730");

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandDocumentSource"/> class.
    /// Adds our command handlers for menu (commands must exist in the command table file)
    /// </summary>
    /// <param name="package">Owner package, not null.</param>
    private CommandDocumentSource(Package package, App.CommandManager cmdManager)
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
    public static CommandDocumentSource Instance
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
      Instance = new CommandDocumentSource(package, cmdManager);
    }

    private void MenuItem_BeforeQueryStatus(object sender, EventArgs e)
    {
      var cmd = (OleMenuCommand)sender;
      cmd.Visible = false;

      string name = GetActiveDocumentName();
      if (string.IsNullOrEmpty(name))
        return;

      if ((name.EndsWith(".h")) || (name.EndsWith(".hi")) || (name.EndsWith(".he")) ||
          (name.EndsWith(".cpp")) || (name.EndsWith(".c")) || (name.EndsWith(".cs")))
      {
        cmd.Visible = true;
      }
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
        string name = GetActiveDocumentName();

        if (string.IsNullOrEmpty(name) == false)
          this.Perform(name.EndsWith(".cs"));
      }
      catch (System.Exception ex)
      {
        System.Windows.Forms.MessageBox.Show(ex.Message);
      }
    }


    private string Space(int count)
    {
      string res = "";
      while (res.Length < count)
      {
        res += " ";
      }
      return res;
    }
    /// <summary>
    /// Performs the command : Launch the application in Windows Explorer
    /// </summary>
    private void Perform(bool dotNet)
    {


      bool makeRegion;
      string date = DateTime.Now.ToString("yyyy-MM-dd");
      Regex rx = new Regex(@"[^ \t]");
      List<string> lines = new List<string>();
      List<string> SpaceShift = new List<string>(2);
      List<string> result = new List<string>();
      EnvDTE.TextSelection selection = (EnvDTE.TextSelection)this.CommandManager.ApplicationObject.ActiveDocument.Selection;
      string text = selection.Text;
      SpaceShift.Insert(0, "");


      if (text.Trim() != "")
      {
        text = text.TrimEnd();
        lines.AddRange(text.Split('\n'));
        makeRegion = true;
      }
      else
      {
        makeRegion = false;
      }
      if (lines.Count > 0)
      {
        SpaceShift.InsertRange(0, rx.Split(lines[0], 2));
      }
      if (makeRegion)
      {
        if (dotNet)
        {
          result.Add(SpaceShift[0] + "#region " + this.GetFunctionName(this.GetFirstLine(text)));
        }
        else
        {
          result.Add(SpaceShift[0] + "#pragma region " + this.GetFunctionName(this.GetFirstLine(text)));
        }
      }
      if (dotNet)
      {
        result.Add(SpaceShift[0] + "/// <summary>");
        result.Add(SpaceShift[0] + "///");
        result.Add(SpaceShift[0] + "/// </summary>");
        result.Add(SpaceShift[0] + "/// <revision date=\"" + date + "\" author=\"" + this.CommandManager.UserName + "\"></revision>");
      }
      else
      {
        if ((text.Length == 0) || (text[0] != '/'))
        {
          result.Add(SpaceShift[0] + "/******************************************************************//**");
          result.Add(SpaceShift[0] + "* \\brief   ");
          result.Add(SpaceShift[0] + "* ");
          result.Add(SpaceShift[0] + "* \\author  " + this.CommandManager.UserName);
          result.Add(SpaceShift[0] + "* \\date    " + date);
          result.Add(SpaceShift[0] + "* \\version 1.0");
          result.Add(SpaceShift[0] + "**********************************************************************/");
        }
      }
      if (text != "")
      {
        result.Add(text);
      }
      if (makeRegion)
      {
        if (dotNet)
        {
          result.Add(SpaceShift[0] + "#endregion\n");
        }
        else
        {
          result.Add(SpaceShift[0] + "#pragma endregion\n");
        }
      }
      Application.DoEvents();
      if (text == "")
      {
        selection.StartOfLine(vsStartOfLineOptions.vsStartOfLineOptionsFirstColumn, false);
      }
      string str = "";
      for (int i = 0; i < result.Count; i++)
      {
        str += result[i] + "\n";
      }
      selection.Insert(str, 2);
    }

    #region  GetFirstLine
    /// <summary>
    ///
    /// </summary>
    /// <revision date="2008-10-29" author="klaus"></revision>
    private string GetFirstLine(string text)
    {
      string line = "";
      int i = 0;
      if (text.Length > 1)
      {
        if (text.Substring(i, 2) == "/*")
        {
          i++;
          while ((i < text.Length - 1) && (text.Substring(i, 2) != "*/"))
          {
            i++;
          }
          i += 2;
          while ((i < text.Length) && ((text[i] == '\n') || (text[i] == '\r')))
          {
            i++;
          }
        }
        while (i < text.Length)
        {
          if (text[i] < ' ')
          {
            return line.Trim();
          }
          else
          {
            line += text[i];
          }
          i++;
        }
      }
      return text;
    }
    #endregion

    #region  Split
    /// <summary>
    ///
    /// </summary>
    /// <revision date="2009-01-12" author="klaus"></revision>
    private List<string> Split(string text, string splitChars)
    {
      List<string> result = new List<string>();
      string line = "";

      for (int i = 0; i < text.Length; i++)
      {
        if (splitChars.IndexOf(text[i]) >= 0)
        {
          result.Add(line);
          line = "";
        }
        else
        {
          line = line + text[i];
        }
      }
      if (line != "")
      {
        result.Add(line);
      }
      return result;
    }
    #endregion

    #region  GetFunctionName
    /// <summary>
    ///
    /// </summary>
    /// <revision date="2009-01-12" author="klaus"></revision>
    private string GetFunctionName(string text)
    {
      int p1, p2;


      p2 = text.Length - 1;
      while ((p2 > 0) && (text[p2] != '('))
      {
        p2--;
      }
      if (p2 > 0)
      {
        p1 = p2;
        while ((p1 > 0) && (text[p1] != ' '))
        {
          p1--;
        }
        if (p1 > 0)
        {
          return text.Substring(p1, p2 - p1).Trim();
        }
      }


      string logestWord = "";
      int length = 0;
      List<string> words = this.Split(text, " ;()#");

      foreach (string w in words)
      {
        if (w.Length > length)
        {
          logestWord = w;
          length = w.Length;
        }
      }
      if (logestWord.IndexOf("::") > 0)
      {
        logestWord = logestWord.Substring(logestWord.IndexOf("::") + 2);
      }
      while (logestWord.IndexOf("~") > 0)
      {
        logestWord = logestWord.Remove(logestWord.IndexOf("~"), 1);
      }
      return logestWord.Trim();
    }
    #endregion
  }
}

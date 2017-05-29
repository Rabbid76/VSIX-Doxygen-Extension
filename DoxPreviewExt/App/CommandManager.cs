using System;
using EnvDTE;
using EnvDTE80;


namespace DoxPreviewExt.App
{
  public class CommandManager
  { 

    private const string ConstStartpattern = "#pragma region";
    private const string ConstEndpattern = "#pragma endregion";

    private const string ConstStartpatternDotNet = "#region";
    private const string ConstEndpatternDotNet = "#endregion";

    private DTE2 applicationObject;
    private string userName;
    private Document toogleRegionDocument; //!< EnvDTE.Document [https://msdn.microsoft.com/en-us/library/envdte.document.aspx]

		public DoxUtil.CManager DoxManager
		{
			get
			{
				DoxUtil.CManager manager = DoxUtil.CManager.Manager;
				if (manager != null)
				  manager.Update();
				return manager;
			}
		}

		public DTE2 ApplicationObject
    {
      get
      {
        return this.applicationObject;
      }
    }


     public string UserName
    {
      get
      {
        return this.userName;
      }
    }

		public CommandManager(DTE2 applicationObject)
		{
			if (applicationObject == null)
				throw new ArgumentException("ApplicationObject is null");

			this.applicationObject = applicationObject;

			this.userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToLower();
			if (this.userName.IndexOf("\\") > 0)
			{
				this.userName = this.userName.Substring(this.userName.IndexOf("\\") + 1).ToLower();
			}
			this.toogleRegionDocument = null;
		}

    public bool GetPatterns(Document document, out string startpattern, out string endpattern)
    {
      startpattern = "";
      endpattern = "";

      if (document.Language == "C/C++")
      {
        startpattern = ConstStartpattern;
        endpattern = ConstEndpattern;
      }
      else if (document.Language == "CSharp")
      {
        startpattern = ConstStartpatternDotNet;
        endpattern = ConstEndpatternDotNet;
      }
      if ((startpattern != "") && (endpattern != ""))
      {
        return true;
      }
      return false;
    }

    public bool ToggleAllRegions(EnvDTE.Document doc, bool closeAll)
    {
      bool open = false;
      TextSelection ts = (TextSelection)doc.Selection;

      string startpattern;
      string endpattern;

      if (!this.GetPatterns(doc, out startpattern, out endpattern))
      {
        return false;
      }
      ts.EndOfDocument(false);
      EditPoint ep = ts.ActivePoint.CreateEditPoint();
      string line;

      while (!ep.AtStartOfDocument)
      {
        ts.StartOfLine(vsStartOfLineOptions.vsStartOfLineOptionsFirstColumn, false);
        ts.LineUp(true, 1);
        line = ts.Text.ToLower().Trim();
        if (line.StartsWith(endpattern))
        {
          open = true;
        }
        else if (line.StartsWith(startpattern))
        {
          if (closeAll)
          {
            if (open)
            {
              doc.DTE.ExecuteCommand("Edit.ToggleOutliningExpansion", "");
            }
          }
          else
          {
            if (!open)
            {
              doc.DTE.ExecuteCommand("Edit.ToggleOutliningExpansion", "");
            }
          }
          open = false;
        }
        ep = ts.ActivePoint.CreateEditPoint();
      }
      toogleRegionDocument = doc;
      ts.Cancel();
      return true;
    }
  }
}

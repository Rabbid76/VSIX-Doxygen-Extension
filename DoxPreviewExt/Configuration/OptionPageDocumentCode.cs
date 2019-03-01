using System;
using Microsoft.VisualStudio.Shell;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.ComponentModel;


namespace DoxPreviewExt.Configuration
{
    /// \brief Options Page Grid
    /// cf Creating an Options Page
    /// [https://docs.microsoft.com/en-us/visualstudio/extensibility/creating-an-options-page?view=vs-2017]
    public class OptionPageDocumentCode : DialogPage
    {
        private bool _user_defined_head = false;
        private string _head_comment_block = "";

        /// \brief Browse Reference, if clicked on
        [Category(ExtensionCommon.ExtensionContext.ConstDoxOptionsDocumentCodeName)]
        [DisplayName("User defined head")]
        [Description("User defined head")]
        public bool UserDefinedHead
        {
            get { return _user_defined_head; }
            set { _user_defined_head = value; }
        }

        /// \brief Browse Reference, if clicked on
        [Category(ExtensionCommon.ExtensionContext.ConstDoxOptionsDocumentCodeName)]
        [DisplayName("Head comment block")]
        [Description("Head comment block")]
        public string HeadCommentBlock
        {
            get { return _head_comment_block; }
            set { _head_comment_block = value; }
        }

        protected override IWin32Window Window
        {
            get
            {
                DocumentCodeUserControl page = new DocumentCodeUserControl();
                page.optionsPage = this;
                page.Initialize();
                return page;
            }
        }
    }
}

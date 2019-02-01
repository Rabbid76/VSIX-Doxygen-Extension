using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DoxPreviewExt.Configuration
{
    public partial class DocumentCodeUserControl : UserControl
    {
        public DocumentCodeUserControl()
        {
            InitializeComponent();
        }

        internal OptionPageDocumentCode optionsPage;

        public void Initialize()
        {
        }

        private void DocumentCodeUserControl_Load(object sender, EventArgs e)
        {
            // initialize had comment block settings
            this.checkUserHeader.Checked = this.optionsPage.UserDefinedHead;
            this.richTextHeadComment.Text = this.optionsPage.HeadCommentBlock;
        }

        private void checkBoxUserHeader_Click(object sender, EventArgs e)
        {
            this.optionsPage.UserDefinedHead = this.checkUserHeader.Checked;
        }

        private void headCommentTextBox_TextChanged(object sender, EventArgs e)
        {
            this.optionsPage.HeadCommentBlock = this.richTextHeadComment.Text;
        }

        private void headCommentTextBox_TextChanged2(object sender, EventArgs e)
        {

        }
    }
}

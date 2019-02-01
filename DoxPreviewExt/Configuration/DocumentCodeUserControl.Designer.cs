namespace DoxPreviewExt.Configuration
{
    partial class DocumentCodeUserControl
    {
        /// <summary> 
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Komponenten-Designer generierter Code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.checkUserHeader = new System.Windows.Forms.CheckBox();
            this.richTextHeadComment = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // checkUserHeader
            // 
            this.checkUserHeader.AutoSize = true;
            this.checkUserHeader.Location = new System.Drawing.Point(3, 14);
            this.checkUserHeader.Name = "checkUserHeader";
            this.checkUserHeader.Size = new System.Drawing.Size(197, 17);
            this.checkUserHeader.TabIndex = 39;
            this.checkUserHeader.Text = "User defined header comment block";
            this.checkUserHeader.UseVisualStyleBackColor = true;
            this.checkUserHeader.Click += new System.EventHandler(this.checkBoxUserHeader_Click);
            // 
            // richTextHeadComment
            // 
            this.richTextHeadComment.AcceptsTab = true;
            this.richTextHeadComment.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextHeadComment.Location = new System.Drawing.Point(3, 37);
            this.richTextHeadComment.Name = "richTextHeadComment";
            this.richTextHeadComment.Size = new System.Drawing.Size(513, 194);
            this.richTextHeadComment.TabIndex = 42;
            this.richTextHeadComment.Text = "";
            this.richTextHeadComment.WordWrap = false;
            this.richTextHeadComment.TextChanged += new System.EventHandler(this.headCommentTextBox_TextChanged);
            // 
            // DocumentCodeUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.richTextHeadComment);
            this.Controls.Add(this.checkUserHeader);
            this.Name = "DocumentCodeUserControl";
            this.Size = new System.Drawing.Size(520, 266);
            this.Load += new System.EventHandler(this.DocumentCodeUserControl_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkUserHeader;
        private System.Windows.Forms.RichTextBox richTextHeadComment;
    }
}

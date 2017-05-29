namespace DoxPreviewExt.Configuration
{
	partial class DoxConfigUserControl
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
			this.checkBoxConfigFile = new System.Windows.Forms.CheckBox();
			this.textBoxDoxConfigFile = new System.Windows.Forms.TextBox();
			this.checkBoxDoxSource = new System.Windows.Forms.CheckBox();
			this.textBoxDoxSourceBrowser = new System.Windows.Forms.TextBox();
			this.labelDoxMimeTexPath = new System.Windows.Forms.Label();
			this.textBoxMimeTexPath = new System.Windows.Forms.TextBox();
			this.buttonUpdateCache = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// checkBoxConfigFile
			// 
			this.checkBoxConfigFile.AutoSize = true;
			this.checkBoxConfigFile.Location = new System.Drawing.Point(3, 43);
			this.checkBoxConfigFile.Name = "checkBoxConfigFile";
			this.checkBoxConfigFile.Size = new System.Drawing.Size(72, 17);
			this.checkBoxConfigFile.TabIndex = 31;
			this.checkBoxConfigFile.Text = "Config file";
			this.checkBoxConfigFile.UseVisualStyleBackColor = true;
			this.checkBoxConfigFile.Click += new System.EventHandler(this.checkBoxConfigFile_Click);
			// 
			// textBoxDoxConfigFile
			// 
			this.textBoxDoxConfigFile.Location = new System.Drawing.Point(104, 41);
			this.textBoxDoxConfigFile.Name = "textBoxDoxConfigFile";
			this.textBoxDoxConfigFile.ReadOnly = true;
			this.textBoxDoxConfigFile.Size = new System.Drawing.Size(386, 20);
			this.textBoxDoxConfigFile.TabIndex = 30;
			// 
			// checkBoxDoxSource
			// 
			this.checkBoxDoxSource.AutoSize = true;
			this.checkBoxDoxSource.Location = new System.Drawing.Point(3, 10);
			this.checkBoxDoxSource.Name = "checkBoxDoxSource";
			this.checkBoxDoxSource.Size = new System.Drawing.Size(97, 17);
			this.checkBoxDoxSource.TabIndex = 27;
			this.checkBoxDoxSource.Text = "Sourcebrowser";
			this.checkBoxDoxSource.UseVisualStyleBackColor = true;
			this.checkBoxDoxSource.Click += new System.EventHandler(this.checkBoxDoxSource_Click);
			// 
			// textBoxDoxSourceBrowser
			// 
			this.textBoxDoxSourceBrowser.Location = new System.Drawing.Point(104, 8);
			this.textBoxDoxSourceBrowser.Name = "textBoxDoxSourceBrowser";
			this.textBoxDoxSourceBrowser.ReadOnly = true;
			this.textBoxDoxSourceBrowser.Size = new System.Drawing.Size(386, 20);
			this.textBoxDoxSourceBrowser.TabIndex = 26;
			// 
			// labelDoxMimeTexPath
			// 
			this.labelDoxMimeTexPath.AutoSize = true;
			this.labelDoxMimeTexPath.Location = new System.Drawing.Point(3, 82);
			this.labelDoxMimeTexPath.Name = "labelDoxMimeTexPath";
			this.labelDoxMimeTexPath.Size = new System.Drawing.Size(93, 13);
			this.labelDoxMimeTexPath.TabIndex = 34;
			this.labelDoxMimeTexPath.Text = "mimeTex.exe path";
			// 
			// textBoxMimeTexPath
			// 
			this.textBoxMimeTexPath.Location = new System.Drawing.Point(104, 79);
			this.textBoxMimeTexPath.Name = "textBoxMimeTexPath";
			this.textBoxMimeTexPath.Size = new System.Drawing.Size(386, 20);
			this.textBoxMimeTexPath.TabIndex = 35;
			this.textBoxMimeTexPath.Leave += new System.EventHandler(this.textBoxMimeTexPath_Leave);
			// 
			// buttonUpdateCache
			// 
			this.buttonUpdateCache.Location = new System.Drawing.Point(3, 125);
			this.buttonUpdateCache.Name = "buttonUpdateCache";
			this.buttonUpdateCache.Size = new System.Drawing.Size(97, 23);
			this.buttonUpdateCache.TabIndex = 36;
			this.buttonUpdateCache.Text = "Update cache";
			this.buttonUpdateCache.UseVisualStyleBackColor = true;
			// 
			// DoxConfigUserControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.buttonUpdateCache);
			this.Controls.Add(this.textBoxMimeTexPath);
			this.Controls.Add(this.labelDoxMimeTexPath);
			this.Controls.Add(this.checkBoxConfigFile);
			this.Controls.Add(this.textBoxDoxConfigFile);
			this.Controls.Add(this.checkBoxDoxSource);
			this.Controls.Add(this.textBoxDoxSourceBrowser);
			this.Name = "DoxConfigUserControl";
			this.Size = new System.Drawing.Size(495, 154);
			this.Click += new System.EventHandler(this.DoxConfigUserControl_Click);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.CheckBox checkBoxConfigFile;
		private System.Windows.Forms.TextBox textBoxDoxConfigFile;
		private System.Windows.Forms.CheckBox checkBoxDoxSource;
		private System.Windows.Forms.TextBox textBoxDoxSourceBrowser;
		private System.Windows.Forms.Label labelDoxMimeTexPath;
		private System.Windows.Forms.TextBox textBoxMimeTexPath;
		private System.Windows.Forms.Button buttonUpdateCache;
	}
}

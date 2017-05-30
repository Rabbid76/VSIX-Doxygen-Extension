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
			this.buttonUpdateCache = new System.Windows.Forms.Button();
			this.textBoxDoxMimeTexExe = new System.Windows.Forms.TextBox();
			this.labelDoxMimeTexExe = new System.Windows.Forms.Label();
			this.checkBoxConfigFile = new System.Windows.Forms.CheckBox();
			this.textBoxDoxConfigFile = new System.Windows.Forms.TextBox();
			this.checkBoxDoxSource = new System.Windows.Forms.CheckBox();
			this.textBoxDoxSourceBrowser = new System.Windows.Forms.TextBox();
			this.textBoxDoxDotExe = new System.Windows.Forms.TextBox();
			this.labelDoxDotExe = new System.Windows.Forms.Label();
			this.textBoxDoxMscGenExe = new System.Windows.Forms.TextBox();
			this.labelDoxMscGenExe = new System.Windows.Forms.Label();
			this.textBoxDoxPlantUmlJar = new System.Windows.Forms.TextBox();
			this.labelDoxPlantUmlJar = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// buttonUpdateCache
			// 
			this.buttonUpdateCache.Location = new System.Drawing.Point(3, 239);
			this.buttonUpdateCache.Name = "buttonUpdateCache";
			this.buttonUpdateCache.Size = new System.Drawing.Size(97, 23);
			this.buttonUpdateCache.TabIndex = 43;
			this.buttonUpdateCache.Text = "Update cache";
			this.buttonUpdateCache.UseVisualStyleBackColor = true;
			this.buttonUpdateCache.Click += new System.EventHandler(this.buttonUpdateCache_Click);
			// 
			// textBoxDoxMimeTexExe
			// 
			this.textBoxDoxMimeTexExe.Location = new System.Drawing.Point(104, 191);
			this.textBoxDoxMimeTexExe.Name = "textBoxDoxMimeTexExe";
			this.textBoxDoxMimeTexExe.Size = new System.Drawing.Size(386, 20);
			this.textBoxDoxMimeTexExe.TabIndex = 42;
			this.textBoxDoxMimeTexExe.Leave += new System.EventHandler(this.textBoxMimeTexExe_Leave);
			// 
			// labelDoxMimeTexExe
			// 
			this.labelDoxMimeTexExe.AutoSize = true;
			this.labelDoxMimeTexExe.Location = new System.Drawing.Point(3, 194);
			this.labelDoxMimeTexExe.Name = "labelDoxMimeTexExe";
			this.labelDoxMimeTexExe.Size = new System.Drawing.Size(74, 13);
			this.labelDoxMimeTexExe.TabIndex = 41;
			this.labelDoxMimeTexExe.Text = "mimeTeX.exe ";
			// 
			// checkBoxConfigFile
			// 
			this.checkBoxConfigFile.AutoSize = true;
			this.checkBoxConfigFile.Location = new System.Drawing.Point(3, 45);
			this.checkBoxConfigFile.Name = "checkBoxConfigFile";
			this.checkBoxConfigFile.Size = new System.Drawing.Size(72, 17);
			this.checkBoxConfigFile.TabIndex = 40;
			this.checkBoxConfigFile.Text = "Config file";
			this.checkBoxConfigFile.UseVisualStyleBackColor = true;
			this.checkBoxConfigFile.Click += new System.EventHandler(this.checkBoxConfigFile_Click);
			// 
			// textBoxDoxConfigFile
			// 
			this.textBoxDoxConfigFile.Location = new System.Drawing.Point(104, 43);
			this.textBoxDoxConfigFile.Name = "textBoxDoxConfigFile";
			this.textBoxDoxConfigFile.ReadOnly = true;
			this.textBoxDoxConfigFile.Size = new System.Drawing.Size(386, 20);
			this.textBoxDoxConfigFile.TabIndex = 39;
			// 
			// checkBoxDoxSource
			// 
			this.checkBoxDoxSource.AutoSize = true;
			this.checkBoxDoxSource.Location = new System.Drawing.Point(3, 12);
			this.checkBoxDoxSource.Name = "checkBoxDoxSource";
			this.checkBoxDoxSource.Size = new System.Drawing.Size(97, 17);
			this.checkBoxDoxSource.TabIndex = 38;
			this.checkBoxDoxSource.Text = "Sourcebrowser";
			this.checkBoxDoxSource.UseVisualStyleBackColor = true;
			this.checkBoxDoxSource.Click += new System.EventHandler(this.checkBoxDoxSource_Click);
			// 
			// textBoxDoxSourceBrowser
			// 
			this.textBoxDoxSourceBrowser.Location = new System.Drawing.Point(104, 10);
			this.textBoxDoxSourceBrowser.Name = "textBoxDoxSourceBrowser";
			this.textBoxDoxSourceBrowser.ReadOnly = true;
			this.textBoxDoxSourceBrowser.Size = new System.Drawing.Size(386, 20);
			this.textBoxDoxSourceBrowser.TabIndex = 37;
			// 
			// textBoxDoxDotExe
			// 
			this.textBoxDoxDotExe.Location = new System.Drawing.Point(104, 88);
			this.textBoxDoxDotExe.Name = "textBoxDoxDotExe";
			this.textBoxDoxDotExe.Size = new System.Drawing.Size(386, 20);
			this.textBoxDoxDotExe.TabIndex = 45;
			this.textBoxDoxDotExe.Leave += new System.EventHandler(this.textBoxDoxDotExe_Leave);
			// 
			// labelDoxDotExe
			// 
			this.labelDoxDotExe.AutoSize = true;
			this.labelDoxDotExe.Location = new System.Drawing.Point(3, 91);
			this.labelDoxDotExe.Name = "labelDoxDotExe";
			this.labelDoxDotExe.Size = new System.Drawing.Size(42, 13);
			this.labelDoxDotExe.TabIndex = 44;
			this.labelDoxDotExe.Text = "dot.exe";
			// 
			// textBoxDoxMscGenExe
			// 
			this.textBoxDoxMscGenExe.Location = new System.Drawing.Point(104, 123);
			this.textBoxDoxMscGenExe.Name = "textBoxDoxMscGenExe";
			this.textBoxDoxMscGenExe.Size = new System.Drawing.Size(386, 20);
			this.textBoxDoxMscGenExe.TabIndex = 47;
			this.textBoxDoxMscGenExe.Leave += new System.EventHandler(this.textBoxDoxMscGenExe_Leave);
			// 
			// labelDoxMscGenExe
			// 
			this.labelDoxMscGenExe.AutoSize = true;
			this.labelDoxMscGenExe.Location = new System.Drawing.Point(3, 126);
			this.labelDoxMscGenExe.Name = "labelDoxMscGenExe";
			this.labelDoxMscGenExe.Size = new System.Drawing.Size(64, 13);
			this.labelDoxMscGenExe.TabIndex = 46;
			this.labelDoxMscGenExe.Text = "mscgen.exe";
			// 
			// textBoxDoxPlantUmlJar
			// 
			this.textBoxDoxPlantUmlJar.Location = new System.Drawing.Point(104, 157);
			this.textBoxDoxPlantUmlJar.Name = "textBoxDoxPlantUmlJar";
			this.textBoxDoxPlantUmlJar.Size = new System.Drawing.Size(386, 20);
			this.textBoxDoxPlantUmlJar.TabIndex = 49;
			this.textBoxDoxPlantUmlJar.Leave += new System.EventHandler(this.textBoxDoxPlantUmlJar_Leave);
			// 
			// labelDoxPlantUmlJar
			// 
			this.labelDoxPlantUmlJar.AutoSize = true;
			this.labelDoxPlantUmlJar.Location = new System.Drawing.Point(3, 160);
			this.labelDoxPlantUmlJar.Name = "labelDoxPlantUmlJar";
			this.labelDoxPlantUmlJar.Size = new System.Drawing.Size(68, 13);
			this.labelDoxPlantUmlJar.TabIndex = 48;
			this.labelDoxPlantUmlJar.Text = "PlantUML.jar";
			// 
			// DoxConfigUserControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.textBoxDoxPlantUmlJar);
			this.Controls.Add(this.labelDoxPlantUmlJar);
			this.Controls.Add(this.textBoxDoxMscGenExe);
			this.Controls.Add(this.labelDoxMscGenExe);
			this.Controls.Add(this.textBoxDoxDotExe);
			this.Controls.Add(this.labelDoxDotExe);
			this.Controls.Add(this.buttonUpdateCache);
			this.Controls.Add(this.textBoxDoxMimeTexExe);
			this.Controls.Add(this.labelDoxMimeTexExe);
			this.Controls.Add(this.checkBoxConfigFile);
			this.Controls.Add(this.textBoxDoxConfigFile);
			this.Controls.Add(this.checkBoxDoxSource);
			this.Controls.Add(this.textBoxDoxSourceBrowser);
			this.Name = "DoxConfigUserControl";
			this.Size = new System.Drawing.Size(520, 266);
			this.Load += new System.EventHandler(this.DoxConfigUserControl_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button buttonUpdateCache;
		private System.Windows.Forms.TextBox textBoxDoxMimeTexExe;
		private System.Windows.Forms.Label labelDoxMimeTexExe;
		private System.Windows.Forms.CheckBox checkBoxConfigFile;
		private System.Windows.Forms.TextBox textBoxDoxConfigFile;
		private System.Windows.Forms.CheckBox checkBoxDoxSource;
		private System.Windows.Forms.TextBox textBoxDoxSourceBrowser;
		private System.Windows.Forms.TextBox textBoxDoxDotExe;
		private System.Windows.Forms.Label labelDoxDotExe;
		private System.Windows.Forms.TextBox textBoxDoxMscGenExe;
		private System.Windows.Forms.Label labelDoxMscGenExe;
		private System.Windows.Forms.TextBox textBoxDoxPlantUmlJar;
		private System.Windows.Forms.Label labelDoxPlantUmlJar;
	}
}

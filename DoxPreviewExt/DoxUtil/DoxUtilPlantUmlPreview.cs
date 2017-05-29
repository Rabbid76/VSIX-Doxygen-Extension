using System.IO;
using System.Diagnostics;


namespace DoxPreviewExt.DoxUtil
{
	public class CPlantUMLImage : CPreviewImageBase
	{
		private string tempSourcePath_ = "";
		private string tempSourceFileName_ = "";
		private string tempSourceFilePath_ = "";
		private string previewCachePath_ = "";
		private string previewFileExt_ = "png";
		private string previewFileName_ = "";
		private string previewFilePath_ = "";
		private Process imgGenProcess_ = null;

		/// \brief ctor
		public CPlantUMLImage(CPreviewImageCollection collection, COptions options, string sourceCode) : base(collection, options)
		{
			// generate new temporary source file name and path
			this.tempSourcePath_ = this.Options.UmlPaths.Count > 0 ? this.Options.UmlPaths[0] : this.Options.DoxRootPath;
			this.TempFilePath(tempSourcePath_, this.TempName, "plantuml", out this.tempSourceFileName_, out this.tempSourceFilePath_);

			// generate new temporary preview file path and path
			previewCachePath_ = CManager.DoxExtTempPath;
			this.TempFilePath(this.previewCachePath_, this.TempName, this.previewFileExt_, out this.previewFileName_, out this.previewFilePath_);

			// generate preview image from source code 
			Convert(sourceCode);
		}

		public override string GetImageFile()
		{
			return this.previewFilePath_;
		}

		protected override void Validate()
		{
			if (state_ != State.busy)
				return;
			try
			{
				// tests if process has exit
				if (imgGenProcess_.WaitForExit(0) == false)
					return;
				imgGenProcess_ = null;

				// delete temporary source file
				File.Delete(this.tempSourceFilePath_); // If the file to be deleted does not exist, no exception is thrown.

				// test if generation was successful and return preview file
				state_ = File.Exists(this.previewFilePath_) ? State.valid : State.failed;
				return;
			}
			catch { }
			state_ = State.failed;
		}

		public override System.Windows.Controls.Image GetValidImage()
		{
			if (state_ != State.valid)
				return null;
			return CManager.LoadOrCreateBitmapImage(this.previewFilePath_);
		}

		/// \brief generate preview image from source code
		private void Convert(string sourceCode)
		{
			state_ = State.busy;
			try
			{
				// find plantuml.jar
				var plantumljar = Options.PlantUmlJarFile;
				if (plantumljar == null || plantumljar == "")
				{
					state_ = State.failed;
					return;
				}

				// create temporary file with source code
				File.WriteAllText(this.tempSourceFilePath_, sourceCode);
				if (File.Exists(this.tempSourceFilePath_) == false)
				{
					state_ = State.failed;
					return;
				}

				// generate preview file
				// PlantUML Command line [http://plantuml.com/command-line]
				// e.g. java -jar c:\tools\plantuml\plantuml.jar -tsvg test1.plantuml

				//var args = "-jar " + PlantUML_JAR + " -t" + fileExt + " " + tempFileName;
				//var args = "-jar " + PlantUML_JAR + " -t" + previewFileExt + " -I/" + inclPath + "/*.* " + tempFileName;
				var args = "-jar " + plantumljar + " -o \"" + this.previewCachePath_ + "\" -t" + this.previewFileExt_ + " " + this.tempSourceFileName_;
				ProcessStartInfo startInfo = new ProcessStartInfo("java", args);
				startInfo.WorkingDirectory = this.tempSourcePath_;
				startInfo.CreateNoWindow = true;
				startInfo.UseShellExecute = false;
				startInfo.WindowStyle = ProcessWindowStyle.Hidden;
				imgGenProcess_ = Process.Start(startInfo);
				return;
			}
			catch { }
			state_ = State.failed;
		}
	}
}

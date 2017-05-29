using System.IO;
using System.Diagnostics;


namespace DoxPreviewExt.DoxUtil
{
	public class CMscGraphImage : CPreviewImageBase
	{
		private bool sourceIsFile_ = false;
		private string tempSourcePath_ = "";
		private string tempSourceFileName_ = "";
		private string tempSourceFilePath_ = "";
		private string previewCachePath_ = "";
		private string previewFileExt_ = "png";
		private string previewFileName_ = "";
		private string previewFilePath_ = "";
		private Process imgGenProcess_ = null;

		/// \brief ctor
		public CMscGraphImage(CPreviewImageCollection collection, COptions options, bool is_file, string file_path_or_sourceCode) : base(collection, options)
		{
			this.sourceIsFile_ = is_file;

			// generate new temporary source file name and path
			this.tempSourcePath_ = CManager.DoxExtTempPath;
			if (this.sourceIsFile_)
			{
				this.tempSourceFileName_ = "";
				this.tempSourceFilePath_ = file_path_or_sourceCode;
			}
			else
			{
				this.TempFilePath(tempSourcePath_, this.TempName, "dot", out this.tempSourceFileName_, out this.tempSourceFilePath_);
			}

			// generate new temporary preview file path and path
			this.previewCachePath_ = CManager.DoxExtTempPath;
			this.TempFilePath(this.previewCachePath_, this.TempName, this.previewFileExt_, out this.previewFileName_, out this.previewFilePath_);

			// generate preview image from source code 
			Convert(this.sourceIsFile_ ? "" : file_path_or_sourceCode);
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
				if (sourceIsFile_ == false)
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
				var mscgenDir = Options.MscBinPath;
				if (mscgenDir == null || mscgenDir == "")
				{
					state_ = State.failed;
					return;
				}
				if (mscgenDir.EndsWith("/") == false && mscgenDir.EndsWith("\\") == false)
					mscgenDir += "\\";
				var mscgenTool = mscgenDir + "mscgen.exe";

				// create temporary file with source code
				if (sourceIsFile_ == false)
					File.WriteAllText(this.tempSourceFilePath_, sourceCode);
				if (File.Exists(tempSourceFilePath_) == false)
				{
					state_ = State.failed;
					return;
				}

				// generate preview file
				// mscgen - Command-Line Referece [http://msc-generator.sourceforge.net/help/3.7.5/ch05s10.html]
				// e.g mscgen -T png input.dot > output.png
				var args = "-T " + this.previewFileExt_ + " -o \"" + this.previewFilePath_ + "\" \"" + this.tempSourceFilePath_ + "\"";
				ProcessStartInfo startInfo = new ProcessStartInfo(mscgenTool, args); // TODO $$$ Why does this not work? Unicode?
				startInfo.WorkingDirectory = mscgenDir;
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

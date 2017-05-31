using System.IO;
using System.Diagnostics;


namespace DoxPreviewExt.DoxUtil
{
	public class CDotGraphImage : CPreviewImageBase
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
		private string processFilePath_ = "";

		/// \brief ctor
		public CDotGraphImage(CPreviewImageCollection collection, COptions options, bool is_file, string file_path_or_sourceCode) : base(collection, options)
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

			// generate temporary process batch file
			string dummyName;
			this.TempFilePath(this.previewCachePath_, this.TempName, "bat", out dummyName, out this.processFilePath_);

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
				File.Delete(this.processFilePath_); // If the file to be deleted does not exist, no exception is thrown.

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
				// find dot.exe
				var dotGraphTool = this.Options.CurrentDotTool;
				if (dotGraphTool == null || dotGraphTool == "")
				{
					state_ = State.failed;
					return;
				}
				var dotGraphDir = dotGraphTool.Substring(0, dotGraphTool.Length - dotGraphTool.Length);

				// create temporary file with source code
				if (sourceIsFile_ == false)
					File.WriteAllText(this.tempSourceFilePath_, sourceCode);
				if (File.Exists(tempSourceFilePath_) == false)
				{
					state_ = State.failed;
					return;
				}

				// generate preview file
				// Graphviz - Command-line Invocation [http://www.graphviz.org/content/command-line-invocation]
				// e.g dot -Tpng input.dot > output.png
				var args = "-T" + this.previewFileExt_ + " \"" + this.tempSourceFilePath_ + "\" > \"" + this.previewFilePath_ + "\"";
				File.WriteAllText(this.processFilePath_, dotGraphTool + " " + args);
				if (File.Exists(this.processFilePath_) == false)
				{
					state_ = State.failed;
					return;
				}
				ProcessStartInfo startInfo = new ProcessStartInfo(this.processFilePath_);
				//ProcessStartInfo startInfo = new ProcessStartInfo(dotGraphTool, args); // TODO $$$ Why does this not work? Unicode?
				startInfo.WorkingDirectory = dotGraphDir;
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

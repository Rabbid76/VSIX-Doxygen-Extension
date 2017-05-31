using System.IO;
using System.Diagnostics;


namespace DoxPreviewExt.DoxUtil
{
	public class CLatexFormulaImage : CPreviewImageBase
	{
		private bool sourceIsFile_ = false;
		private string tempSourcePath_ = "";
		private string tempSourceFileName_ = "";
		private string tempSourceFilePath_ = "";
		private string previewCachePath_ = "";
		private string previewFileExt_ = "gif";
		private int priveFontSize_ = 5;
		private string previewFileName_ = "";
		private string previewFilePath_ = "";
		private Process imgGenProcess_ = null;

		/// \brief ctor
		public CLatexFormulaImage(CPreviewImageCollection collection, COptions options, bool is_file, string file_path_or_formula) : base(collection, options)
		{
			this.sourceIsFile_ = is_file;

			// generate new temporary source file name and path
			this.tempSourcePath_ = CManager.DoxExtTempPath;
			if (this.sourceIsFile_)
			{
				this.tempSourceFileName_ = "";
				this.tempSourceFilePath_ = file_path_or_formula;
			}
			else
			{
				this.TempFilePath(tempSourcePath_, this.TempName, "latexf", out this.tempSourceFileName_, out this.tempSourceFilePath_);
			}

			// generate new temporary preview file path and path
			this.previewCachePath_ = CManager.DoxExtTempPath;
			this.TempFilePath(this.previewCachePath_, this.TempName, this.previewFileExt_, out this.previewFileName_, out this.previewFilePath_);

			// generate preview image from source code 
			Convert(this.sourceIsFile_ ? "" : file_path_or_formula);
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
				// find mimetex.exe
				var mimeTeXTool = this.Options.CurrentMimeTeXTool;
				if (mimeTeXTool == null || mimeTeXTool == "")
				{
					state_ = State.failed;
					return;
				}
				var mimeTeXDir = mimeTeXTool.Substring(0, mimeTeXTool.Length - mimeTeXTool.Length);

				// create temporary file with source code
				if (sourceIsFile_ == false)
					File.WriteAllText(this.tempSourceFilePath_, sourceCode);
				if (File.Exists(tempSourceFilePath_) == false)
				{
					state_ = State.failed;
					return;
				}

				// generate preview file
				// mimeTeX - Command-Line Referece [http://blog.math.ntu.edu.tw/~history/cgi-bin/mimetex.html#cmdline]
				// e.g 
				//   mimetex.exe -e test.gif "x^2+y^2" -o -s 5
				//   mimetex.exe -e "c:\temp\test.gif" -f "c:\temp\testmimetex.txt" -o -s 5
				var args = "-e \"" + this.previewFilePath_ + "\" -f \"" + this.tempSourceFilePath_ + "\" -o -s " + priveFontSize_.ToString();
				ProcessStartInfo startInfo = new ProcessStartInfo(mimeTeXTool, args);
				startInfo.WorkingDirectory = mimeTeXDir;
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

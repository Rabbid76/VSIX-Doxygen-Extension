using ExtensionCommon;


namespace DoxPreviewExt.DoxUtil
{
	public abstract class CPreviewImageBase : IPreviewImage
	{
		protected enum State { busy, failed, valid };
		protected State state_ = State.busy;

		public abstract string GetImageFile();
		public abstract System.Windows.Controls.Image GetValidImage();
		protected abstract void Validate();

		public CPreviewImageCollection Collection { get; }
		public COptions Options { get; }
		public string TempName { get; }

		/// \brief ctor
		public CPreviewImageBase(CPreviewImageCollection collection, COptions options)
		{
			this.Collection = collection;
			this.Options = options;

			// generate new temporary file name
			this.TempName = collection.NewTempPreviewFileName();
		}

		//! \brief get image accoriding to current state
		public System.Windows.Controls.Image GetImage()
		{
			try
			{
				this.Validate();
				switch (state_)
				{
					case State.busy:
					case State.failed:
						string imgName = ExtensionContext.ConstExtensionImgResName;
						imgName += state_ == State.failed ? "error.png" : "refresh.png";
						System.Reflection.Assembly thisAss = System.Reflection.Assembly.GetExecutingAssembly();
						//var resNames = thisAss.GetManifestResourceNames();
						System.IO.Stream fileStream = thisAss.GetManifestResourceStream(imgName);
						var img = new System.Windows.Controls.Image();
						System.Windows.Media.Imaging.PngBitmapDecoder decoder =
							new System.Windows.Media.Imaging.PngBitmapDecoder(fileStream,
							System.Windows.Media.Imaging.BitmapCreateOptions.PreservePixelFormat,
							System.Windows.Media.Imaging.BitmapCacheOption.Default);
						img.Source = decoder.Frames[0];
						return img;

					case State.valid:
						return this.GetValidImage();
				}
			}
			catch { }
			return null;
		}

		/// \brief Get new temporary file name and path
		protected void TempFilePath(string path, string name, string extension, out string fileName, out string filePath)
		{
			var tempPath = path;
			if (!tempPath.EndsWith("/") && !tempPath.EndsWith("\\"))
				tempPath += "\\";
			fileName = name + "." + extension;
			filePath = tempPath + fileName;
		}
	}
}

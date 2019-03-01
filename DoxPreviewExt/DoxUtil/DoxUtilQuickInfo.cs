using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using System.Windows;


/*!

Walkthrough: Displaying QuickInfo Tooltips
[https://msdn.microsoft.com/en-us/library/ee197646.aspx]


*/

namespace DoxPreviewExt.DoxUtil
{
	//! \brief The provider of the QuickInfo controller serves primarily to export itself as a MEF component part and instantiate the
	//! QuickInfo controller. Because it is a MEF component part, it can import other MEF component parts.
	[Export(typeof(IIntellisenseControllerProvider))]
	[Name("Doxgen Extension QuickInfo Controller")]
	[ContentType("code")]
	internal class CmdQuickInfoControllerProvider : IIntellisenseControllerProvider
	{
		[Import]
		internal IQuickInfoBroker QuickInfoBroker { get; set; }

		public IIntellisenseController TryCreateIntellisenseController(ITextView textView, IList<ITextBuffer> subjectBuffers)
		{
			return new CmdQuickInfoController(textView, subjectBuffers, this);
		}
	}

	// \brief QuickInfo controllers determine when QuickInfo should be displayed.In this example, QuickInfo is displayed when the pointer
	// is over a word that corresponds to one of the method names.The QuickInfo controller implements a mouse hover event handler that
	// triggers a QuickInfo session.
	internal class CmdQuickInfoController : IIntellisenseController
	{
		private ITextView m_textView;
		private IList<ITextBuffer> m_subjectBuffers;
		private CmdQuickInfoControllerProvider m_provider;
		private IQuickInfoSession m_session;

		internal CmdQuickInfoController(ITextView textView, IList<ITextBuffer> subjectBuffers, CmdQuickInfoControllerProvider provider)
		{
			m_textView = textView;
			m_subjectBuffers = subjectBuffers;
			m_provider = provider;

			m_textView.MouseHover += this.OnTextViewMouseHover;
		}

		private void OnTextViewMouseHover(object sender, MouseHoverEventArgs e)
		{
			//find the mouse position by mapping down to the subject buffer
			SnapshotPoint? point = m_textView.BufferGraph.MapDownToFirstMatch(
					new SnapshotPoint(m_textView.TextSnapshot, e.Position),
					PointTrackingMode.Positive,
					snapshot => m_subjectBuffers.Contains(snapshot.TextBuffer),
					PositionAffinity.Predecessor);

			if (point == null)
				return;

			ITrackingPoint triggerPoint = point.Value.Snapshot.CreateTrackingPoint(point.Value.Position, PointTrackingMode.Positive);
			if (!m_provider.QuickInfoBroker.IsQuickInfoActive(m_textView))
			{
				m_session = m_provider.QuickInfoBroker.TriggerQuickInfo(m_textView, triggerPoint, true);
			}
		}

		public void Detach(ITextView textView)
		{
			if (m_textView == textView)
			{
				m_textView.MouseHover -= this.OnTextViewMouseHover;
				m_textView = null;
			}
		}

		public void ConnectSubjectBuffer(ITextBuffer subjectBuffer)
		{
		}

		public void DisconnectSubjectBuffer(ITextBuffer subjectBuffer)
		{
		}
	}

	//! \brief The provider of the QuickInfo source serves primarily to export itself as a MEF component part and instantiate the
	//! QuickInfo source. Because it is a MEF component part, it can import other MEF component parts.
	[Export(typeof(IQuickInfoSourceProvider))]
	[Name("Doxygen Extension QuickInfo Source")]
	[Order(Before = "Default Quick Info Presenter")]
	//[Order(After = "Default Quick Info Presenter")]
	//[ContentType("code")]
	[ContentType("C/C++"), ContentType("CSharp"), ContentType("DoxCode")]
	internal class CmdQuickInfoSourceProvider : IQuickInfoSourceProvider
	{
		[Import]
		internal ITextStructureNavigatorSelectorService NavigatorService { get; set; }

		[Import]
		internal ITextBufferFactoryService TextBufferFactoryService { get; set; }

		[Import]
		public IClassificationTypeRegistryService ClassificationTypeRegistry { get; set; }

		[Import]
		internal IBufferTagAggregatorFactoryService AggregatorFactory { get; set; }

		public IQuickInfoSource TryCreateQuickInfoSource(ITextBuffer buffer)
		{
			ITagAggregator<DoxTokenTag> doxTagAggregator = AggregatorFactory.CreateTagAggregator<DoxTokenTag>(buffer);
			return new CmdQuickInfoSource(this, doxTagAggregator, buffer, ClassificationTypeRegistry);
			//return buffer.Properties.GetOrCreateSingletonProperty(() =>
			//	new CmdQuickInfoSource(this, doxTagAggregator, buffer, ClassificationTypeRegistry)) as IQuickInfoSource;
		}
	}

	//! \brief The QuickInfo source is responsible for collecting the set of identifiers and their descriptions and adding the content
	//! to the tooltip text buffer when one of the identifiers is encountered. In this example, the identifiers and their descriptions
	//! are just added in the source constructor.
	internal class CmdQuickInfoSource : IQuickInfoSource
	{
		CmdQuickInfoSourceProvider Provider { get; set; }
		ITagAggregator<DoxTokenTag> Aggregator { get; set; }
		ITextBuffer SubjectBuffer { get; set; }
		IClassificationTypeRegistryService ClassificationRegistry { get; set; }

		public CmdQuickInfoSource(CmdQuickInfoSourceProvider provider, ITagAggregator<DoxTokenTag> aggregator, ITextBuffer subjectBuffer, IClassificationTypeRegistryService classificationRegistry)
		{
			Provider = provider;
			Aggregator = aggregator;
			SubjectBuffer = subjectBuffer;
			ClassificationRegistry = classificationRegistry;
		}

		public void NotyfieMouseLeftBtnUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			System.Windows.Documents.Run srcRun = e.Source as System.Windows.Documents.Run;
			if (srcRun == null)
				return;
			var contentText = srcRun.Text;
			if (contentText.ToLower().StartsWith("http") || contentText.ToLower().StartsWith("file:///"))
			{
				string doxURL = contentText;
				DoxUtil.CManager.OpenHTML(doxURL);
			}
		}

		//! \brief create Quickinfo content
		public void AugmentQuickInfoSession(IQuickInfoSession session, IList<object> qiContent, out ITrackingSpan applicableToSpan)
		{
			applicableToSpan = null;

			DoxUtil.CManager manager = DoxUtil.CManager.Manager;
			if (manager == null)
				return;

			// Map the trigger point down to our buffer.
			SnapshotPoint? subjectTriggerPoint = session.GetTriggerPoint(SubjectBuffer.CurrentSnapshot);
			if (!subjectTriggerPoint.HasValue)
			  return;

			ITextSnapshot currentSnapshot = subjectTriggerPoint.Value.Snapshot;
			DoxTokenTag spanInfo = DoxTokenTaggerHelper.TagHitTest(this.Aggregator, currentSnapshot, subjectTriggerPoint.Value);
      if (spanInfo == null)
				return;

			bool isCmdKnown = spanInfo.Type != DoxTokenType.Cmd || spanInfo.WellKnown;
			string cmdName = spanInfo.Name;
			if (spanInfo.Name.Length > 0 && (spanInfo.Name[0] == '@' || spanInfo.Name[0] == '\\'))
				cmdName = spanInfo.Name.Substring(1);

			var opt = manager.Options;
			var colDflt = Color.FromRgb(128, 128, 128);
			var colCmd  = CmdClassifications.CmdColor;
			var colHRef = CmdClassifications.RefColor;
			var colInc  = CmdClassifications.IncludeColor;
			/*
			try
			{
				var cmdClassify = ClassificationRegistry.GetClassificationType(CmdClassifications.DoxCommand);
				if (cmdClassify!=null)
				{
					
				}
			}
			catch { }
			*/

			if (opt.GeneralQT)
			{
				// C# String Formatting Dictionary Intellisence
				// [http://stackoverflow.com/questions/39524387/c-sharp-string-formatting-dictionary-intellisence]
				var textBlock = new System.Windows.Controls.TextBlock { TextWrapping = System.Windows.TextWrapping.NoWrap };
				if (isCmdKnown)
				{
					AddText(textBlock, colDflt, false, false, "Doxygen Special Command  ");
					AddText(textBlock, colCmd, true, false, "@" + cmdName);
					AddText(textBlock, colDflt, false, false, "  [");
					AddText(textBlock, colHRef, false, true, manager.Options.DoxCommadsHelpURL + "#cmd" + cmdName);
					AddText(textBlock, colDflt, false, false, "]");
				}
				else
				{
					AddText(textBlock, colDflt, false, false, "Unknown Doxygen Special Command  ");
					AddText(textBlock, colDflt, true, false, "@" + cmdName);
					AddText(textBlock, colDflt, false, false, ", see [");
					AddText(textBlock, colHRef, false, true, manager.Options.DoxCommadsHelpURL);
					AddText(textBlock, colDflt, false, false, "] for more information");
				}
				qiContent.Add(textBlock);
			}

			switch (spanInfo.Type)
			{
				case DoxTokenType.Cmd:
					// nothing to do
					break;

				case DoxTokenType.Ref:
					if (opt.RefQT)
					{
						var refTextBlock = new System.Windows.Controls.TextBlock { TextWrapping = System.Windows.TextWrapping.NoWrap };
						if (spanInfo.WellKnown)
						{
							AddText(refTextBlock, colDflt, false, false, "[");
							AddText(refTextBlock, colHRef, false, true, spanInfo.Argument);
							AddText(refTextBlock, colDflt, false, false, "]");
						}
						else
						{
							AddText(refTextBlock, colDflt, false, false, "unknown doxgen reference  ");
							AddText(refTextBlock, colDflt, true, false, "spanInfo.Argument");
						}
						qiContent.Add(refTextBlock);
					}
					break;

				case DoxTokenType.Image:
					if (opt.ImageQT)
					{
						if (spanInfo.WellKnown)
						{
							var imagePath = spanInfo.Argument;
							AddImage(qiContent, imagePath);

							qiContent.Add(CreateTextBlockHRef(colDflt, colHRef, "image file", "file:///" + imagePath));
            }
						else
						{
							var imgTextBlock = new System.Windows.Controls.TextBlock { TextWrapping = System.Windows.TextWrapping.NoWrap };
							AddText(imgTextBlock, colDflt, false, false, "unknown image file  ");
							AddText(imgTextBlock, colDflt, true, false, spanInfo.Argument);
							qiContent.Add(imgTextBlock);
							//var hlpURL = manager.FindDoxygenLink(DoxUtil.CManager.DoxRefAddImage, false);
							//if (hlpURL != "")
							//	qiContent.Add(CreateTextBlockHRef(colDflt, colHRef, "How to add an image to source code documentation", hlpURL));
						}
					}
					break;

				case DoxTokenType.Dot:
					if (opt.DotQT)
					{
						// create preview
						string dotContent = spanInfo.ContentSpan.GetText();
						var previewImg = manager.GenerateDotGraphPreview(dotContent);
						if (previewImg != null)
							qiContent.Add(previewImg);

						// add help URL
						qiContent.Add(CreateTextBlockHRef(colDflt, colHRef, "Graphviz - Graph Visualization Software", DoxUtil.CManager.GraphvizHelpURL));
					}
					break;

				case DoxTokenType.DotFile:
					if (opt.DotQT)
					{
						var dotTextBlock = new System.Windows.Controls.TextBlock { TextWrapping = System.Windows.TextWrapping.NoWrap };
						if (spanInfo.WellKnown)
						{
							var dotPath = spanInfo.Argument;
							var previewImg = manager.GenerateDotFileGraphPreview(dotPath);
							if (previewImg != null)
								qiContent.Add(previewImg);
						}
						else
						{
							AddText(dotTextBlock, colDflt, false, false, "unknown dot graph file  ");
							AddText(dotTextBlock, colDflt, true, false, spanInfo.Argument);
						}
						qiContent.Add(dotTextBlock);
					}
					break;

				case DoxTokenType.Msc:
					if (opt.MscQT)
					{
						// create preview
						string mscContent = spanInfo.ContentSpan.GetText();
						var previewImg = manager.GenerateMscGraphPreview("msc {\n" + mscContent + "\n}\n");
						if (previewImg != null)
							qiContent.Add(previewImg);

						// add help URL
						qiContent.Add(CreateTextBlockHRef(colDflt, colHRef, "Mscgen", DoxUtil.CManager.MscgenHelpURL));
					}
					break;

				case DoxTokenType.MscFile:
					if (opt.MscQT)
					{
						var mscTextBlock = new System.Windows.Controls.TextBlock { TextWrapping = System.Windows.TextWrapping.NoWrap };
						if (spanInfo.WellKnown)
						{
							var mscPath = spanInfo.Argument;
							var previewImg = manager.GenerateMscFileGraphPreview(mscPath);
							if (previewImg != null)
								qiContent.Add(previewImg);
						}
						else
						{
							AddText(mscTextBlock, colDflt, false, false, "unknown dot graph file  ");
							AddText(mscTextBlock, colDflt, true, false, spanInfo.Argument);
						}
						qiContent.Add(mscTextBlock);
					}
					break;

				case DoxTokenType.DiaFile:
					// TODO $$$ not yet implemented
					break;

				case DoxTokenType.Uml:
					if (opt.PlantUmlQT)
					{
						// create preview
						string umlContent = spanInfo.ContentSpan.GetText();
						var previewImg = manager.GeneratePlatUMLPreview("@startuml\n" + umlContent + "\n@enduml");
						if (previewImg != null)
							qiContent.Add(previewImg);

						// add help URL
						qiContent.Add(CreateTextBlockHRef(colDflt, colHRef, "PlantUML in a nutshell", DoxUtil.CManager.PlanUMLHelpURL));
					}
					break;

				case DoxTokenType.LatexFormula:
					if (opt.LatexFormulaQT)
					{
						// create preview
						string latexFormula = spanInfo.ContentSpan.GetText();
						var previewImg = manager.GenerateLatexFormulaPreview(latexFormula);
						if (previewImg != null)
							qiContent.Add(previewImg);

						// add help URL
						qiContent.Add(CreateTextBlockHRef(colDflt, colHRef, "LaTeX/Mathematics", DoxUtil.CManager.LatexHelpURL));
					}
					break;
			}

			if (qiContent.Count > 0)
			  applicableToSpan = currentSnapshot.CreateTrackingSpan(spanInfo.Span, SpanTrackingMode.EdgeInclusive);
		}

		//! \brief create text block with URL
		private System.Windows.Controls.TextBlock CreateTextBlockHRef(Color colDflt, Color colHRef, string text, string url)
		{
			var textBlock = new System.Windows.Controls.TextBlock { TextWrapping = System.Windows.TextWrapping.NoWrap };
			AddText(textBlock, colDflt, false, false, text + " [");
			AddText(textBlock, colHRef, false, true, url);
			AddText(textBlock, colDflt, false, false, "]");
			return textBlock;
		}

		//! \brief Add text contnet to TextBlock of Qickinfo 
		private void AddText(System.Windows.Controls.TextBlock textBlock, Color col, bool bold, bool underline, string text)
		{
			var run = new System.Windows.Documents.Run(text);

			run.MouseLeftButtonUp += this.NotyfieMouseLeftBtnUp;

			run.Foreground = new SolidColorBrush(col);
			if (bold)
				run.FontWeight = System.Windows.FontWeights.Bold;
			if (underline)
			{
				// add collection of text decorations 
				run.TextDecorations = new TextDecorationCollection();

				// create undeline text decoration
				TextDecoration underlineDecoration = new TextDecoration();
				Pen underlinePen = new Pen();
				underlinePen.Brush = new SolidColorBrush(col);
				underlinePen.Thickness = 1.5;
				underlinePen.DashStyle = DashStyles.Solid;
				underlineDecoration.Pen = underlinePen;
				underlineDecoration.PenThicknessUnit = TextDecorationUnit.FontRecommended;
				run.TextDecorations.Add(underlineDecoration);
			}
			textBlock.Inlines.Add(run);
		}


		//! \brief Add image preview to QuickInfo content
		void AddImage(IList<object> qiContent, string imagePath)
		{
			var img = CManager.LoadOrCreateBitmapImage(imagePath);
			if (img != null)
				qiContent.Add(img);

			// <img src="refl1_non.jpg" alt="refl1_non.jpg"/>

			//var web = new System.Windows.Controls.WebBrowser();
			//web.Width = bmp.Width;
			//web.Height = bmp.Height;
			//string webContent = "<body><p><b>Test</b></p></body>";
			//string webContent = "<object type = \"image/svg+xml\" data = \"" + "file:///" + imagePath + "\"></object>";
			//web.NavigateToString(webContent);
			//web.Source = new Uri("file:///" + imagePath);
			//web.Navigate("file:///" + imagePath);
			//qiContent.Add(web);
		}

		private bool m_isDisposed;
		public void Dispose()
		{
			if (!m_isDisposed)
			{
				GC.SuppressFinalize(this);
				m_isDisposed = true;
			}
		}
	}
}

using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Text.Editor;
using System.Windows.Controls;


namespace DoxPreviewExt.DoxUtil
{
	//! \brief interactive mouse processor provider for text views
	[Export(typeof(IMouseProcessorProvider))]
	[Name("doxgen reference mouse processor")]
	//[ContentType("code")]
	[ContentType("C/C++"), ContentType("CSharp"), ContentType("DoxCode")]
	[TextViewRole(PredefinedTextViewRoles.Interactive)]
	internal sealed class CMouseProcessorProvider : IMouseProcessorProvider
	{
		[Import]
		internal IBufferTagAggregatorFactoryService AggregatorFactory { get; set; }

		public IMouseProcessor GetAssociatedProcessor(IWpfTextView wpfTextView)
		{
			ITagAggregator<DoxTokenTag> doxTagAggregator = AggregatorFactory.CreateTagAggregator<DoxTokenTag>(wpfTextView.TextBuffer);
			return wpfTextView.Properties.GetOrCreateSingletonProperty(() =>
				new CMouseProcessor(doxTagAggregator, wpfTextView)) as IMouseProcessor;
		}

		//! \brief interactive mouse processor for text views
		public class CMouseProcessor : IMouseProcessor
		{
			ITagAggregator<DoxTokenTag> Aggregator { get; set; }
			IWpfTextView WpfTextView { get; set; }
			
			Microsoft.VisualStudio.Text.Editor.MouseHoverEventArgs HoverArgs { get; set; }

			public CMouseProcessor(ITagAggregator<DoxTokenTag> aggregator, IWpfTextView wpfTextView)
			{
				this.Aggregator = aggregator;
				this.WpfTextView = wpfTextView;
				this.WpfTextView.MouseHover += this.WpfTextViewMouseHover;
			}

			public void WpfTextViewMouseHover(object sender, Microsoft.VisualStudio.Text.Editor.MouseHoverEventArgs args)
			{
				HoverArgs = args;
			}

			private DoxTokenTag FindSpanTag(System.Windows.Input.MouseEventArgs args)
			{
				try
				{
					// find canavas
					var over = System.Windows.Input.Mouse.PrimaryDevice.DirectlyOver;
					Canvas canvas = over as Canvas;
					if (canvas == null)
					{
						System.Windows.FrameworkElement elem = over as System.Windows.FrameworkElement;
						while (elem != null && elem.Parent != null)
						{
							canvas = elem as Canvas;
							if (canvas != null)
								break;
							elem = elem.Parent as System.Windows.FrameworkElement;
						}
					}
					if (canvas == null)
						return null;

					// get mouse position relative to canavas
					System.Windows.Point pos = args.GetPosition(canvas);
					// HitTestResult hitRes = VisualTreeHelper.HitTest(canvas, pos);

					// find the line where the mouse hovers
					var lines = this.WpfTextView.TextViewLines;
					Microsoft.VisualStudio.Text.Formatting.ITextViewLine hitLine = null;
					foreach (var line in lines)
					{
						if (line.Left < pos.X && line.Right > pos.X && line.Top < pos.Y && line.Bottom > pos.Y)
						{
							hitLine = line;
							break;
						}
					}
					if (hitLine == null)
						return null;

					// find the positon in the line where the mouse hovers
					SnapshotPoint? snapPt = hitLine.GetBufferPositionFromXCoordinate(pos.X);
					if (snapPt == null)
						return null;

					// find doxygen tag
					DoxTokenTag spanInfo = DoxTokenTaggerHelper.TagHitTest(this.Aggregator, this.WpfTextView.TextSnapshot, snapPt.Value);
					return spanInfo;
				}
				catch { }
				return null;
			}

			private void OpenDoxReference(System.Windows.Input.MouseEventArgs args)
			{
				DoxUtil.CManager manager = DoxUtil.CManager.Manager;
				if (manager == null)
					return;
				if (manager.Options.ClickOnRef == false)
					return;

				DoxTokenTag spanTag = FindSpanTag(args);
				if (spanTag == null)
					return;

				if (spanTag.Type == DoxTokenType.Ref && spanTag.WellKnown)
				{
					var doxURL = spanTag.Argument;
					DoxUtil.CManager.OpenHTML(doxURL);
				}
			}

			public void PostprocessDragEnter(System.Windows.DragEventArgs args) { }
			public void PostprocessDragLeave(System.Windows.DragEventArgs args) { }
			public void PostprocessDragOver(System.Windows.DragEventArgs args) { }
			public void PostprocessDrop(System.Windows.DragEventArgs args) { }
			public void PostprocessGiveFeedback(System.Windows.GiveFeedbackEventArgs args) { }
			public void PostprocessMouseDown(System.Windows.Input.MouseButtonEventArgs args) { }
			public void PostprocessMouseEnter(System.Windows.Input.MouseEventArgs args) { }
			public void PostprocessMouseLeave(System.Windows.Input.MouseEventArgs args) { }
			public void PostprocessMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs args) { }
			public void PostprocessMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs args) { }
			public void PostprocessMouseMove(System.Windows.Input.MouseEventArgs args) { }
			public void PostprocessMouseRightButtonDown(System.Windows.Input.MouseButtonEventArgs args) { }
			public void PostprocessMouseRightButtonUp(System.Windows.Input.MouseButtonEventArgs args) { }
			public void PostprocessMouseUp(System.Windows.Input.MouseButtonEventArgs args) { }
			public void PostprocessMouseWheel(System.Windows.Input.MouseWheelEventArgs args) { }
			public void PostprocessQueryContinueDrag(System.Windows.QueryContinueDragEventArgs args) { }
			public void PreprocessDragEnter(System.Windows.DragEventArgs args) { }
			public void PreprocessDragLeave(System.Windows.DragEventArgs args) { }
			public void PreprocessDragOver(System.Windows.DragEventArgs args) { }
			public void PreprocessDrop(System.Windows.DragEventArgs args) { }
			public void PreprocessGiveFeedback(System.Windows.GiveFeedbackEventArgs args) { }
			public void PreprocessMouseDown(System.Windows.Input.MouseButtonEventArgs args) { }
			public void PreprocessMouseEnter(System.Windows.Input.MouseEventArgs args) { }
			public void PreprocessMouseLeave(System.Windows.Input.MouseEventArgs args) { }
			public void PreprocessMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs args) { }
			public void PreprocessMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs args) { OpenDoxReference(args); }
			public void PreprocessMouseMove(System.Windows.Input.MouseEventArgs args) { }
			public void PreprocessMouseRightButtonDown(System.Windows.Input.MouseButtonEventArgs args) { }
			public void PreprocessMouseRightButtonUp(System.Windows.Input.MouseButtonEventArgs args) { }
			public void PreprocessMouseUp(System.Windows.Input.MouseButtonEventArgs args) { }
			public void PreprocessMouseWheel(System.Windows.Input.MouseWheelEventArgs args) { }
			public void PreprocessQueryContinueDrag(System.Windows.QueryContinueDragEventArgs args) { }
		}
	}
}

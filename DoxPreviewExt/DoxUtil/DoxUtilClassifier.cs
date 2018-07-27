using System;
using System.Windows;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Text.Editor;


/*!
 
[http://stackoverflow.com/questions/37579509/vsix-iclassifier-assigning-multiple-classificationtypes]
VSIX IClassifier assigning multiple ClassificationTypes:

- A classifier (really, an `IClassificationTag` tagger) yields classification tag-spans for a given section
  of a text buffer on demand.

- Classification tag-spans consist of the span in the buffer that the tag applies to, and the classification tag itself.
  The classification tag simply specifies a classification type to apply.

- Classification types are used to relate tags of that classification to a given format.

- Formats (specifically, `ClassificationFormatDefinition`s) are exported via MEF (Managed Extensibility Framework)
  (as `EditorFormatDefinitions`) so that VS can discover them and use them to colour spans that have the associated
	classification type. They also (optionally) appear in the Fonts & Colors options.

- A classifier provider is exported via MEF (Managed Extensibility Framework) in order for Visual Studio  to discover it;
  it gives Visual Studio a means of instantiating your classifier for each open buffer (and thus discovering the tags in it).


\attention Clear the contents of the cache before starting debugging (e.g. C:\Users\Gernot\AppData\Local\Microsoft\VisualStudio\15.0_ff39e235Exp\ComponentModelCache)
 
pink   : Color.FromRgb(0xFF, 0x69, 0xB4);  
maroon : Color.FromRgb(0xB0, 0x30, 0x60);

[ContentType("C/C++"), ContentType("CSharp")]
	

- `[UserVisible(true)]` \n
  Controls whether it appears in Fonts & Colors options for user configuration
- `[Name(DoxCommand)]` \n
  This could be anything but I like to reuse the classification type name
- `[Order(After = Priority.Default, Before = Priority.High)]` \n
  Optionally include this attribute if your classification should take precedence over some of the builtin ones like keywords

*/

namespace DoxPreviewExt.DoxUtil
{
	//! \brief doxygen reference format definitions
	public static class CmdClassifications
	{
		public static Color CmdColor                 = Color.FromRgb(90, 160, 200); // Colors.Blue;
		public static Color RefColor                 = Color.FromRgb(120, 210, 230); // Color.SkyBlue;
		public static Color IncludeColor             = Color.FromRgb(90, 160, 200); // Color.Blue
		public static Color DisableColor             = Color.FromRgb(128, 128, 128); // Color.Gray
		public static Color CodeContentColor         = Color.FromRgb(0xF0, 0x75, 0x75); // orange
		public static Color DotContentColor          = Color.FromRgb(0xB0, 0x45, 0x75); 
		public static Color MscContentColor          = Color.FromRgb(0xB0, 0x50, 0x70); 
		public static Color UMLContentColor          = Color.FromRgb(0xB0, 0x40, 0x80); 
		public static Color LatexFormulaContentColor = Color.FromRgb(0x60, 0xC0, 0xA0); 

		// These are the strings that will be used to form the classification types
		// and bind those types to formats
		public const string DoxCommand             = "DoxPreviewExt/CommandVar";
		public const string DoxReference           = "DoxPreviewExt/ReferenceVar";
		public const string DoxIncludeFile         = "DoxPreviewExt/IncludeFileVar";
		public const string DoxUnknownCommand      = "DoxPreviewExt/UnknownCommandVar";
		public const string DoxUnknownReference    = "DoxPreviewExt/UnknownReferenceVar";
		public const string DoxUnknownIncludeFile  = "DoxPreviewExt/UnknownIncludeFileVar";
		public const string DoxCodeContent         = "DoxPreviewExt/CodeContentVar";
		public const string DoxDotContent          = "DoxPreviewExt/DotContentVar";
		public const string DoxMscContent          = "DoxPreviewExt/MscContentVar";
		public const string DoxPlantUMLContent     = "DoxPreviewExt/PlantUMLContentVar";
		public const string DoxLatexFormulaContent = "DoxPreviewExt/LatexFormulaContentVar";

		// These MEF exports define the types themselves
		[Export]
		[Name(DoxCommand)]
		private static ClassificationTypeDefinition DoxCommandType = null;
		[Export]
		[Name(DoxReference)]
		private static ClassificationTypeDefinition DoxReferenceType = null;
		[Export]
		[Name(DoxIncludeFile)]
		private static ClassificationTypeDefinition DoxIncludeFileType = null;
		[Export]
		[Name(DoxUnknownCommand)]
		private static ClassificationTypeDefinition DoxUnknownCommandType = null;
		[Export]
		[Name(DoxUnknownReference)]
		private static ClassificationTypeDefinition DoxUnknownReferenceType = null;
		[Export]
		[Name(DoxUnknownIncludeFile)]
		private static ClassificationTypeDefinition DoxUnknownIncludeFileType = null;
		[Export]
		[Name(DoxCodeContent)]
		private static ClassificationTypeDefinition DoxCodeContentType = null;
		[Export]
		[Name(DoxDotContent)]
		private static ClassificationTypeDefinition DoxDotContentType = null;
		[Export]
		[Name(DoxMscContent)]
		private static ClassificationTypeDefinition DoxMscContentType = null;
		[Export]
		[Name(DoxPlantUMLContent)]
		private static ClassificationTypeDefinition DoxPlantUMLContentType = null;
		[Export]
		[Name(DoxLatexFormulaContent)]
		private static ClassificationTypeDefinition DoxLatexFormulaContentType = null;

		/// \brief content type for .dox files derived from content type "code"
		[Export]
		[Name("DoxCode")]
		[BaseDefinition("code")]
		internal static ContentTypeDefinition DOXContentTypeDefinition = null;

		/// \brief associate file extension .dox to content type "DoxCode"
		[Export]
		[FileExtension(".dox")]
		[ContentType("DoxCode")]
		internal static FileExtensionToContentTypeDefinition DOXFileType = null;


		[Export(typeof(EditorFormatDefinition))]
		[ClassificationType(ClassificationTypeNames = DoxCommand)]
		[UserVisible(true)]
		[Name(DoxCommand)]
		[Order(After = Priority.High)]
		public sealed class CommandFormatDefinition : ClassificationFormatDefinition
		{
			public CommandFormatDefinition()
			{
				ForegroundColor = CmdColor;
				DisplayName = "Doxygen Command  @...";
			}
		}

		// These are the format definitions that specify how things will look
		[Export(typeof(EditorFormatDefinition))]
		[ClassificationType(ClassificationTypeNames = DoxReference)]
		[UserVisible(true)]
		[Name(DoxReference)]
		[Order(After = Priority.High)]
		public sealed class ReferenceFormatDefinition : ClassificationFormatDefinition
		{
			public ReferenceFormatDefinition()
			{
				this.DisplayName = "Doxygen Reference  @ref";

				var forgroundColor = RefColor;

				this.ForegroundColor = forgroundColor;

				// add collection of text decorations 
				this.TextDecorations = new TextDecorationCollection();

				// create undeline text decoration
				TextDecoration underlineDecoration = new TextDecoration();
				Pen underlinePen = new Pen();
				underlinePen.Brush = new SolidColorBrush(forgroundColor);
				underlinePen.Thickness = 1.5;
				underlinePen.DashStyle = DashStyles.Solid;
				underlineDecoration.Pen = underlinePen;
				underlineDecoration.PenThicknessUnit = TextDecorationUnit.FontRecommended;
				this.TextDecorations.Add(underlineDecoration);
			}
		}

		[Export(typeof(EditorFormatDefinition))]
		[ClassificationType(ClassificationTypeNames = DoxIncludeFile)]
		[UserVisible(true)]
		[Name(DoxIncludeFile)]
		[Order(After = Priority.High)]
		public sealed class ImageFormatDefinition : ClassificationFormatDefinition
		{
			public ImageFormatDefinition()
			{
				ForegroundColor = IncludeColor;
				DisplayName = "Doxygen File Reference  @image";
			}
		}

		[Export(typeof(EditorFormatDefinition))]
		[ClassificationType(ClassificationTypeNames = DoxUnknownCommand)]
		[UserVisible(true)]
		[Name(DoxUnknownCommand)]
		[Order(After = Priority.High)]
		public sealed class UnknownCommandFormatDefinition : ClassificationFormatDefinition
		{
			public UnknownCommandFormatDefinition()
			{
				ForegroundColor = DisableColor;
				DisplayName = "Doxygen unknown Command  @...";
			}
		}

		// These are the format definitions that specify how things will look
		[Export(typeof(EditorFormatDefinition))]
		[ClassificationType(ClassificationTypeNames = DoxUnknownReference)]
		[UserVisible(true)]
		[Name(DoxUnknownReference)]
		[Order(After = Priority.High)]
		public sealed class UnknownReferenceFormatDefinition : ClassificationFormatDefinition
		{
			public UnknownReferenceFormatDefinition()
			{
				ForegroundColor = DisableColor;
				DisplayName = "Doxygen unknown Reference  @ref";
			}
		}

		[Export(typeof(EditorFormatDefinition))]
		[ClassificationType(ClassificationTypeNames = DoxUnknownIncludeFile)]
		[UserVisible(true)]
		[Name(DoxUnknownIncludeFile)]
		[Order(After = Priority.High)]
		public sealed class UnknownImageFormatDefinition : ClassificationFormatDefinition
		{
			public UnknownImageFormatDefinition()
			{
				ForegroundColor = DisableColor;
				DisplayName = "Doxygen unknown File Reference  @image";
			}
		}

		[Export(typeof(EditorFormatDefinition))]
		[ClassificationType(ClassificationTypeNames = DoxCodeContent)]
		[UserVisible(true)]
		[Name(DoxCodeContent)]
		[Order(After = Priority.High)]
		public sealed class CodeContentFormatDefinition : ClassificationFormatDefinition
		{
			public CodeContentFormatDefinition()
			{
				ForegroundColor = CodeContentColor;
				DisplayName = "Doxygen Code  @code ... @endcode";
			}
		}

		[Export(typeof(EditorFormatDefinition))]
		[ClassificationType(ClassificationTypeNames = DoxDotContent)]
		[UserVisible(true)]
		[Name(DoxDotContent)]
		[Order(After = Priority.High)]
		public sealed class DotContentFormatDefinition : ClassificationFormatDefinition
		{
			public DotContentFormatDefinition()
			{
				ForegroundColor = DotContentColor;
				DisplayName = "Doxygen Dot Graph  @dot ... @enddot";
			}
		}

		[Export(typeof(EditorFormatDefinition))]
		[ClassificationType(ClassificationTypeNames = DoxMscContent)]
		[UserVisible(true)]
		[Name(DoxMscContent)]
		[Order(After = Priority.High)]
		public sealed class MscContentFormatDefinition : ClassificationFormatDefinition
		{
			public MscContentFormatDefinition()
			{
				ForegroundColor = MscContentColor;
				DisplayName = "Doxygen Message Sequence Chart  @msc ... @endmsc";
			}
		}

		[Export(typeof(EditorFormatDefinition))]
		[ClassificationType(ClassificationTypeNames = DoxPlantUMLContent)]
		[UserVisible(true)]
		[Name(DoxPlantUMLContent)]
		[Order(After = Priority.High)]
		public sealed class PlantUMLContentFormatDefinition : ClassificationFormatDefinition
		{
			public PlantUMLContentFormatDefinition()
			{
				ForegroundColor = UMLContentColor;
				DisplayName = "Doxygen Plant UML  @startuml ... @enduml";
			}
		}

		[Export(typeof(EditorFormatDefinition))]
		[ClassificationType(ClassificationTypeNames = DoxLatexFormulaContent)]
		[UserVisible(true)]
		[Name(DoxLatexFormulaContent)]
		[Order(After = Priority.High)]
		public sealed class LatexFormulaContentFormatDefinition : ClassificationFormatDefinition
		{
			public LatexFormulaContentFormatDefinition()
			{
				ForegroundColor = LatexFormulaContentColor;
				DisplayName = "Doxygen Latex Formula  @f$ ... @f$ / @f[ ... @f]";
			}
		}
	}

	/// \brief Classification provider 
	[Export(typeof(IViewTaggerProvider))]
	//[ContentType("code")]
	[ContentType("C/C++"), ContentType("CSharp"), ContentType("DoxCode")]
	[TagType(typeof(ClassificationTag))]
	public class CmdClassifierProvider : IViewTaggerProvider
	{
		[Import]
		public IClassificationTypeRegistryService ClassificationTypeRegistry { get; set; }
		[Import]
		internal IClassifierAggregatorService AggregatorService { get; set; }
		[Import]
		internal IBufferTagAggregatorFactoryService AggregatorFactory { get; set; }

		public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag
		{
			ITagAggregator<DoxTokenTag> doxTagAggregator = AggregatorFactory.CreateTagAggregator<DoxTokenTag>(buffer);
			return buffer.Properties.GetOrCreateSingletonProperty(() => 
			  new CmdClassificationTagger(doxTagAggregator, textView, buffer, ClassificationTypeRegistry)) as ITagger<T>;
		}
	}

	/// \brief Classification tagger
	public class CmdClassificationTagger : ITagger<ClassificationTag>
	{
		ITagAggregator<DoxTokenTag> Aggregator { get; set; }
		ITextView TextView { get; set; }
		ITextBuffer SubjectBuffer { get; set; }
		IClassificationTypeRegistryService ClassificationRegistry { get; set; }

		public event EventHandler<SnapshotSpanEventArgs> TagsChanged
		{
			add { }
			remove { }
		}

		private Dictionary<string, ClassificationTag> _tags;

		public CmdClassificationTagger(ITagAggregator<DoxTokenTag> aggregator, ITextView textView, ITextBuffer subjectBuffer, IClassificationTypeRegistryService classificationRegistry)
		{
			this.Aggregator = aggregator;
			this.TextView = textView;
			this.SubjectBuffer = subjectBuffer;
			this.ClassificationRegistry = classificationRegistry;
			
			// Build the tags that correspond to each of the possible classifications
			_tags = new Dictionary<string, ClassificationTag> {
				{ CmdClassifications.DoxCommand,             BuildTag(classificationRegistry, CmdClassifications.DoxCommand) },
				{ CmdClassifications.DoxReference,           BuildTag(classificationRegistry, CmdClassifications.DoxReference) },
				{ CmdClassifications.DoxIncludeFile,         BuildTag(classificationRegistry, CmdClassifications.DoxIncludeFile) },
				{ CmdClassifications.DoxUnknownCommand,      BuildTag(classificationRegistry, CmdClassifications.DoxUnknownCommand) },
				{ CmdClassifications.DoxUnknownReference,    BuildTag(classificationRegistry, CmdClassifications.DoxUnknownReference) },
				{ CmdClassifications.DoxUnknownIncludeFile,  BuildTag(classificationRegistry, CmdClassifications.DoxUnknownIncludeFile) },
				{ CmdClassifications.DoxCodeContent,         BuildTag(classificationRegistry, CmdClassifications.DoxCodeContent) },
				{ CmdClassifications.DoxDotContent,          BuildTag(classificationRegistry, CmdClassifications.DoxDotContent) },
				{ CmdClassifications.DoxMscContent,          BuildTag(classificationRegistry, CmdClassifications.DoxMscContent) },
				{ CmdClassifications.DoxPlantUMLContent,     BuildTag(classificationRegistry, CmdClassifications.DoxPlantUMLContent) },
				{ CmdClassifications.DoxLatexFormulaContent, BuildTag(classificationRegistry, CmdClassifications.DoxLatexFormulaContent) }
			};
    }

		private static ClassificationTag BuildTag(IClassificationTypeRegistryService classificationRegistry, string typeName)
		{
			return new ClassificationTag(classificationRegistry.GetClassificationType(typeName));
		}

		public IEnumerable<ITagSpan<ClassificationTag>> GetTags(NormalizedSnapshotSpanCollection spans)
		{
			foreach (var tagSpan in Aggregator.GetTags(spans))
			{
				var spanInfo = tagSpan.Tag;
				string classification;
				switch (spanInfo.Type)
				{
					default:
					case DoxTokenType.Cmd:
						classification = spanInfo.WellKnown ? CmdClassifications.DoxCommand : CmdClassifications.DoxUnknownCommand;
						yield return new TagSpan<ClassificationTag>(spanInfo.Span, _tags[classification]);
						break;

					case DoxTokenType.Ref:
						classification = spanInfo.WellKnown ? CmdClassifications.DoxReference : CmdClassifications.DoxUnknownReference;
						yield return new TagSpan<ClassificationTag>(spanInfo.Span, _tags[classification]);
						break;

					case DoxTokenType.Image:
					case DoxTokenType.DotFile:
					case DoxTokenType.MscFile:
					case DoxTokenType.DiaFile:
						classification = spanInfo.WellKnown ? CmdClassifications.DoxIncludeFile : CmdClassifications.DoxUnknownIncludeFile;
						yield return new TagSpan<ClassificationTag>(spanInfo.Span, _tags[classification]);
						break;

					case DoxTokenType.Code:
						classification = CmdClassifications.DoxCommand;
						yield return new TagSpan<ClassificationTag>(new SnapshotSpan(spanInfo.Span.Start, spanInfo.ContentSpan.Start), _tags[classification]);
						yield return new TagSpan<ClassificationTag>(spanInfo.ContentSpan, _tags[CmdClassifications.DoxCodeContent]);
						yield return new TagSpan<ClassificationTag>(new SnapshotSpan(spanInfo.ContentSpan.End, spanInfo.Span.End), _tags[classification]);
						break;

					case DoxTokenType.Dot:
						classification = CmdClassifications.DoxCommand;
						yield return new TagSpan<ClassificationTag>(new SnapshotSpan(spanInfo.Span.Start, spanInfo.ContentSpan.Start), _tags[classification]);
						yield return new TagSpan<ClassificationTag>(spanInfo.ContentSpan, _tags[CmdClassifications.DoxDotContent]);
						yield return new TagSpan<ClassificationTag>(new SnapshotSpan(spanInfo.ContentSpan.End, spanInfo.Span.End), _tags[classification]);
						break;

					case DoxTokenType.Msc:
						classification = CmdClassifications.DoxCommand;
						yield return new TagSpan<ClassificationTag>(new SnapshotSpan(spanInfo.Span.Start, spanInfo.ContentSpan.Start), _tags[classification]);
						yield return new TagSpan<ClassificationTag>(spanInfo.ContentSpan, _tags[CmdClassifications.DoxMscContent]);
						yield return new TagSpan<ClassificationTag>(new SnapshotSpan(spanInfo.ContentSpan.End, spanInfo.Span.End), _tags[classification]);
						break;

					case DoxTokenType.Uml:
						classification = CmdClassifications.DoxCommand;
						yield return new TagSpan<ClassificationTag>(new SnapshotSpan(spanInfo.Span.Start, spanInfo.ContentSpan.Start), _tags[classification]);
						yield return new TagSpan<ClassificationTag>(spanInfo.ContentSpan, _tags[CmdClassifications.DoxPlantUMLContent]);
						yield return new TagSpan<ClassificationTag>(new SnapshotSpan(spanInfo.ContentSpan.End, spanInfo.Span.End), _tags[classification]);
						break;

					case DoxTokenType.LatexFormula:
						classification = CmdClassifications.DoxCommand;
						yield return new TagSpan<ClassificationTag>(new SnapshotSpan(spanInfo.Span.Start, spanInfo.ContentSpan.Start), _tags[classification]);
						yield return new TagSpan<ClassificationTag>(spanInfo.ContentSpan, _tags[CmdClassifications.DoxLatexFormulaContent]);
						yield return new TagSpan<ClassificationTag>(new SnapshotSpan(spanInfo.ContentSpan.End, spanInfo.Span.End), _tags[classification]);
						break;
				}
			}
		}
  }
}

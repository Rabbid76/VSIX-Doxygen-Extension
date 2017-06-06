using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using System.ComponentModel.Composition;


namespace DoxPreviewExt.DoxUtil
{
	//! \brief type of a doxygen tags
	public enum DoxTokenType { Cmd, Ref, Image, Code, Dot, DotFile, Msc, MscFile, DiaFile, Uml, LatexFormula };

	//! \brief complete information about one span tag
	public class DoxTokenTag : ITag
	{
		public DoxTokenType Type { get; }
		public SnapshotSpan Span { get; }
		public SnapshotSpan ContentSpan { get; }
		public string Name { get; }
		public string Argument { get; }
		public bool WellKnown { get; }

		public DoxTokenTag(DoxTokenType type, SnapshotSpan span, SnapshotSpan contentSpan, string name, string arg, bool wellKnown)
		{
			this.Type = type;
			this.Span = span;
			this.ContentSpan = contentSpan;
			this.Name = name;
			this.Argument = arg;
			this.WellKnown = wellKnown;
		}
	}

	//! \brief doxygen tagger provider 
	[Export(typeof(ITaggerProvider))]
	//[ContentType("code")]
	[ContentType("C/C++"), ContentType("CSharp"), ContentType("DoxCode")]
	[TagType(typeof(DoxTokenTag))]
	internal sealed class DoxTokenTagProvider : ITaggerProvider
	{
		[Import]
		internal IClassifierAggregatorService AggregatorService;

		public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
		{
			//return new DoxTokenTagger(buffer) as ITagger<T>;

			Func<ITagger<T>> ttc = delegate ()
			{
				return new DoxTokenTagger(buffer, AggregatorService.GetClassifier(buffer)) as ITagger<T>;
			};

			return buffer.Properties.GetOrCreateSingletonProperty<ITagger<T>>(ttc);
		}
	}

	//! \brief doxygen tagger
	internal sealed class DoxTokenTagger : ITagger<DoxTokenTag>
	{
		List<DoxTokenTag> doxTags_ = null;

		ITextBuffer Buffer { get; }
	  IClassifier Classifier { get; }

		internal DoxTokenTagger(ITextBuffer buffer, IClassifier classifier)
		{
			Buffer = buffer;
			Classifier = classifier;

			Buffer.Changed += TextBufferChanged;

			ReParse();
		}

		public event EventHandler<SnapshotSpanEventArgs> TagsChanged
		{
			add { }
			remove { }
		}

		// \brief notification when the text buffer has changed
		// cf Walkthrough: Outlining, [https://msdn.microsoft.com/en-us/library/ee197665.aspx]
		void TextBufferChanged(object sender, TextContentChangedEventArgs e)
		{
			// If this isn't the most up-to-date version of the buffer, then ignore it for now (we'll eventually get another change event).
			if (e.After != Buffer.CurrentSnapshot)
				return;
			ReParse();
		}

		// \brief generqte list of tags from the text buffer
		void ReParse ()
		{
			SnapshotSpan bufferSpan = new SnapshotSpan(Buffer.CurrentSnapshot, new Span(0, Buffer.CurrentSnapshot.Length));
			List<SnapshotSpan> testSpanList = new List<SnapshotSpan>();
			testSpanList.Add(bufferSpan);
			try
			{
				doxTags_ = DoxTokenTaggerHelper.GenerateTags(testSpanList);
			}
			catch
			{
				doxTags_ = null;
			}
		}

		// \brief get tags according to spans
		public IEnumerable<ITagSpan<DoxTokenTag>> GetTags(NormalizedSnapshotSpanCollection spans)
		{
			if (doxTags_ == null || doxTags_.Count == 0 || spans == null || spans.Count == 0)
				yield break;

			/*
			Since DoxTokenTaggerHelper::LexIdentifiers has already recognized and analyzed the comments,
			this is no longer necessary

			if (Buffer.ContentType.ToString() == "C/C++" || Buffer.ContentType.ToString() == "CSharp")
			{
				// checks if snapshot is part of a comment
				// cf Walkthrough: Creating a Margin Glyph, [https://msdn.microsoft.com/en-us/library/ee361745.aspx]
				List<SnapshotSpan> commentSpans = new List<SnapshotSpan>();
				foreach (var span in spans)
				{
					foreach (ClassificationSpan classification in Classifier.GetClassificationSpans(span))
				  {
						//if the classification is a comment
						if (classification.ClassificationType.Classification.ToLower().Contains("comment"))
							commentSpans.Add(span);
					}
				}

				NormalizedSnapshotSpanCollection commnstSpanCollection = new NormalizedSnapshotSpanCollection(commentSpans);
				foreach (var spanTag in doxTags_)
				{
					if (commnstSpanCollection.IntersectsWith(spanTag.Span))
						yield return new TagSpan<DoxTokenTag>(spanTag.Span, spanTag);
				}
			}
			else
			*/
			{
				foreach (var spanTag in doxTags_)
				{
					if (spans.IntersectsWith(spanTag.Span))
					  yield return new TagSpan<DoxTokenTag>(spanTag.Span, spanTag);
			  }
			}
		}
	}

	//! \brief simple doxygen special command parser
	public static class DoxTokenTaggerHelper
	{
		//! \brief find tag at position
		public static DoxTokenTag TagHitTest(ITagAggregator<DoxTokenTag> aggregator, ITextSnapshot snapshot, int pos)
		{
			if (aggregator == null)
				return null;

			SnapshotSpan snapshotSpan = new SnapshotSpan(snapshot, new Span(0, snapshot.Length));
			foreach (var spanTag in aggregator.GetTags(snapshotSpan) )
			{
				if (spanTag.Tag.Span.Start <= pos && spanTag.Tag.Span.End > pos)
					return spanTag.Tag;
			}
			return null;
		}
			
		
		public static List<DoxTokenTag> GenerateTags(IEnumerable<SnapshotSpan> spans)
		{
			List<DoxTokenTag> spanTags = new List<DoxTokenTag>();
			CManager manager = CManager.Manager;
			if (manager == null)
				return spanTags;

			List<SnapshotSpan> identList = new List<SnapshotSpan>();
			foreach (var span in spans)
			{
				var newSpans = LexIdentifiers(span);
				identList.InsertRange(identList.Count, newSpans);
			}

			SortedSet<string> imageKinds = new SortedSet<string>{ "html", "latex", "docbook", "rtf"};

			for (int i = 0; i < identList.Count(); ++i)
			{
				var identSpan = identList[i];
				var ident = identSpan.GetText().ToLower();

				bool isDoxCommand = ident.Length > 0 && IsStartOfCommand(ident[0]);
				if (isDoxCommand)
				{
					bool cmdIdentified = false;
					bool valide_cmd = false;
					int file_name_inx = 0;
					int sequnce_end_inx = 0;
					var cmdName = ident.Substring(1);
					if (cmdName == "ref")
					{
						cmdIdentified = true;
						bool validRefDecl = i + 1 < identList.Count();
						var refText = validRefDecl ? identList[i + 1].GetText() : "";
						if (validRefDecl)
							validRefDecl = refText.Length > 0 && char.IsLetter(refText[0]);
						if (validRefDecl)
						{
							var refTextSpan = identList[i + 1];
							var refTextEndPoint = refTextSpan.End;
							var dotPos = refText.IndexOf('.');
							if ( dotPos >= 0 )
							{
								refText = refText.Substring(0, dotPos);
								refTextEndPoint = refTextSpan.Start + dotPos;
							}
							var refSpan = new SnapshotSpan(identSpan.Start, refTextEndPoint);
							var url = manager.FindDoxygenLink(refText, false);
							spanTags.Add(new DoxTokenTag(DoxTokenType.Ref, refSpan, refTextSpan, cmdName, url != "" ? url : refText, url != ""));
							i++;
						}
						else
						{
							spanTags.Add(new DoxTokenTag(DoxTokenType.Cmd, identSpan, identSpan, cmdName, "", true));
						}
					}
					else if (FindCmdFileRef(identList, "image", imageKinds, i, out valide_cmd, out file_name_inx, out sequnce_end_inx ) )
					{
						cmdIdentified = true;
						if (valide_cmd)
						{
							var endSpan = identList[sequnce_end_inx];
							var imgNameSpan = identList[file_name_inx];
							var imgName = identList[file_name_inx].GetText();
							var imgSpan = new SnapshotSpan(identSpan.Start, endSpan.End);
							var imgPath = manager.FindImageFilePath(imgName);
							spanTags.Add(new DoxTokenTag(DoxTokenType.Image, imgSpan, imgNameSpan, cmdName, imgPath != "" ? imgPath : imgName, imgPath != ""));
						}
						else
						{
							spanTags.Add(new DoxTokenTag(DoxTokenType.Image, identSpan, identSpan, cmdName, "", false));
						}
						i = sequnce_end_inx;
					}
					else if (FindCmdBlock(identList, "code", "endcode", i, out sequnce_end_inx))
					{
						cmdIdentified = true;
						var endSpan = identList[sequnce_end_inx];
						var umlSpan = new SnapshotSpan(identSpan.Start, endSpan.End);
						var umlContentSpan = new SnapshotSpan(identSpan.End, endSpan.Start);
						spanTags.Add(new DoxTokenTag(DoxTokenType.Code, umlSpan, umlContentSpan, cmdName, "", true));
						i = sequnce_end_inx;
					}
					else if (FindCmdBlock(identList, "dot", "enddot", i, out sequnce_end_inx))
					{
						cmdIdentified = true;
						var endSpan = identList[sequnce_end_inx];
						var umlSpan = new SnapshotSpan(identSpan.Start, endSpan.End);
						var umlContentSpan = new SnapshotSpan(identSpan.End, endSpan.Start);
						spanTags.Add(new DoxTokenTag(DoxTokenType.Dot, umlSpan, umlContentSpan, cmdName, "", true));
						i = sequnce_end_inx;
					}
					else if (FindCmdFileRef(identList, "dotfile", null, i, out valide_cmd, out file_name_inx, out sequnce_end_inx))
					{
						cmdIdentified = true;
						if (valide_cmd)
						{
							var endSpan = identList[sequnce_end_inx];
							var dotNameSpan = identList[file_name_inx];
							var dotName = identList[file_name_inx].GetText();
							var dotSpan = new SnapshotSpan(identSpan.Start, endSpan.End);
							var dotPath = manager.FindDotFilePath(dotName);
							spanTags.Add(new DoxTokenTag(DoxTokenType.DotFile, dotSpan, dotNameSpan, cmdName, dotPath != "" ? dotPath : dotName, dotPath != ""));
						}
						else
						{
							spanTags.Add(new DoxTokenTag(DoxTokenType.DotFile, identSpan, identSpan, cmdName, "", false));
						}
						i = sequnce_end_inx;
					}
					else if (FindCmdBlock(identList, "msc", "endmsc", i, out sequnce_end_inx))
					{
						cmdIdentified = true;
						var endSpan = identList[sequnce_end_inx];
						var umlSpan = new SnapshotSpan(identSpan.Start, endSpan.End);
						var umlContentSpan = new SnapshotSpan(identSpan.End, endSpan.Start);
						spanTags.Add(new DoxTokenTag(DoxTokenType.Msc, umlSpan, umlContentSpan, cmdName, "", true));
						i = sequnce_end_inx;
					}
					else if (FindCmdFileRef(identList, "mscfile", null, i, out valide_cmd, out file_name_inx, out sequnce_end_inx))
					{
						cmdIdentified = true;
						if (valide_cmd)
						{
							var endSpan = identList[sequnce_end_inx];
							var mscNameSpan = identList[file_name_inx];
							var mscName = identList[file_name_inx].GetText();
							var mscSpan = new SnapshotSpan(identSpan.Start, endSpan.End);
							var mscPath = manager.FindMscFilePath(mscName);
							spanTags.Add(new DoxTokenTag(DoxTokenType.MscFile, mscSpan, mscNameSpan, cmdName, mscPath != "" ? mscPath : mscName, mscPath != ""));
						}
						else
						{
							spanTags.Add(new DoxTokenTag(DoxTokenType.MscFile, identSpan, identSpan, cmdName, "", false));
						}
						i = sequnce_end_inx;
					}
					else if (FindCmdFileRef(identList, "diafile", null, i, out valide_cmd, out file_name_inx, out sequnce_end_inx))
					{
						cmdIdentified = true;
						if (valide_cmd)
						{
							var endSpan = identList[sequnce_end_inx];
							var diaNameSpan = identList[file_name_inx];
							var diaName = identList[file_name_inx].GetText();
							var diaSpan = new SnapshotSpan(identSpan.Start, endSpan.End);
							var diaPath = manager.FindDiaFilePath(diaName);
							spanTags.Add(new DoxTokenTag(DoxTokenType.MscFile, diaSpan, diaNameSpan, cmdName, diaPath != "" ? diaPath : diaName, diaPath != ""));
						}
						else
						{
							spanTags.Add(new DoxTokenTag(DoxTokenType.MscFile, identSpan, identSpan, cmdName, "", false));
						}
						i = sequnce_end_inx;
					}
					else if (FindCmdBlock(identList, "startuml", "enduml", i, out sequnce_end_inx))
					{
						cmdIdentified = true;
						var endSpan = identList[sequnce_end_inx];
						var umlSpan = new SnapshotSpan(identSpan.Start, endSpan.End);
						var umlContentSpan = new SnapshotSpan(identSpan.End, endSpan.Start);
						spanTags.Add(new DoxTokenTag(DoxTokenType.Uml, umlSpan, umlContentSpan, cmdName, "", true));
						i = sequnce_end_inx;
					}
					else if (FindCmdBlock(identList, "f$", "f$", i, out sequnce_end_inx) ||
									 FindCmdBlock(identList, "f[", "f]", i, out sequnce_end_inx))
					{
						cmdIdentified = true;
						var endSpan = identList[sequnce_end_inx];
						var umlSpan = new SnapshotSpan(identSpan.Start, endSpan.End);
						var umlContentSpan = new SnapshotSpan(identSpan.End, endSpan.Start);
						spanTags.Add(new DoxTokenTag(DoxTokenType.LatexFormula, umlSpan, umlContentSpan, cmdName, "", true));
						i = sequnce_end_inx;
					}

					if (cmdIdentified == false )
					  spanTags.Add(new DoxTokenTag(DoxTokenType.Cmd, identSpan, identSpan, cmdName, "", DoxUtil.CManager.Manager.IsKnownCommand(ident)));
				}
			}
			return spanTags;
		}

		// \brief search for a command block in the ident list 
		public static bool FindCmdFileRef(List<SnapshotSpan> identList, string startCmd, SortedSet<string> kinds, int start_inx, out bool valid, out int file_name_inx, out int sequnce_end_inx)
		{
			file_name_inx = start_inx;
			sequnce_end_inx = start_inx;
			valid = false;

			var startIdent = identList[start_inx].GetText().ToLower();
			if (IsStartOfCommand(startIdent[0]) == false || startIdent.Substring(1) != startCmd)
				return false;

			bool hasKind = kinds != null && kinds.Count() > 0;
			int tokens = hasKind ? 2 : 1;
			if (start_inx + tokens >= identList.Count())
				return true;

			if (hasKind)
			{
				var tokenKind = identList[start_inx + 1].GetText().ToLower();
				if (kinds.Contains(tokenKind) == false)
					return true;
			}

			file_name_inx = sequnce_end_inx = start_inx + tokens;
			valid = true;
			return true;
		}

		// \brief search for a command block in the ident list 
		public static bool FindCmdBlock(List<SnapshotSpan> identList, string startCmd, string endCmd, int start_inx, out int sequnce_end_inx)
		{
			sequnce_end_inx = start_inx;
			var startIdent = identList[start_inx].GetText().ToLower();
			if (IsStartOfCommand(startIdent[0]) == false || startIdent.Substring(1) != startCmd)
				return false;
			return FindCmdToken(identList, endCmd, start_inx + 1, out sequnce_end_inx);
		}

		// \brief search for a command in the ident list  
		public static bool FindCmdToken( List<SnapshotSpan> identList, string endCmd, int strat_inx, out int token_inx )
		{
			token_inx = strat_inx;
			for (int test_inx = strat_inx; test_inx < identList.Count(); ++test_inx)
			{
				var identStr = identList[test_inx].GetText().ToLower();
				if (IsStartOfCommand(identStr[0]) && identStr.Substring(1).StartsWith(endCmd))
				{
					token_inx = test_inx;
					return true;
				}
			}
			return false;
		}

		public static bool IsDoxBlockCommentStart(string buffer, int pos)
		{
			return pos + 2 < buffer.Length && buffer[pos] == '/' && buffer[pos + 1] == '*' && (buffer[pos + 2] == '*' || buffer[pos + 2] == '!');
		}

		public static bool IsBlockCommentStart( string buffer, int pos )
		{
			return pos + 1 < buffer.Length && buffer[pos] == '/' && buffer[pos + 1] == '*';
		}

		public static bool IsBlockCommentEnd(string buffer, int pos)
		{
			return pos + 1 < buffer.Length && buffer[pos] == '*' && buffer[pos + 1] == '/';
		}

		public static bool IsDoxLineCommentStart(string buffer, int pos)
		{
			return pos + 2 < buffer.Length && buffer[pos] == '/' && buffer[pos + 1] == '/' && (buffer[pos + 2] == '/' || buffer[pos + 2] == '!');
		}

		public static bool IsLineCommentStart(string buffer, int pos)
		{
			return pos + 1 < buffer.Length && buffer[pos] == '/' && buffer[pos + 1] == '/';
		}

		public static bool IsLineCommentEnd(string buffer, int pos)
		{
			return pos < buffer.Length && buffer[pos] == '\n';
		}

		public static IEnumerable<SnapshotSpan> CommentSpans(SnapshotSpan span)
		{
			var s = span.GetText();
			List<SnapshotSpan> commentSpans = new List<SnapshotSpan>();
			for (int i = 0; i < s.Length; ++i)
			{
				if (IsDoxBlockCommentStart(s, i))
				{
					var start = i;
					int commentCount = 1;
					for (i += 2; i < s.Length; ++i)
					{
						if (IsBlockCommentEnd(s, i))
							commentCount--;
						else if (IsBlockCommentStart(s, i))
							commentCount++;
						if (commentCount == 0)
							break;
					}
					yield return new SnapshotSpan(span.Start + start, Math.Min(i + 1, s.Length) - start);
					continue;
				}
				else if (IsDoxLineCommentStart(s,i))
				{
					var start = i;
					for (i += 2; i < s.Length; ++i)
					{
						if (IsLineCommentEnd(s,i))
							break;
					}
					yield return new SnapshotSpan(span.Start + start, Math.Min(i + 1, s.Length) - start);
					continue;
				}
			}
		}

		public static List<SnapshotSpan> LexIdentifiers(SnapshotSpan span)
		{
			var s = span.GetText();
			List<SnapshotSpan> identList = new List<SnapshotSpan>();
			// Tokenize the string into identifiers and numbers, returning only the identifiers
			foreach (var commentSpan in CommentSpans(span))
			{
				for (int i = commentSpan.Start; i < commentSpan.Start + commentSpan.Length;)
				{
					if (IsStartOfIdent(s[i]))
					{
						var start = i;
						for (++i; i < s.Length && IsTokenChar(s[i]); ++i) ;
						identList.Add(new SnapshotSpan(span.Start + start, i - start));
						continue;
					}
					if (char.IsDigit(s[i]))
					{
						var start = i;
						for (++i; i < s.Length && char.IsDigit(s[i]); ++i) ;
						identList.Add(new SnapshotSpan(span.Start + start, i - start));
						continue;
					}
					if (s[i] == '\"')
					{
						var start = i;
						var end = i;
						for (var str_i = i+1; str_i < s.Length && s[str_i] != '\n'; ++ str_i)
						{
							if (s[str_i] == '\"' && s[str_i - 1] != '\\')
							{
								end = str_i;
								break;
							}
						}
						if (end > start)
						{
							i = end + 1;
							identList.Add(new SnapshotSpan(span.Start + start, i - start));
							continue;
						}
					}
					if (s[i] == '\'')
					{
						var start = i;
						var end = i;
						for (var str_i = i + 1; str_i < s.Length && s[str_i] != '\n'; ++str_i)
						{
							if (s[str_i] == '\'' && s[str_i - 1] != '\\')
							{
								end = str_i;
								break;
							}
						}
						if (end > start)
						{
							i = end + 1;
							identList.Add(new SnapshotSpan(span.Start + start, i - start));
							continue;
						}
					}
					/*
					if (s[i] == '[')
					{
						var start = i;
						for (++i; i < s.Length && s[i] != ']'; ++i) ;
						if (i < s.Length) ++i;
						identList.Add(new SnapshotSpan(span.Start + start, i - start));
						continue;
					}
					*/
					if (s[i] == '\n')
					{
						var start = i;
						++i;
						identList.Add(new SnapshotSpan(span.Start + start, i - start));
						continue;
					}
					++i;
				}
			}
			return identList;
		}

		public static bool IsStartOfIdent(char c)
		{
			return IsStartOfCommand(c) || c == '_' || char.IsLetter(c);
		}

		public static bool IsStartOfCommand(char c)
		{
			return c == '@' || c == '\\';
		}

		public static bool IsTokenChar(char c)
		{
			return char.IsLetterOrDigit(c) || c == '_' || c == ':' || c == '.'  || c == '$' || c == '[' || c == ']' || c == '-' || c == '/';
			/*|| c == '\\' || c == '@'*/
		}
	}
}

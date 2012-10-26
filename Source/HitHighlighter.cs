using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TwitterText {
    /// <summary>
    /// A class for adding HTML highlighting in Tweet text (such as would be returned from a Search)
    /// </summary>
    public class HitHighlighter {

        /// <summary>
        /// Default HTML tag for highlight hits
        /// </summary>
        public const string DEFAULT_HIGHLIGHT_TAG = "em";

        /// <summary>
        /// Get or sets the current HTML tag used for phrase highlighting.
        /// </summary>
        public string HighlightTag { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HitHighlighter"/> class.
        /// </summary>
        public HitHighlighter() {
            HighlightTag = DEFAULT_HIGHLIGHT_TAG;
        }

        /// <summary>
        /// Surround the hits in the provided text with an HTML tag. This is used with offsets
        /// from the search API to support the highlighting of query terms.
        /// </summary>
        /// <param name="text">text of the Tweet to highlight</param>
        /// <param name="hits"> A List of highlighting offsets (themselves lists of two elements)</param>
        /// <returns>text with highlight HTML added</returns>
        public string Highlight(string text, List<List<int>> hits) {
            if (hits == null || !hits.Any()) {
                return (text);
            }

            // TODO: translate to C#

            //StringBuilder sb = new StringBuilder(text.Length);
            //CharacterIterator iterator = new StringCharacterIterator(text);
            //bool isCounting = true;
            //bool tagOpened = false;
            //int currentIndex = 0;
            //char currentChar = iterator.first();

            //while (currentChar != CharacterIterator.DONE) {
            //  // TODO: this is slow.
            //  for (List<int> start_end : hits) {
            //    if (start_end.get(0) == currentIndex) {
            //      sb.append(tag(false));
            //      tagOpened = true;
            //    } else if (start_end.get(1) == currentIndex) {
            //      sb.append(tag(true));
            //      tagOpened = false;
            //    }
            //  }

            //  if (currentChar == '<') {
            //    isCounting = false;
            //  } else if (currentChar == '>' && !isCounting) {
            //    isCounting = true;
            //  }

            //  if (isCounting) {
            //    currentIndex++;
            //  }
            //  sb.Append(currentChar);
            //  currentChar = iterator.next();
            //}

            //if (tagOpened) {
            //  sb.Append(tag(true));
            //}
            //return sb.ToString();

            return text;
        }

        /// <summary>
        /// Format the current HighlightTag by adding &lt; and >. If <paramref name="closeTag"/> is true.
        /// then the tag returned will include a / to signify a closing tag.
        /// <param name="closeTag">true if this is a closing tag, otherwise false</param>
        /// <returns></returns>
        protected string Tag(bool closeTag) {
            StringBuilder sb = new StringBuilder(HighlightTag.Length + 3);
            sb.Append("<");
            if (closeTag) {
                sb.Append("/");
            }
            sb.Append(HighlightTag).Append(">");
            return sb.ToString();
        }
    }
}

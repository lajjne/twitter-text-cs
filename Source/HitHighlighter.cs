using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TwitterText {
    /// <summary>
    /// A class for adding HTML highlighting in Tweet text (such as would be returned from a Search)
    /// </summary>
    public class HitHighlighter {

        /** Default HTML tag for highlight hits */
        public static readonly string DEFAULT_HIGHLIGHT_TAG = "em";

        /** the current HTML tag used for hit highlighting */
        protected string highlightTag;

        /** Create a new HitHighlighter object. */
        public HitHighlighter() {
            highlightTag = DEFAULT_HIGHLIGHT_TAG;
        }

        /**
         * Surround the <code>hits</code> in the provided <code>text</code> with an HTML tag. This is used with offsets
         * from the search API to support the highlighting of query terms.
         *
         * @param text of the Tweet to highlight
         * @param hits A List of highlighting offsets (themselves lists of two elements)
         * @return text with highlight HTML added
         */
        public string highlight(string text, List<List<int>> hits) {
            if (hits == null || !hits.Any()) {
                return (text);
            }

            StringBuilder sb = new StringBuilder(text.Length);

            // TODO: translate to C#
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

            return sb.ToString();
        }

        /**
         * Format the current <code>highlightTag</code> by adding &lt; and >. If <code>closeTag</code> is <code>true</code>
         * then the tag returned will include a <code>/</code> to signify a closing tag.
         *
         * @param true if this is a closing tag, false otherwise
         */
        protected string tag(bool closeTag) {
            StringBuilder sb = new StringBuilder(highlightTag.Length + 3);
            sb.Append("<");
            if (closeTag) {
                sb.Append("/");
            }
            sb.Append(highlightTag).Append(">");
            return sb.ToString();
        }

        /**
         * Get the current HTML tag used for phrase highlighting.
         *
         * @return current HTML tag (without &lt; or >)
         */
        public string getHighlightTag() {
            return highlightTag;
        }

        /**
         * Set the current HTML tag used for phrase highlighting.
         *
         * @param new HTML tag (without &lt; or >)
         */
        public void setHighlightTag(string highlightTag) {
            this.highlightTag = highlightTag;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TwitterText {
    /// <summary>
    /// A class for validating Tweet texts.
    /// </summary>
    public class Validator {

        public static readonly int MAX_TWEET_LENGTH = 140;

        protected int shortUrlLength = 20;
        protected int shortUrlLengthHttps = 21;

        private Extractor extractor = new Extractor();

        public int getTweetLength(String text) {
            //text = Normalizer.normalize(text, Normalizer.Form.NFC);
            //int length = text.codePointCount(0, text.length());            
            text = text.Normalize(NormalizationForm.FormC);
            int length = text.Length;

            //for (Extractor.Entity urlEntity : extractor.extractURLsWithIndices(text)) {
            foreach (Entity urlEntity in extractor.extractURLsWithIndices(text)) {
              length += urlEntity.start - urlEntity.end;
              length += urlEntity.value.ToLower().StartsWith("https://") ? shortUrlLengthHttps : shortUrlLength;
            }
            return length;
        }

        public bool isValidTweet(String text) {
            if (string.IsNullOrEmpty(text)) {
                return false;
            }

            //for (char c : text.toCharArray()) {
            foreach (char c in text) {
                //if (c == '\uFFFE' || c == '\uuFEFF' ||   // BOM
                if (c == '\uFFFE' ||   // BOM
                    c == '\uFFFF' ||                     // Special
                    (c >= '\u202A' && c <= '\u202E')) {  // Direction change
                    return false;
                }
            }

            return getTweetLength(text) <= MAX_TWEET_LENGTH;
        }

        public int getShortUrlLength() {
            return shortUrlLength;
        }

        public void setShortUrlLength(int shortUrlLength) {
            this.shortUrlLength = shortUrlLength;
        }

        public int getShortUrlLengthHttps() {
            return shortUrlLengthHttps;
        }

        public void setShortUrlLengthHttps(int shortUrlLengthHttps) {
            this.shortUrlLengthHttps = shortUrlLengthHttps;
        }
    }
}

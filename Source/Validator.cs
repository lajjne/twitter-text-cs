using System;
using System.Text;

namespace TwitterText {
    /// <summary>
    /// A class for validating Tweet texts.
    /// </summary>
    public class Validator {

        /// <summary>
        /// The Extractor used to extract entities from text.
        /// </summary>
        private Extractor _extractor;
        
        /// <summary>
        /// 
        /// </summary>
        public const int MAX_TWEET_LENGTH = 140;
        
        /// <summary>
        /// 
        /// </summary>
        public int ShortUrlLength { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public int ShortUrlLengthHttps { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Validator() {
            ShortUrlLength = 20;
            ShortUrlLengthHttps = 21;
            _extractor = new Extractor();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public int GetTweetLength(String text) {
            text = text.Normalize(NormalizationForm.FormC);
            int length = text.Length;
            foreach (Entity urlEntity in _extractor.ExtractURLsWithIndices(text)) {
                length += urlEntity.Start - urlEntity.End;
                length += urlEntity.Value.ToLower().StartsWith("https://") ? ShortUrlLengthHttps : ShortUrlLength;
            }
            return length;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public bool IsValidTweet(String text) {
            if (string.IsNullOrEmpty(text)) {
                return false;
            }
            foreach (char c in text) {
                //if (c == '\uFFFE' || c == '\uuFEFF' ||   // BOM
                if (c == '\uFFFE' ||   // BOM
                    c == '\uFFFF' ||                     // Special
                    (c >= '\u202A' && c <= '\u202E')) {  // Direction change
                    return false;
                }
            }
            return GetTweetLength(text) <= MAX_TWEET_LENGTH;
        }
    }
}

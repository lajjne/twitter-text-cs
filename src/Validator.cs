using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Twitter.Text {
    /// <summary>
    /// A class for validating Tweet texts.
    /// </summary>
    public class Validator {
        // max allowed tweet length
        private const int MAX_TWEET_LENGTH = 140;

        // extractor used when caclculating tweet length
        private Extractor _extractor = new Extractor();

        /// <summary>
        /// Gets or sets the max length of http://t.co urls. See https://dev.twitter.com/overview/t.co.
        /// </summary>
        public int ShortUrlLength { get; set; } = 23;

        /// <summary>
        /// Gets or sets the max length of https://t.co urls. See https://dev.twitter.com/overview/t.co.
        /// </summary>
        public int ShortUrlLengthHttps { get; set; } = 23;

        /// <summary>
        /// 
        /// </summary>
        public Validator() {
        }

        /// <summary>
        /// Returns the length of the specified tweet.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public int GetTweetLength(string text) {
            try {
                text = text.Normalize(NormalizationForm.FormC);
            } catch { }

            int length = new StringInfo(text).LengthInTextElements;
            foreach (TweetEntity urlEntity in _extractor.ExtractUrlsWithIndices(text)) {
                // Subtract the length of the original URL
                length -= (urlEntity.End - urlEntity.Start);

                // Add `ShortUrlLengthHttps` characters for URL starting with https:// Otherwise add `ShortUrlLength` characters
                length += urlEntity.Value.ToLower().StartsWith("https://") ? ShortUrlLengthHttps : ShortUrlLength;
            }
            return length;
        }

        /// <summary>
        /// Checks if the specified text is a valid tweet.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public bool IsValidTweet(string text) {
            if (string.IsNullOrWhiteSpace(text)) {
                return false;
            }

            if (Regex.INVALID_CHARACTERS.IsMatch(text)) {
                return false;
            }

            return this.GetTweetLength(text) <= MAX_TWEET_LENGTH;
        }

        /// <summary>
        /// Checks if the specified text is a valid username.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public bool IsValidUsername(string text) {
            if (string.IsNullOrWhiteSpace(text)) {
                return false;
            }

            // Must match whole string
            Match match = Regex.VALID_REPLY.Match(text);
            if (match.Success && match.Length == text.Length) {
                return true;
            } else {
                return false;
            }
        }

        /// <summary>
        /// Checks if the specified text is a valid list.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public bool IsValidList(string text) {
            if (string.IsNullOrWhiteSpace(text)) {
                return false;
            }

            // Must match, have nothing before, and contain a list
            Match match = Regex.VALID_MENTION_OR_LIST.Match(text);
            if (match.Success &&
                match.Length == text.Length &&
                string.IsNullOrEmpty(match.Groups[Regex.VALID_MENTION_OR_LIST_GROUP_BEFORE].Value) &&
                !string.IsNullOrEmpty(match.Groups[Regex.VALID_MENTION_OR_LIST_GROUP_LIST].Value)) {
                return true;
            } else {
                return false;
            }
        }

        /// <summary>
        /// Checks if the specified text is a valid hashtag.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public bool IsValidHashTag(string text) {
            if (string.IsNullOrWhiteSpace(text)) {
                return false;
            }

            // Must match, have nothing before, and contain a list
            Match match = Regex.VALID_HASHTAG.Match(text);
            if (match.Success && match.Length == text.Length) {
                return true;
            } else {
                return false;
            }
        }

        /// <summary>
        /// Checks if the specified text is a valid url.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public bool IsValidUrl(string text) {
            if (string.IsNullOrWhiteSpace(text)) {
                return false;
            }

            // Must match, have nothing before, and contain a list
            Match match = Regex.VALID_URL.Match(text);
            if (match.Success && match.Length == text.Length) {
                return true;
            } else {
                return false;
            }
        }
    }
}
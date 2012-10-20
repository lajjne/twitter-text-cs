using System;
using System.Collections.Generic;

namespace TwitterText {

    /// <summary>
    /// Extension methods for extracting @mentions and #hashtags etc. from strings.
    /// </summary>
    public static class TweetExtensions {

        /// <summary>
        /// Extract @username references from a string.
        /// </summary>
        /// <param name="text">text from which to extract usernames</param>
        /// <returns>List of usernames referenced (without the leading @ sign)</returns>
        public static List<string> ExtractUsernames(this string text) {
            Extractor extractor = new Extractor();
            return extractor.extractMentionedScreennames(text);
        }

        /// <summary>
        /// Extract a @username reference from the beginning of a string. A reply is an occurance of @username at the
        /// beginning of a string, preceded by 0 or more spaces.
        /// </summary>
        /// <param name="text">text from which to extract the replied to username.</param>
        /// <returns>sername referenced, if any (without the leading @ sign). Returns null if this is not a reply.</returns>
        public static string ExtractReplyUsername(this string text) {
            Extractor extractor = new Extractor();
            return extractor.extractReplyScreenname(text);
        }

        /// <summary>
        /// Extract URL references from a string.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static List<string> ExtractURLs(this string text) {
            Extractor extractor = new Extractor();
            return extractor.extractURLs(text);
        }

        /// <summary>
        /// Extract #hashtag references from a string.
        /// </summary>
        /// <param name="text">text from which to extract hashtags</param>
        /// <returns>List of hashtags referenced (without the leading # sign)</returns>
        public static List<string> ExtractHashtags(this string text) {
            Extractor extractor = new Extractor();
            return extractor.extractHashtags(text);
        }

        /// <summary>
        ///  Auto-link @usernames, #hashtags and URLs.
        /// </summary>
        /// <param name="text">text of the Tweet to auto-link</param>
        /// <returns>text with auto-link HTML added</returns>
        public static string AutoLink(this string text) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Auto-link the @username references in the provided text. Links to @username references will
        /// have the usernameClass CSS classes added.
        /// </summary>
        /// <param name="text">text of the Tweet to auto-link</param>
        /// <returns>text with auto-link HTML added</returns>
        public static string AutoLinkUsernames(this string text) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Auto-link #hashtag references in the provided Tweet text. The #hashtag links will have the hashtagClass CSS class
        /// added.
        /// </summary>
        /// <param name="text">text of the Tweet to auto-link</param>
        /// <returns>ext with auto-link HTML added</returns>
        public static string AutoLinkHashtags(this string text) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Auto-link URLs in the Tweet text provided.
        /// </summary>
        /// <param name="text">text of the Tweet to auto-link</param>
        /// <returns>text with auto-link HTML added</returns>
        public static string AutoLinkURLs(this string text) {
            throw new NotImplementedException();
        }
    }
}

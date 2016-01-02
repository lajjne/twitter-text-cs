using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Twitter.Text {
    /// <summary>
    /// A class to extract usernames, lists, hashtags and URLs from Tweet text.
    /// </summary>
    public class Extractor {

        /// <summary>
        /// Compares entities bases on start index.
        /// </summary>
        private class StartIndexComparer : Comparer<TweetEntity> {
            public override int Compare(TweetEntity a, TweetEntity b) {
                if (a.Start > b.Start) return 1;
                else if (a.Start < b.Start) return -1;
                else return 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool ExtractURLWithoutProtocol { get; set; } = true;

        /// <summary>
        /// Create a new extractor.
        /// </summary>
        public Extractor() {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        private void RemoveOverlappingEntities(List<TweetEntity> entities) {
            // Sort by index
            entities.Sort(new StartIndexComparer());

            // Remove overlapping entities.
            // Two entities overlap only when one is URL and the other is hashtag/mention
            // which is a part of the URL. When it happens, we choose URL over hashtag/mention
            // by selecting the one with smaller start index.
            List<TweetEntity> toRemove = new List<TweetEntity>();
            if (entities.Count > 0) {
                IEnumerator<TweetEntity> it = entities.GetEnumerator();
                it.MoveNext();
                TweetEntity prev = it.Current;
                while (it.MoveNext()) {
                    TweetEntity cur = it.Current;
                    if (prev.End > cur.Start) {
                        toRemove.Add(cur);
                    } else {
                        prev = cur;
                    }
                }
                foreach (TweetEntity remove in toRemove) {
                    entities.Remove(remove);
                }
            }
        }

        /// <summary>
        /// Extract URLs, @mentions, lists and #hashtag from a given text/tweet.
        /// </summary>
        /// <param name="text">text of tweet</param>
        /// <returns>list of extracted entities</returns>
        public List<TweetEntity> ExtractEntitiesWithIndices(string text) {
            List<TweetEntity> entities = new List<TweetEntity>();
            entities.AddRange(ExtractURLsWithIndices(text));
            entities.AddRange(ExtractHashtagsWithIndices(text, false));
            entities.AddRange(ExtractMentionsOrListsWithIndices(text));
            entities.AddRange(ExtractCashtagsWithIndices(text));

            RemoveOverlappingEntities(entities);
            return entities;
        }

        /// <summary>
        /// Extract @username references from Tweet text. A mention is an occurance of @username anywhere in a Tweet.
        /// </summary>
        /// <param name="text">text of the tweet from which to extract usernames</param>
        /// <returns>List of usernames referenced (without the leading @ sign)</returns>
        public List<string> ExtractMentionedScreennames(string text) {
            if (string.IsNullOrWhiteSpace(text)) {
                return new List<string>();
            }

            List<string> extracted = new List<string>();
            foreach (TweetEntity entity in ExtractMentionedScreennamesWithIndices(text)) {
                extracted.Add(entity.Value);
            }
            return extracted;
        }

        /// <summary>
        /// Extract @username references from Tweet text. A mention is an occurance of @username anywhere in a Tweet.
        /// </summary>
        /// <param name="text">text of the tweet from which to extract usernames</param>
        /// <returns>List of usernames referenced (without the leading @ sign)</returns>
        public List<TweetEntity> ExtractMentionedScreennamesWithIndices(string text) {
            List<TweetEntity> extracted = new List<TweetEntity>();
            foreach (TweetEntity entity in ExtractMentionsOrListsWithIndices(text)) {
                if (entity.ListSlug == null) {
                    extracted.Add(entity);
                }
            }
            return extracted;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public List<TweetEntity> ExtractMentionsOrListsWithIndices(string text) {
            if (string.IsNullOrWhiteSpace(text)) {
                return new List<TweetEntity>();
            }

            // Performance optimization.
            // If text doesn't contain @/＠ at all, the text doesn't
            // contain @mention. So we can simply return an empty list.
            bool found = false;
            foreach (char c in text) {
                if (c == '@' || c == '＠') {
                    found = true;
                    break;
                }
            }
            if (!found) {
                return new List<TweetEntity>();
            }

            List<TweetEntity> extracted = new List<TweetEntity>();
            MatchCollection matcher = Regex.VALID_MENTION_OR_LIST.Matches(text);
            foreach (Match match in matcher) {
                string after = text.Substring(match.Index + match.Length);
                if (!Regex.INVALID_MENTION_MATCH_END.IsMatch(after)) {
                    if (!match.Groups[Regex.VALID_MENTION_OR_LIST_GROUP_LIST].Success) {
                        extracted.Add(new TweetEntity(match, TweetEntityType.Mention, Regex.VALID_MENTION_OR_LIST_GROUP_USERNAME));
                    } else {
                        extracted.Add(new TweetEntity(match.Groups[Regex.VALID_MENTION_OR_LIST_GROUP_USERNAME].Index - 1,
                            match.Groups[Regex.VALID_MENTION_OR_LIST_GROUP_LIST].Index + match.Groups[Regex.VALID_MENTION_OR_LIST_GROUP_LIST].Length,
                            match.Groups[Regex.VALID_MENTION_OR_LIST_GROUP_USERNAME].Value,
                            match.Groups[Regex.VALID_MENTION_OR_LIST_GROUP_LIST].Value,
                            TweetEntityType.Mention));
                    }
                }
            }
            return extracted;
        }

        /// <summary>
        /// Extract a @username reference from the beginning of Tweet text. A reply is an occurance of @username at the
        /// beginning of a Tweet, preceded by 0 or more spaces.
        /// </summary>
        /// <param name="text">text of the tweet from which to extract the replied to username</param>
        /// <returns>username referenced, if any (without the leading @ sign). Returns null if this is not a reply.</returns>
        public string ExtractReplyScreenname(string text) {
            if (text == null) {
                return null;
            }

            Match matcher = Regex.VALID_REPLY.Match(text);
            if (matcher.Success) {
                string after = text.Substring(matcher.Index + matcher.Length);
                if (Regex.INVALID_MENTION_MATCH_END.IsMatch(after)) {
                    return null;
                } else {
                    return matcher.Groups[Regex.VALID_REPLY_GROUP_USERNAME].Value;
                }
            } else {
                return null;
            }
        }

        /// <summary>
        /// Extract URL references from Tweet text.
        /// </summary>
        /// <param name="text">text of the tweet from which to extract URLs</param>
        /// <returns>List of URLs referenced.</returns>
        public List<string> ExtractURLs(string text) {
            if (string.IsNullOrWhiteSpace(text)) {
                return new List<string>();
            }

            List<string> urls = new List<string>();
            foreach (TweetEntity entity in ExtractURLsWithIndices(text)) {
                urls.Add(entity.Value);
            }
            return urls;
        }

        /// <summary>
        /// Extract URL references from Tweet text.
        /// </summary>
        /// <param name="text">text of the tweet from which to extract URLs</param>
        /// <returns>List of URLs referenced.</returns>
        public List<TweetEntity> ExtractURLsWithIndices(string text) {
            if (string.IsNullOrWhiteSpace(text)
                || (ExtractURLWithoutProtocol ? text.IndexOf('.') : text.IndexOf(':')) == -1) {
                // Performance optimization.
                // If text doesn't contain '.' or ':' at all, text doesn't contain URL,
                // so we can simply return an empty list.
                return new List<TweetEntity>();
            }

            List<TweetEntity> urls = new List<TweetEntity>();

            MatchCollection matcher = Regex.VALID_URL.Matches(text);
            foreach (Match match in matcher) {
                if (!match.Groups[Regex.VALID_URL_GROUP_PROTOCOL].Success) {
                    // Skip if protocol is not present and 'extractURLWithoutProtocol' is false or URL is preceded by invalid character.
                    if (!ExtractURLWithoutProtocol || Regex.INVALID_URL_WITHOUT_PROTOCOL_MATCH_BEGIN.IsMatch(match.Groups[Regex.VALID_URL_GROUP_BEFORE].Value)) {
                        continue;
                    }
                }
                string url = match.Groups[Regex.VALID_URL_GROUP_URL].Value;
                int start = match.Groups[Regex.VALID_URL_GROUP_URL].Index;
                int end = match.Groups[Regex.VALID_URL_GROUP_URL].Index + match.Groups[Regex.VALID_URL_GROUP_URL].Length;
                Match tco_matcher = Regex.VALID_TCO_URL.Match(url);
                if (tco_matcher.Success) {
                    // In the case of t.co URLs, don't allow additional path characters.
                    url = tco_matcher.Value;
                    end = start + url.Length;
                }
                urls.Add(new TweetEntity(start, end, url, TweetEntityType.Url));
            }
            return urls;
        }

        /// <summary>
        /// Extract #hashtag references from Tweet text.
        /// </summary>
        /// <param name="text">text of the tweet from which to extract hashtags</param>
        /// <returns>List of hashtags referenced (without the leading # sign)</returns>
        public List<string> ExtractHashtags(string text) {
            if (string.IsNullOrWhiteSpace(text)) {
                return new List<string>();
            }

            List<string> extracted = new List<string>();
            foreach (TweetEntity entity in ExtractHashtagsWithIndices(text)) {
                extracted.Add(entity.Value);
            }

            return extracted;
        }

        /// <summary>
        /// Extract #hashtag references from Tweet text.
        /// </summary>
        /// <param name="text">text of the tweet from which to extract hashtags</param>
        /// <returns>List of hashtags referenced (without the leading # sign)</returns>
        public List<TweetEntity> ExtractHashtagsWithIndices(string text) {
            return ExtractHashtagsWithIndices(text, true);
        }

        /// <summary>
        /// Extract #hashtag references from Tweet text.
        /// </summary>
        /// <param name="text">text of the tweet from which to extract hashtags</param>
        /// <param name="checkUrlOverlap">if true, check if extracted hashtags overlap URLs and remove overlapping ones</param>
        /// <returns>List of hashtags referenced (without the leading # sign)</returns>
        private List<TweetEntity> ExtractHashtagsWithIndices(string text, bool checkUrlOverlap) {
            if (string.IsNullOrWhiteSpace(text)) {
                return new List<TweetEntity>();
            }

            // Performance optimization.
            // If text doesn't contain #/＃ at all, text doesn't contain
            // hashtag, so we can simply return an empty list.
            bool found = false;
            foreach (char c in text) {
                if (c == '#' || c == '＃') {
                    found = true;
                    break;
                }
            }
            if (!found) {
                return new List<TweetEntity>();
            }

            List<TweetEntity> extracted = new List<TweetEntity>();
            MatchCollection matcher = Regex.VALID_HASHTAG.Matches(text);
            foreach (Match match in matcher) {
                string after = text.Substring(match.Index + match.Length);
                if (!Regex.INVALID_HASHTAG_MATCH_END.IsMatch(after)) {
                    extracted.Add(new TweetEntity(match, TweetEntityType.Hashtag, Regex.VALID_HASHTAG_GROUP_TAG));
                }
            }

            if (checkUrlOverlap) {
                // extract URLs
                List<TweetEntity> urls = ExtractURLsWithIndices(text);
                if (urls.Any()) {
                    extracted.AddRange(urls);
                    // remove overlap
                    RemoveOverlappingEntities(extracted);
                    // remove URL entities
                    extracted = extracted.Where(x => x.Type == TweetEntityType.Hashtag).ToList();
                }
            }

            return extracted;
        }

        /// <summary>
        /// Extract $cashtag references from Tweet text.
        /// </summary>
        /// <param name="text">text of the tweet from which to extract cashtags</param>
        /// <returns>List of cashtags referenced (without the leading $ sign)</returns>
        public List<string> ExtractCashtags(string text) {
            if (string.IsNullOrWhiteSpace(text)) {
                return new List<string>();
            }

            List<string> extracted = new List<string>();
            foreach (TweetEntity entity in ExtractCashtagsWithIndices(text)) {
                extracted.Add(entity.Value);
            }

            return extracted;
        }

        /// <summary>
        /// Extract $cashtag references from Tweet text.
        /// </summary>
        /// <param name="text">text of the tweet from which to extract cashtags</param>
        /// <returns>List of cashtags referenced (without the leading $ sign)</returns>
        public List<TweetEntity> ExtractCashtagsWithIndices(string text) {
            if (string.IsNullOrWhiteSpace(text)) {
                return new List<TweetEntity>();
            }

            // Performance optimization.
            // If text doesn't contain $, text doesn't contain
            // cashtag, so we can simply return an empty list.
            if (text.IndexOf('$') == -1) {
                return new List<TweetEntity>();
            }

            List<TweetEntity> extracted = new List<TweetEntity>();
            MatchCollection matcher = Regex.VALID_CASHTAG.Matches(text);
            foreach (Match match in matcher) {
                extracted.Add(new TweetEntity(match, TweetEntityType.Cashtag, Regex.VALID_CASHTAG_GROUP_CASHTAG));
            }

            return extracted;
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TwitterText {

    /// <summary>
    /// A class to extract usernames, lists, hashtags and URLs from Tweet text.
    /// </summary>
    public class Extractor {

        /// <summary>
        /// Indicates whether URLs without protocol should be extracted.
        /// </summary>
        public bool ExtractURLWithoutProtocol { get; set; }

         /// <summary>
        /// Initializes a new instance of the <see cref="Extractor"/> class.
         /// </summary>
        public Extractor() {
            ExtractURLWithoutProtocol = true;
        }

        /// <summary>
        /// Extract URLs, #hashtags, @mentions, lists and $cashtags from a given text/tweet.
        /// </summary>
        /// <param name="text">text of the tweet from which to extract entities.</param>
        /// <returns>List of entities referenced.</returns>
        public List<Entity> ExtractEntitiesWithIndices(string text) {
            List<Entity> entities = new List<Entity>();
            entities.AddRange(ExtractURLsWithIndices(text));
            entities.AddRange(ExtractHashtagsWithIndices(text, false));
            entities.AddRange(ExtractMentionsOrListsWithIndices(text));
            entities.AddRange(ExtractCashtagsWithIndices(text));
            entities = RemoveOverlappingEntities(entities);
            return entities;
        }
       
        /// <summary>
        /// Extract @username references from Tweet text. A mention is an occurance of @username anywhere in a Tweet.
        /// </summary>
        /// <param name="text">text of the tweet from which to extract usernames.</param>
        /// <returns>List of usernames referenced (without the leading @ sign).</returns>
        public List<String> ExtractMentionedScreennames(string text) {
            if (string.IsNullOrEmpty(text)) {
                return new List<string>();
            }

            List<string> extracted = new List<string>();
            foreach (Entity entity in ExtractMentionedScreennamesWithIndices(text)) {
                extracted.Add(entity.Value);
            }
            return extracted;
        }

        /// <summary>
        /// Extract @username references from Tweet text. A mention is an occurance of @username anywhere in a Tweet.
        /// </summary>
        /// <param name="text">text of the tweet from which to extract usernames.</param>
        /// <returns>List of usernames referenced (without the leading @ sign).</returns>
        public List<Entity> ExtractMentionedScreennamesWithIndices(string text) {
            List<Entity> extracted = new List<Entity>();
            foreach (Entity entity in ExtractMentionsOrListsWithIndices(text)) {
                if (entity.ListSlug == null) {
                    extracted.Add(entity);
                }
            }
            return extracted;
        }

        /// <summary>
        /// Extract @username or @username/listname references from Tweet text. 
        /// </summary>
        /// <param name="text">text of the tweet from which to extract mentions or lists.</param>
        /// <returns>List of usernames and lists referenced (without the leading @ sign).</returns>
        public List<Entity> ExtractMentionsOrListsWithIndices(string text) {
            if (string.IsNullOrEmpty(text)) {
                return new List<Entity>();
            }

            // Performance optimization. If text doesn't contain @/＠ at all, the text doesn't contain @mention. So we can simply return an empty list.
            bool found = false;
            foreach (char c in text) {
                if (c == '@' || c == '＠') {
                    found = true;
                    break;
                }
            }
            if (!found) {
                return new List<Entity>();
            }

            List<Entity> extracted = new List<Entity>();
            System.Text.RegularExpressions.MatchCollection matcher = Regex.VALID_MENTION_OR_LIST.Matches(text);
            foreach (System.Text.RegularExpressions.Match match in matcher) {
                String after = text.Substring(match.Index + match.Length);
                if (!Regex.INVALID_MENTION_MATCH_END.IsMatch(after)) {
                    if (!match.Groups[Regex.VALID_MENTION_OR_LIST_GROUP_LIST].Success) {
                        extracted.Add(new Entity(match, EntityType.Mention, Regex.VALID_MENTION_OR_LIST_GROUP_USERNAME));
                    } else {
                        extracted.Add(new Entity(match.Groups[Regex.VALID_MENTION_OR_LIST_GROUP_USERNAME].Index - 1,
                            match.Groups[Regex.VALID_MENTION_OR_LIST_GROUP_LIST].Index + match.Groups[Regex.VALID_MENTION_OR_LIST_GROUP_LIST].Length,
                            match.Groups[Regex.VALID_MENTION_OR_LIST_GROUP_USERNAME].Value,
                            match.Groups[Regex.VALID_MENTION_OR_LIST_GROUP_LIST].Value,
                            EntityType.Mention));
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

            System.Text.RegularExpressions.Match matcher = Regex.VALID_REPLY.Match(text);
            if (matcher.Success) {
                String after = text.Substring(matcher.Index + matcher.Length);
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
            if (string.IsNullOrEmpty(text)) {
                return new List<string>();
            }

            List<string> urls = new List<string>();
            foreach (Entity entity in ExtractURLsWithIndices(text)) {
                urls.Add(entity.Value);
            }
            return urls;
        }

        /// <summary>
        /// Extract URL references from Tweet text.
        /// </summary>
        /// <param name="text">text of the tweet from which to extract URLs.</param>
        /// <returns>List of URLs referenced.</returns>
        public List<Entity> ExtractURLsWithIndices(string text) {
            if (string.IsNullOrEmpty(text) || (ExtractURLWithoutProtocol ? text.IndexOf('.') : text.IndexOf(':')) == -1) {
                // Performance optimization. If text doesn't contain '.' or ':' at all, text doesn't contain URL, so we can simply return an empty list.
                return new List<Entity>();
            }

            List<Entity> urls = new List<Entity>();
            System.Text.RegularExpressions.MatchCollection matcher = Regex.VALID_URL.Matches(text);
            foreach (System.Text.RegularExpressions.Match match in matcher) {
                if (!match.Groups[Regex.VALID_URL_GROUP_PROTOCOL].Success) {
                    // Skip if protocol is not present and 'extractURLWithoutProtocol' is false or URL is preceded by invalid character.
                    if (!ExtractURLWithoutProtocol || Regex.INVALID_URL_WITHOUT_PROTOCOL_MATCH_BEGIN.IsMatch(match.Groups[Regex.VALID_URL_GROUP_BEFORE].Value)) {
                        continue;
                    }
                }
                String url = match.Groups[Regex.VALID_URL_GROUP_URL].Value;
                int start = match.Groups[Regex.VALID_URL_GROUP_URL].Index;
                int end = match.Groups[Regex.VALID_URL_GROUP_URL].Index + match.Groups[Regex.VALID_URL_GROUP_URL].Length;
                System.Text.RegularExpressions.Match tco_matcher = Regex.VALID_TCO_URL.Match(url);
                if (tco_matcher.Success) {
                    // In the case of t.co URLs, don't allow additional path characters.
                    url = tco_matcher.Value;
                    end = start + url.Length;
                }
                urls.Add(new Entity(start, end, url, EntityType.Url));
            }
            return urls;
        }

        /// <summary>
        /// Extract #hashtag references from Tweet text.
        /// </summary>
        /// <param name="text">text of the tweet from which to extract hashtags.</param>
        /// <returns>List of hashtags referenced (without the leading # sign).</returns>
        public List<string> ExtractHashtags(string text) {
            if (string.IsNullOrEmpty(text)) {
                return new List<string>();
            }

            List<string> extracted = new List<string>();
            foreach (Entity entity in ExtractHashtagsWithIndices(text)) {
                extracted.Add(entity.Value);
            }

            return extracted;
        }

        /// <summary>
        /// Extract #hashtag references from Tweet text. 
        /// </summary>
        /// <param name="text">text of the tweet from which to extract hashtags.</param>
        /// <returns>List of hashtags referenced (without the leading # sign)List of hashtags referenced (without the leading # sign).</returns>
        public List<Entity> ExtractHashtagsWithIndices(string text) {
            return ExtractHashtagsWithIndices(text, true);
        }

        /// <summary>
        /// Extract #hashtag references from Tweet text.
        /// </summary>
        /// <param name="text">text of the tweet from which to extract hashtags</param>
        /// <param name="checkUrlOverlap">checkUrlOverlap if true, check if extracted hashtags overlap URLs and remove overlapping ones</param>
        /// <returns>List of hashtags referenced (without the leading # sign)</returns>
        private List<Entity> ExtractHashtagsWithIndices(string text, bool checkUrlOverlap) {
            if (string.IsNullOrEmpty(text)) {
                return new List<Entity>();
            }

            // Performance optimization. If text doesn't contain #/＃ at all, text doesn't contain hashtag, so we can simply return an empty list.
            bool found = false;
            foreach (char c in text) {
                if (c == '#' || c == '＃') {
                    found = true;
                    break;
                }
            }
            if (!found) {
                return new List<Entity>();
            }

            List<Entity> extracted = new List<Entity>();
            System.Text.RegularExpressions.MatchCollection matcher = Regex.VALID_HASHTAG.Matches(text);
            foreach (System.Text.RegularExpressions.Match match in matcher) {
                string after = text.Substring(match.Index + match.Length);
                if (!Regex.INVALID_HASHTAG_MATCH_END.IsMatch(after)) {
                    extracted.Add(new Entity(match, EntityType.Hashtag, Regex.VALID_HASHTAG_GROUP_TAG));
                }
            }

            if (checkUrlOverlap) {
                // extract URLs
                List<Entity> urls = ExtractURLsWithIndices(text);
                if (urls.Any()) {
                    extracted.AddRange(urls);
                    // remove overlap
                    extracted = RemoveOverlappingEntities(extracted);
                    // remove URL entities
                    extracted = extracted.Where(x => x.Type == EntityType.Hashtag).ToList();
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
            if (string.IsNullOrEmpty(text)) {
                return new List<string>();
            }

            List<string> extracted = new List<string>();
            foreach (Entity entity in ExtractCashtagsWithIndices(text)) {
                extracted.Add(entity.Value);
            }

            return extracted;
        }

        /// <summary>
        /// Extract $cashtag references from Tweet text.
        /// </summary>
        /// <param name="text">text of the tweet from which to extract cashtags</param>
        /// <returns>List of cashtags referenced (without the leading $ sign)</returns>
        public List<Entity> ExtractCashtagsWithIndices(string text) {
            if (string.IsNullOrEmpty(text)) {
                return new List<Entity>();
            }

            // Performance optimization. If text doesn't contain $, text doesn't contain cashtag, so we can simply return an empty list.
            if (text.IndexOf('$') == -1) {
                return new List<Entity>();
            }

            List<Entity> extracted = new List<Entity>();
            System.Text.RegularExpressions.MatchCollection matcher = Regex.VALID_CASHTAG.Matches(text);
            foreach (System.Text.RegularExpressions.Match match in matcher) {
                extracted.Add(new Entity(match, EntityType.Cashtag, Regex.VALID_CASHTAG_GROUP_CASHTAG));
            }

            return extracted;
        }

        /// <summary>
        /// Remove overlapping entities. Two entities overlap only when one is URL and the other is hashtag/mention
        /// which is a part of the URL. When it happens, we choose URL over hashtag/mention by selecting the one with smaller start index.
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        private List<Entity> RemoveOverlappingEntities(List<Entity> entities) {

            // Sort by index
            entities.Sort(new StartIndexComparer());

            // Remove overlapping entities. Two entities overlap only when one is URL and the other is hashtag/mention which is a part of the URL. 
            // When it happens, we choose URL over hashtag/mention by selecting the one with smaller start index.
            List<Entity> overlapping = new List<Entity>();
            if (entities.Any()) {
                Entity prev = null;
                foreach (var cur in entities) {
                    if (prev != null && prev.End > cur.Start) {
                        overlapping.Add(cur);
                    } else {
                        prev = cur;
                    }
                }
            }
            return entities.Except(overlapping).ToList();
        }

        /// <summary>
        /// Compares entities bases on start index.
        /// </summary>
        private class StartIndexComparer : Comparer<Entity> {
            public override int Compare(Entity a, Entity b) {
                if (a.Start > b.Start) {
                    return 1;
                } if (a.Start < b.Start) {
                    return -1;
                } else {
                    return 0;
                }
            }
        }
    }
}

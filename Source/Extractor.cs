using System;
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
        protected bool extractURLWithoutProtocol = true;

        /**
         * Create a new extractor.
         */
        public Extractor() {
        }

        private void removeOverlappingEntities(List<Entity> entities) {
            // TODO: translate to C#
            //    // sort by index
            //    Collections.<Entity>sort(entities, new Comparator<Entity>() {
            //      public int compare(Entity e1, Entity e2) {
            //        return e1.start - e2.start;
            //      }
            //    });

            //    // Remove overlapping entities.
            //    // Two entities overlap only when one is URL and the other is hashtag/mention
            //    // which is a part of the URL. When it happens, we choose URL over hashtag/mention
            //    // by selecting the one with smaller start index.
            //    if (!entities.isEmpty()) {
            //      Iterator<Entity> it = entities.iterator();
            //      Entity prev = it.next();
            //      while (it.hasNext()) {
            //        Entity cur = it.next();
            //        if (prev.getEnd() > cur.getStart()) {
            //          it.remove();
            //        } else {
            //          prev = cur;
            //        }
            //      }
            //    }
        }

        /**
         * Extract URLs, @mentions, lists and #hashtag from a given text/tweet.
         * @param text text of tweet
         * @return list of extracted entities
         */
        public List<Entity> extractEntitiesWithIndices(String text) {
            List<Entity> entities = new List<Entity>();
            entities.AddRange(extractURLsWithIndices(text));
            entities.AddRange(extractHashtagsWithIndices(text, false));
            entities.AddRange(extractMentionsOrListsWithIndices(text));
            entities.AddRange(extractCashtagsWithIndices(text));

            removeOverlappingEntities(entities);
            return entities;
        }

        /**
         * Extract @username references from Tweet text. A mention is an occurance of @username anywhere in a Tweet.
         *
         * @param text of the tweet from which to extract usernames
         * @return List of usernames referenced (without the leading @ sign)
         */
        public List<String> extractMentionedScreennames(String text) {
            if (string.IsNullOrEmpty(text)) {
                return new List<string>();
            }

            List<String> extracted = new List<String>();
            //for (Entity entity : extractMentionedScreennamesWithIndices(text)) {
            foreach (Entity entity in extractMentionedScreennamesWithIndices(text)) {
                extracted.Add(entity.value);
            }
            return extracted;
        }

        /**
         * Extract @username references from Tweet text. A mention is an occurance of @username anywhere in a Tweet.
         *
         * @param text of the tweet from which to extract usernames
         * @return List of usernames referenced (without the leading @ sign)
         */
        public List<Entity> extractMentionedScreennamesWithIndices(String text) {
            List<Entity> extracted = new List<Entity>();
            //for (Entity entity : extractMentionsOrListsWithIndices(text)) {
            foreach (Entity entity in extractMentionsOrListsWithIndices(text)) {
                if (entity.listSlug == null) {
                    extracted.Add(entity);
                }
            }
            return extracted;
        }

        public List<Entity> extractMentionsOrListsWithIndices(String text) {
            if (string.IsNullOrEmpty(text)) {
                return new List<Entity>();
            }

            // Performance optimization.
            // If text doesn't contain @/＠ at all, the text doesn't
            // contain @mention. So we can simply return an empty list.
            bool found = false;
            //for (char c : text.toCharArray()) {
            foreach (char c in text) {
                if (c == '@' || c == '＠') {
                    found = true;
                    break;
                }
            }
            if (!found) {
                return new List<Entity>();
            }

            //List<Entity> extracted = new ArrayList<Entity>();
            //Matcher matcher = Regex.VALID_MENTION_OR_LIST.matcher(text);
            //while (matcher.find()) {
            //  String after = text.substring(matcher.end());
            //  if (! Regex.INVALID_MENTION_MATCH_END.matcher(after).find()) {
            //    if (matcher.group(Regex.VALID_MENTION_OR_LIST_GROUP_LIST) == null) {
            //      extracted.add(new Entity(matcher, Entity.Type.MENTION, Regex.VALID_MENTION_OR_LIST_GROUP_USERNAME));
            //    } else {
            //      extracted.add(new Entity(matcher.start(Regex.VALID_MENTION_OR_LIST_GROUP_USERNAME) - 1,
            //          matcher.end(Regex.VALID_MENTION_OR_LIST_GROUP_LIST),
            //          matcher.group(Regex.VALID_MENTION_OR_LIST_GROUP_USERNAME),
            //          matcher.group(Regex.VALID_MENTION_OR_LIST_GROUP_LIST),
            //          Entity.Type.MENTION));
            //    }
            //  }
            //}

            // REVIEW: är detta rätt?
            List<Entity> extracted = new List<Entity>();
            System.Text.RegularExpressions.MatchCollection matcher = Regex.VALID_MENTION_OR_LIST.Matches(text);
            foreach (System.Text.RegularExpressions.Match match in matcher) {
                String after = text.Substring(match.Index + match.Length);
                if (!Regex.INVALID_MENTION_MATCH_END.IsMatch(after)) {

                    //if (match.Groups[Regex.VALID_MENTION_OR_LIST_GROUP_LIST].Value == null) {
                    if (!match.Groups[Regex.VALID_MENTION_OR_LIST_GROUP_LIST].Success) {
                        extracted.Add(new Entity(match, EntityType.MENTION, Regex.VALID_MENTION_OR_LIST_GROUP_USERNAME));
                    } else {
                        extracted.Add(new Entity(match.Groups[Regex.VALID_MENTION_OR_LIST_GROUP_USERNAME].Index - 1,
                            match.Groups[Regex.VALID_MENTION_OR_LIST_GROUP_LIST].Index + match.Groups[Regex.VALID_MENTION_OR_LIST_GROUP_LIST].Length,
                            match.Groups[Regex.VALID_MENTION_OR_LIST_GROUP_USERNAME].Value,
                            match.Groups[Regex.VALID_MENTION_OR_LIST_GROUP_LIST].Value,
                            EntityType.MENTION));
                    }
                }
            }

            return extracted;
        }

        /**
         * Extract a @username reference from the beginning of Tweet text. A reply is an occurance of @username at the
         * beginning of a Tweet, preceded by 0 or more spaces.
         *
         * @param text of the tweet from which to extract the replied to username
         * @return username referenced, if any (without the leading @ sign). Returns null if this is not a reply.
         */
        public String extractReplyScreenname(String text) {
            if (text == null) {
                return null;
            }

            //Matcher matcher = Regex.VALID_REPLY.matcher(text);
            // if (matcher.find()) {
            //   String after = text.substring(matcher.end());
            //   if (Regex.INVALID_MENTION_MATCH_END.matcher(after).find()) {
            //     return null;
            //   } else {
            //     return matcher.group(Regex.VALID_REPLY_GROUP_USERNAME);
            //   }
            // } else {
            //   return null;
            // }

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

        /**
         * Extract URL references from Tweet text.
         *
         * @param text of the tweet from which to extract URLs
         * @return List of URLs referenced.
         */
        public List<String> extractURLs(String text) {
            if (string.IsNullOrEmpty(text)) {
                return new List<string>();
            }

            List<String> urls = new List<String>();
            //for (Entity entity : extractURLsWithIndices(text)) {
            foreach (Entity entity in extractURLsWithIndices(text)) {
                urls.Add(entity.value);
            }
            return urls;
        }

        /**
         * Extract URL references from Tweet text.
         *
         * @param text of the tweet from which to extract URLs
         * @return List of URLs referenced.
         */
        public List<Entity> extractURLsWithIndices(String text) {
            if (string.IsNullOrEmpty(text) || (extractURLWithoutProtocol ? text.IndexOf('.') : text.IndexOf(':')) == -1) {
                // Performance optimization.
                // If text doesn't contain '.' or ':' at all, text doesn't contain URL,
                // so we can simply return an empty list.
                return new List<Entity>();
            }

            List<Entity> urls = new List<Entity>();

            //Matcher matcher = Regex.VALID_URL.matcher(text);
            //while (matcher.find()) {
            //  if (matcher.group(Regex.VALID_URL_GROUP_PROTOCOL) == null) {
            //    // skip if protocol is not present and 'extractURLWithoutProtocol' is false
            //    // or URL is preceded by invalid character.
            //    if (!extractURLWithoutProtocol
            //        || Regex.INVALID_URL_WITHOUT_PROTOCOL_MATCH_BEGIN
            //                .matcher(matcher.group(Regex.VALID_URL_GROUP_BEFORE)).matches()) {
            //      continue;
            //    }
            //  }
            //  String url = matcher.group(Regex.VALID_URL_GROUP_URL);
            //  int start = matcher.start(Regex.VALID_URL_GROUP_URL);
            //  int end = matcher.end(Regex.VALID_URL_GROUP_URL);
            //  Matcher tco_matcher = Regex.VALID_TCO_URL.matcher(url);
            //  if (tco_matcher.find()) {
            //    // In the case of t.co URLs, don't allow additional path characters.
            //    url = tco_matcher.group();
            //    end = start + url.length();
            //  }

            //  urls.add(new Entity(start, end, url, Entity.Type.URL));
            //}

            System.Text.RegularExpressions.MatchCollection matcher = Regex.VALID_URL.Matches(text);
            foreach (System.Text.RegularExpressions.Match match in matcher) {

                //if (match.Groups[Regex.VALID_URL_GROUP_PROTOCOL].Value == null) {
                if (!match.Groups[Regex.VALID_URL_GROUP_PROTOCOL].Success) {
                    // skip if protocol is not present and 'extractURLWithoutProtocol' is false
                    // or URL is preceded by invalid character.
                    if (!extractURLWithoutProtocol || Regex.INVALID_URL_WITHOUT_PROTOCOL_MATCH_BEGIN.IsMatch(match.Groups[Regex.VALID_URL_GROUP_BEFORE].Value)) {
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

                urls.Add(new Entity(start, end, url, EntityType.URL));

            }


            return urls;
        }


        /**
         * Extract #hashtag references from Tweet text.
         *
         * @param text of the tweet from which to extract hashtags
         * @return List of hashtags referenced (without the leading # sign)
         */
        public List<String> extractHashtags(String text) {
            if (string.IsNullOrEmpty(text)) {
                return new List<string>();
            }

            List<String> extracted = new List<String>();
            //for (Entity entity : extractHashtagsWithIndices(text)) {
            foreach (Entity entity in extractHashtagsWithIndices(text)) {
                extracted.Add(entity.value);
            }

            return extracted;
        }

        /**
         * Extract #hashtag references from Tweet text.
         *
         * @param text of the tweet from which to extract hashtags
         * @return List of hashtags referenced (without the leading # sign)
         */
        public List<Entity> extractHashtagsWithIndices(String text) {
            return extractHashtagsWithIndices(text, true);
        }

        /**
         * Extract #hashtag references from Tweet text.
         *
         * @param text of the tweet from which to extract hashtags
         * @param checkUrlOverlap if true, check if extracted hashtags overlap URLs and remove overlapping ones
         * @return List of hashtags referenced (without the leading # sign)
         */
        private List<Entity> extractHashtagsWithIndices(String text, bool checkUrlOverlap) {
            if (string.IsNullOrEmpty(text)) {
                return new List<Entity>();
            }

            // Performance optimization.
            // If text doesn't contain #/＃ at all, text doesn't contain
            // hashtag, so we can simply return an empty list.
            bool found = false;
            //for (char c : text.toCharArray()) {
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

            //Matcher matcher = Regex.VALID_HASHTAG.matcher(text);
            //while (matcher.find()) {
            //  String after = text.substring(matcher.end());
            //  if (!Regex.INVALID_HASHTAG_MATCH_END.matcher(after).find()) {
            //    extracted.add(new Entity(matcher, Entity.Type.HASHTAG, Regex.VALID_HASHTAG_GROUP_TAG));
            //  }
            //}

            //if (checkUrlOverlap) {
            //  // extract URLs
            //  List<Entity> urls = extractURLsWithIndices(text);
            //  if (!urls.isEmpty()) {
            //    extracted.addAll(urls);
            //    // remove overlap
            //    removeOverlappingEntities(extracted);
            //    // remove URL entities
            //    Iterator<Entity> it = extracted.iterator();
            //    while (it.hasNext()) {
            //      Entity entity = it.next();
            //      if (entity.getType() != Entity.Type.HASHTAG) {
            //        it.remove();
            //      }
            //    }
            //  }
            //}

            System.Text.RegularExpressions.MatchCollection matcher = Regex.VALID_HASHTAG.Matches(text);
            foreach (System.Text.RegularExpressions.Match match in matcher) {


                String after = text.Substring(match.Index + match.Length);
                if (!Regex.INVALID_HASHTAG_MATCH_END.IsMatch(after)) {
                    extracted.Add(new Entity(match, EntityType.HASHTAG, Regex.VALID_HASHTAG_GROUP_TAG));
                }
            }

            if (checkUrlOverlap) {
                // extract URLs
                List<Entity> urls = extractURLsWithIndices(text);
                if (urls.Any()) {
                    extracted.AddRange(urls);
                    // remove overlap
                    removeOverlappingEntities(extracted);
                    // remove URL entities
                    //Iterator<Entity> it = extracted.iterator();
                    //while (it.hasNext()) {
                    //  Entity entity = it.next();
                    //  if (entity.getType() != Entity.Type.HASHTAG) {
                    //    it.remove();
                    //  }
                    //}
                    extracted = extracted.Where(x => x.getType() != EntityType.HASHTAG).ToList();
                }
            }

            return extracted;
        }

        /**
         * Extract $cashtag references from Tweet text.
         *
         * @param text of the tweet from which to extract cashtags
         * @return List of cashtags referenced (without the leading $ sign)
         */
        public List<String> extractCashtags(String text) {
            if (string.IsNullOrEmpty(text)) {
                return new List<string>();
            }

            List<String> extracted = new List<String>();
            //for (Entity entity : extractCashtagsWithIndices(text)) {
            foreach (Entity entity in extractCashtagsWithIndices(text)) {
                extracted.Add(entity.value);
            }

            return extracted;
        }

        /**
         * Extract $cashtag references from Tweet text.
         *
         * @param text of the tweet from which to extract cashtags
         * @return List of cashtags referenced (without the leading $ sign)
         */
        public List<Entity> extractCashtagsWithIndices(String text) {
            if (string.IsNullOrEmpty(text)) {
                return new List<Entity>();
            }

            // Performance optimization.
            // If text doesn't contain $, text doesn't contain
            // cashtag, so we can simply return an empty list.
            if (text.IndexOf('$') == -1) {
                return new List<Entity>();
            }

            List<Entity> extracted = new List<Entity>();

            //Matcher matcher = Regex.VALID_CASHTAG.matcher(text);
            //while (matcher.find()) {
            //  extracted.add(new Entity(matcher, Entity.Type.CASHTAG, Regex.VALID_CASHTAG_GROUP_CASHTAG));
            //}

            System.Text.RegularExpressions.MatchCollection matcher = Regex.VALID_CASHTAG.Matches(text);
            foreach (System.Text.RegularExpressions.Match match in matcher) {
                extracted.Add(new Entity(match, EntityType.CASHTAG, Regex.VALID_CASHTAG_GROUP_CASHTAG));
            }

            return extracted;
        }

        public void setExtractURLWithoutProtocol(bool extractURLWithoutProtocol) {
            this.extractURLWithoutProtocol = extractURLWithoutProtocol;
        }

        public bool isExtractURLWithoutProtocol() {
            return extractURLWithoutProtocol;
        }

        /*
         * Modify Unicode-based indices of the entities to UTF-16 based indices.
         *
         * In UTF-16 based indices, Unicode supplementary characters are counted as two characters.
         *
         * This method requires that the list of entities be in ascending order by start index.
         *
         * @param text original text
         * @param entities entities with Unicode based indices
         */
        public void modifyIndicesFromUnicodeToUTF16(String text, List<Entity> entities) {
            // TODO: convert to C#
            //IndexConverter convert = new IndexConverter(text);

            //foreach (Entity entity in entities) {
            //    entity.start = convert.codePointsToCodeUnits(entity.start);
            //    entity.end = convert.codePointsToCodeUnits(entity.end);
            //}
        }

        /*
         * Modify UTF-16-based indices of the entities to Unicode-based indices.
         *
         * In Unicode-based indices, Unicode supplementary characters are counted as single characters.
         *
         * This method requires that the list of entities be in ascending order by start index.
         *
         * @param text original text
         * @param entities entities with UTF-16 based indices
         */
        public void modifyIndicesFromUTF16ToToUnicode(String text, List<Entity> entities) {
            // TODO: convert to C#
            //IndexConverter convert = new IndexConverter(text);

            //foreach (Entity entity in entities) {
            //    entity.start = convert.codeUnitsToCodePoints(entity.start);
            //    entity.end = convert.codeUnitsToCodePoints(entity.end);
            //}
        }

        ///**
        // * An efficient converter of indices between code points and code units.
        // */
        //private sealed class IndexConverter {
        //    protected readonly String text;

        //    // Keep track of a single corresponding pair of code unit and code point
        //    // offsets so that we can re-use counting work if the next requested
        //    // entity is near the most recent entity.
        //    protected int codePointIndex = 0;
        //    protected int charIndex = 0;

        //    IndexConverter(String text) {
        //        this.text = text;
        //    }

        //    /**
        //     * @param charIndex Index into the string measured in code units.
        //     * @return The code point index that corresponds to the specified character index.
        //     */
        //    int codeUnitsToCodePoints(int charIndex) {
        //        if (charIndex < this.charIndex) {
        //            this.codePointIndex -= text.codePointCount(charIndex, this.charIndex);
        //        } else {
        //            this.codePointIndex += text.codePointCount(this.charIndex, charIndex);
        //        }
        //        this.charIndex = charIndex;

        //        // Make sure that charIndex never points to the second code unit of a
        //        // surrogate pair.
        //        if (charIndex > 0 && Character.isSupplementaryCodePoint(text.codePointAt(charIndex - 1))) {
        //            this.charIndex -= 1;
        //        }
        //        return this.codePointIndex;
        //    }

        //    /**
        //     * @param codePointIndex Index into the string measured in code points.
        //     * @return the code unit index that corresponds to the specified code point index.
        //     */
        //    int codePointsToCodeUnits(int codePointIndex) {
        //        // Note that offsetByCodePoints accepts negative indices.
        //        this.charIndex = text.offsetByCodePoints(this.charIndex, codePointIndex - this.codePointIndex);
        //        this.codePointIndex = codePointIndex;
        //        return this.charIndex;
        //    }
        //}
    }
}

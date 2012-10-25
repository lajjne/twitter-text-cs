using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TwitterText {
    /// <summary>
    /// A class for adding HTML links to hashtag, username and list references in Tweet text.
    /// </summary>
    public class Autolink {
        /** Default CSS class for auto-linked list URLs */
        public static readonly string DEFAULT_LIST_CLASS = "tweet-url list-slug";
        /** Default CSS class for auto-linked username URLs */
        public static readonly string DEFAULT_USERNAME_CLASS = "tweet-url username";
        /** Default CSS class for auto-linked hashtag URLs */
        public static readonly string DEFAULT_HASHTAG_CLASS = "tweet-url hashtag";
        /** Default CSS class for auto-linked cashtag URLs */
        public static readonly string DEFAULT_CASHTAG_CLASS = "tweet-url cashtag";
        /** Default href for username links (the username without the @ will be appended) */
        public static readonly string DEFAULT_USERNAME_URL_BASE = "https://twitter.com/";
        /** Default href for list links (the username/list without the @ will be appended) */
        public static readonly string DEFAULT_LIST_URL_BASE = "https://twitter.com/";
        /** Default href for hashtag links (the hashtag without the # will be appended) */
        public static readonly string DEFAULT_HASHTAG_URL_BASE = "https://twitter.com/#!/search?q=%23";
        /** Default href for cashtag links (the cashtag without the $ will be appended) */
        public static readonly string DEFAULT_CASHTAG_URL_BASE = "https://twitter.com/#!/search?q=%24";
        /** Default attribute for invisible span tag */
        public static readonly string DEFAULT_INVISIBLE_TAG_ATTRS = "style='position:absolute;left:-9999px;'";

        public interface LinkAttributeModifier {
            void modify(Entity entity, IDictionary<string, string> attributes);
        };

        public interface LinkTextModifier {
            string modify(Entity entity, string text);
        }

        protected string urlClass = null;
        protected string listClass;
        protected string usernameClass;
        protected string hashtagClass;
        protected string cashtagClass;
        protected string usernameUrlBase;
        protected string listUrlBase;
        protected string hashtagUrlBase;
        protected string cashtagUrlBase;
        protected string invisibleTagAttrs;
        protected bool noFollow = true;
        protected bool usernameIncludeSymbol = false;
        protected string symbolTag = null;
        protected string textWithSymbolTag = null;
        protected string urlTarget = null;
        protected LinkAttributeModifier linkAttributeModifier = null;
        protected LinkTextModifier linkTextModifier = null;

        private Extractor extractor = new Extractor();

        private static string escapeHTML(string text) {
            StringBuilder builder = new StringBuilder(text.Length * 2);
            for (int i = 0; i < text.Length; i++) {
                char c = text[i];
                switch (c) {
                    case '&': builder.Append("&amp;"); break;
                    case '>': builder.Append("&gt;"); break;
                    case '<': builder.Append("&lt;"); break;
                    case '"': builder.Append("&quot;"); break;
                    case '\'': builder.Append("&#39;"); break;
                    default: builder.Append(c); break;
                }
            }
            return builder.ToString();
        }

        public Autolink() {
            urlClass = null;
            listClass = DEFAULT_LIST_CLASS;
            usernameClass = DEFAULT_USERNAME_CLASS;
            hashtagClass = DEFAULT_HASHTAG_CLASS;
            cashtagClass = DEFAULT_CASHTAG_CLASS;
            usernameUrlBase = DEFAULT_USERNAME_URL_BASE;
            listUrlBase = DEFAULT_LIST_URL_BASE;
            hashtagUrlBase = DEFAULT_HASHTAG_URL_BASE;
            cashtagUrlBase = DEFAULT_CASHTAG_URL_BASE;
            invisibleTagAttrs = DEFAULT_INVISIBLE_TAG_ATTRS;

            extractor.setExtractURLWithoutProtocol(false);
        }

        public string escapeBrackets(string text) {
            int len = text.Length;
            if (len == 0)
                return text;

            StringBuilder sb = new StringBuilder(len + 16);
            for (int i = 0; i < len; ++i) {
                char c = text[i];
                if (c == '>')
                    sb.Append("&gt;");
                else if (c == '<')
                    sb.Append("&lt;");
                else
                    sb.Append(c);
            }
            return sb.ToString();
        }

        public void linkToText(Entity entity, string text, IDictionary<string, string> attributes, StringBuilder builder) {
            if (noFollow) {
                attributes["rel"] = "nofollow";
            }
            if (linkAttributeModifier != null) {
                linkAttributeModifier.modify(entity, attributes);
            }
            if (linkTextModifier != null) {
                text = linkTextModifier.modify(entity, text);
            }
            // append <a> tag
            builder.Append("<a");
            foreach (var entry in attributes) {
                builder.Append(" ").Append(escapeHTML(entry.Key)).Append("=\"").Append(escapeHTML(entry.Value)).Append("\"");
            }
            builder.Append(">").Append(text).Append("</a>");
        }

        public void linkToTextWithSymbol(Entity entity, string symbol, string text, IDictionary<string, string> attributes, StringBuilder builder) {
            //string taggedSymbol = string.IsNullOrEmpty(symbolTag) ? symbol : string.format("<%s>%s</%s>", symbolTag, symbol, symbolTag);
            string taggedSymbol = string.IsNullOrEmpty(symbolTag) ? symbol : string.Format("<{0}>{1}</{0}>", symbolTag, symbol);
            text = escapeHTML(text);
            //string taggedText = string.IsNullOrEmpty(textWithSymbolTag) ? text : string.format("<%s>%s</%s>", textWithSymbolTag, text, textWithSymbolTag);
            string taggedText = string.IsNullOrEmpty(textWithSymbolTag) ? text : string.Format("<{0}>{1}</{0}>", textWithSymbolTag, text);
            bool includeSymbol = usernameIncludeSymbol || !Regex.AT_SIGNS.IsMatch(symbol);

            if (includeSymbol) {
                linkToText(entity, taggedSymbol.ToString() + taggedText, attributes, builder);
            } else {
                builder.Append(taggedSymbol);
                linkToText(entity, taggedText, attributes, builder);
            }
        }

        public void linkToHashtag(Entity entity, string text, StringBuilder builder) {
            // Get the original hash char from text as it could be a full-width char.
            string hashChar = text.Substring(entity.getStart(), 1);
            string hashtag = entity.getValue();

            IDictionary<string, string> attrs = new Dictionary<string, string>();
            attrs["href"] = hashtagUrlBase + hashtag;
            attrs["title"] = "#" + hashtag;
            attrs["class"] = hashtagClass;

            linkToTextWithSymbol(entity, hashChar, hashtag, attrs, builder);
        }

        public void linkToCashtag(Entity entity, string text, StringBuilder builder) {
            string cashtag = entity.getValue();

            IDictionary<string, string> attrs = new Dictionary<string, string>();
            attrs["href"] = cashtagUrlBase + cashtag;
            attrs["title"] = "$" + cashtag;
            attrs["class"] = cashtagClass;

            linkToTextWithSymbol(entity, "$", cashtag, attrs, builder);
        }

        public void linkToMentionAndList(Entity entity, string text, StringBuilder builder) {
            string mention = entity.getValue();
            // Get the original at char from text as it could be a full-width char.
            string atChar = text.Substring(entity.getStart(), 1);

            IDictionary<string, string> attrs = new Dictionary<string, string>();
            if (entity.listSlug != null) {
                mention += entity.listSlug;
                attrs["class"] = listClass;
                attrs["href"] = listUrlBase + mention;
            } else {
                attrs["class"] = usernameClass;
                attrs["href"] = usernameUrlBase + mention;
            }

            linkToTextWithSymbol(entity, atChar, mention, attrs, builder);
        }

        public void linkToURL(Entity entity, string text, StringBuilder builder) {
            string url = entity.getValue();
            string linkText = escapeHTML(url);

            if (entity.displayURL != null && entity.expandedURL != null) {
                // Goal: If a user copies and pastes a tweet containing t.co'ed link, the resulting paste
                // should contain the full original URL (expanded_url), not the display URL.
                //
                // Method: Whenever possible, we actually emit HTML that contains expanded_url, and use
                // font-size:0 to hide those parts that should not be displayed (because they are not part of display_url).
                // Elements with font-size:0 get copied even though they are not visible.
                // Note that display:none doesn't work here. Elements with display:none don't get copied.
                //
                // Additionally, we want to *display* ellipses, but we don't want them copied.  To make this happen we
                // wrap the ellipses in a tco-ellipsis class and provide an onCopy handler that sets display:none on
                // everything with the tco-ellipsis class.
                //
                // As an example: The user tweets "hi http://longdomainname.com/foo"
                // This gets shortened to "hi http://t.co/xyzabc", with display_url = "…nname.com/foo"
                // This will get rendered as:
                // <span class='tco-ellipsis'> <!-- This stuff should get displayed but not copied -->
                //   …
                //   <!-- There's a chance the onCopy event handler might not fire. In case that happens,
                //        we include an &nbsp; here so that the … doesn't bump up against the URL and ruin it.
                //        The &nbsp; is inside the tco-ellipsis span so that when the onCopy handler *does*
                //        fire, it doesn't get copied.  Otherwise the copied text would have two spaces in a row,
                //        e.g. "hi  http://longdomainname.com/foo".
                //   <span style='font-size:0'>&nbsp;</span>
                // </span>
                // <span style='font-size:0'>  <!-- This stuff should get copied but not displayed -->
                //   http://longdomai
                // </span>
                // <span class='js-display-url'> <!-- This stuff should get displayed *and* copied -->
                //   nname.com/foo
                // </span>
                // <span class='tco-ellipsis'> <!-- This stuff should get displayed but not copied -->
                //   <span style='font-size:0'>&nbsp;</span>
                //   …
                // </span>
                //
                // Exception: pic.twitter.com images, for which expandedUrl = "https://twitter.com/#!/username/status/1234/photo/1
                // For those URLs, display_url is not a substring of expanded_url, so we don't do anything special to render the elided parts.
                // For a pic.twitter.com URL, the only elided part will be the "https://", so this is fine.
                string displayURLSansEllipses = entity.displayURL.Replace("…", "");
                int diplayURLIndexInExpandedURL = entity.expandedURL.IndexOf(displayURLSansEllipses);
                if (diplayURLIndexInExpandedURL != -1) {
                    string beforeDisplayURL = entity.expandedURL.Substring(0, diplayURLIndexInExpandedURL);
                    string afterDisplayURL = entity.expandedURL.Substring(diplayURLIndexInExpandedURL + displayURLSansEllipses.Length);
                    string precedingEllipsis = entity.displayURL.StartsWith("…") ? "…" : "";
                    string followingEllipsis = entity.displayURL.EndsWith("…") ? "…" : "";
                    string invisibleSpan = "<span " + invisibleTagAttrs + ">";

                    StringBuilder sb = new StringBuilder("<span class='tco-ellipsis'>");
                    sb.Append(precedingEllipsis);
                    sb.Append(invisibleSpan).Append("&nbsp;</span></span>");
                    sb.Append(invisibleSpan).Append(escapeHTML(beforeDisplayURL)).Append("</span>");
                    sb.Append("<span class='js-display-url'>").Append(escapeHTML(displayURLSansEllipses)).Append("</span>");
                    sb.Append(invisibleSpan).Append(escapeHTML(afterDisplayURL)).Append("</span>");
                    sb.Append("<span class='tco-ellipsis'>").Append(invisibleSpan).Append("&nbsp;</span>").Append(followingEllipsis).Append("</span>");

                    linkText = sb.ToString();
                } else {
                    linkText = entity.displayURL;
                }
            }

            IDictionary<string, string> attrs = new Dictionary<string, string>();
            attrs["href"] = url;

            if (!string.IsNullOrEmpty(urlClass)) {
                attrs["class"] = urlClass;
            }

            if (!string.IsNullOrEmpty(urlTarget)) {
                attrs["target"] = urlTarget;
            }
            linkToText(entity, linkText, attrs, builder);
        }

        public string autoLinkEntities(string text, List<Entity> entities) {
            StringBuilder builder = new StringBuilder(text.Length * 2);
            int beginIndex = 0;

            foreach (Entity entity in entities) {
                //builder.Append(text.subSequence(beginIndex, entity.start));
                builder.Append(text.Substring(beginIndex, entity.start - beginIndex));

                switch (entity.type) {
                    case EntityType.URL:
                        linkToURL(entity, text, builder);
                        break;
                    case EntityType.HASHTAG:
                        linkToHashtag(entity, text, builder);
                        break;
                    case EntityType.MENTION:
                        linkToMentionAndList(entity, text, builder);
                        break;
                    case EntityType.CASHTAG:
                        linkToCashtag(entity, text, builder);
                        break;
                }
                beginIndex = entity.end;
            }
            //builder.append(text.subSequence(beginIndex, text.length()));
            builder.Append(text.Substring(beginIndex, text.Length - beginIndex));

            return builder.ToString();
        }

        /**
         * Auto-link hashtags, URLs, usernames and lists.
         *
         * @param text of the Tweet to auto-link
         * @return text with auto-link HTML added
         */
        public string autoLink(string text) {
            text = escapeBrackets(text);

            // extract entities
            List<Entity> entities = extractor.extractEntitiesWithIndices(text);
            return autoLinkEntities(text, entities);
        }

        /**
         * Auto-link the @username and @username/list references in the provided text. Links to @username references will
         * have the usernameClass CSS classes added. Links to @username/list references will have the listClass CSS class
         * added.
         *
         * @param text of the Tweet to auto-link
         * @return text with auto-link HTML added
         */
        public string autoLinkUsernamesAndLists(string text) {
            return autoLinkEntities(text, extractor.extractMentionsOrListsWithIndices(text));
        }

        /**
         * Auto-link #hashtag references in the provided Tweet text. The #hashtag links will have the hashtagClass CSS class
         * added.
         *
         * @param text of the Tweet to auto-link
         * @return text with auto-link HTML added
         */
        public string autoLinkHashtags(string text) {
            return autoLinkEntities(text, extractor.extractHashtagsWithIndices(text));
        }

        /**
         * Auto-link URLs in the Tweet text provided.
         * <p/>
         * This only auto-links URLs with protocol.
         *
         * @param text of the Tweet to auto-link
         * @return text with auto-link HTML added
         */
        public string autoLinkURLs(string text) {
            return autoLinkEntities(text, extractor.extractURLsWithIndices(text));
        }

        /**
         * Auto-link $cashtag references in the provided Tweet text. The $cashtag links will have the cashtagClass CSS class
         * added.
         *
         * @param text of the Tweet to auto-link
         * @return text with auto-link HTML added
         */
        public string autoLinkCashtags(string text) {
            return autoLinkEntities(text, extractor.extractCashtagsWithIndices(text));
        }

        /**
         * @return CSS class for auto-linked URLs
         */
        public string getUrlClass() {
            return urlClass;
        }

        /**
         * Set the CSS class for auto-linked URLs
         *
         * @param urlClass new CSS value.
         */
        public void setUrlClass(string urlClass) {
            this.urlClass = urlClass;
        }

        /**
         * @return CSS class for auto-linked list URLs
         */
        public string getListClass() {
            return listClass;
        }

        /**
         * Set the CSS class for auto-linked list URLs
         *
         * @param listClass new CSS value.
         */
        public void setListClass(string listClass) {
            this.listClass = listClass;
        }

        /**
         * @return CSS class for auto-linked username URLs
         */
        public string getUsernameClass() {
            return usernameClass;
        }

        /**
         * Set the CSS class for auto-linked username URLs
         *
         * @param usernameClass new CSS value.
         */
        public void setUsernameClass(string usernameClass) {
            this.usernameClass = usernameClass;
        }

        /**
         * @return CSS class for auto-linked hashtag URLs
         */
        public string getHashtagClass() {
            return hashtagClass;
        }

        /**
         * Set the CSS class for auto-linked hashtag URLs
         *
         * @param hashtagClass new CSS value.
         */
        public void setHashtagClass(string hashtagClass) {
            this.hashtagClass = hashtagClass;
        }

        /**
         * @return CSS class for auto-linked cashtag URLs
         */
        public string getCashtagClass() {
            return cashtagClass;
        }

        /**
         * Set the CSS class for auto-linked cashtag URLs
         *
         * @param cashtagClass new CSS value.
         */
        public void setCashtagClass(string cashtagClass) {
            this.cashtagClass = cashtagClass;
        }

        /**
         * @return the href value for username links (to which the username will be appended)
         */
        public string getUsernameUrlBase() {
            return usernameUrlBase;
        }

        /**
         * Set the href base for username links.
         *
         * @param usernameUrlBase new href base value
         */
        public void setUsernameUrlBase(string usernameUrlBase) {
            this.usernameUrlBase = usernameUrlBase;
        }

        /**
         * @return the href value for list links (to which the username/list will be appended)
         */
        public string getListUrlBase() {
            return listUrlBase;
        }

        /**
         * Set the href base for list links.
         *
         * @param listUrlBase new href base value
         */
        public void setListUrlBase(string listUrlBase) {
            this.listUrlBase = listUrlBase;
        }

        /**
         * @return the href value for hashtag links (to which the hashtag will be appended)
         */
        public string getHashtagUrlBase() {
            return hashtagUrlBase;
        }

        /**
         * Set the href base for hashtag links.
         *
         * @param hashtagUrlBase new href base value
         */
        public void setHashtagUrlBase(string hashtagUrlBase) {
            this.hashtagUrlBase = hashtagUrlBase;
        }

        /**
         * @return the href value for cashtag links (to which the cashtag will be appended)
         */
        public string getCashtagUrlBase() {
            return cashtagUrlBase;
        }

        /**
         * Set the href base for cashtag links.
         *
         * @param cashtagUrlBase new href base value
         */
        public void setCashtagUrlBase(string cashtagUrlBase) {
            this.cashtagUrlBase = cashtagUrlBase;
        }

        /**
         * @return if the current URL links will include rel="nofollow" (true by default)
         */
        public bool isNoFollow() {
            return noFollow;
        }

        /**
         * Set if the current URL links will include rel="nofollow" (true by default)
         *
         * @param noFollow new noFollow value
         */
        public void setNoFollow(bool noFollow) {
            this.noFollow = noFollow;
        }

        /**
         * Set if the at mark '@' should be included in the link (false by default)
         *
         * @param noFollow new noFollow value
         */
        public void setUsernameIncludeSymbol(bool usernameIncludeSymbol) {
            this.usernameIncludeSymbol = usernameIncludeSymbol;
        }

        /**
         * Set HTML tag to be applied around #/@/# symbols in hashtags/usernames/lists/cashtag
         *
         * @param tag HTML tag without bracket. e.g., "b" or "s"
         */
        public void setSymbolTag(string tag) {
            this.symbolTag = tag;
        }

        /**
         * Set HTML tag to be applied around text part of hashtags/usernames/lists/cashtag
         *
         * @param tag HTML tag without bracket. e.g., "b" or "s"
         */
        public void setTextWithSymbolTag(string tag) {
            this.textWithSymbolTag = tag;
        }

        /**
         * Set the value of the target attribute in auto-linked URLs
         *
         * @param target target value e.g., "_blank"
         */
        public void setUrlTarget(string target) {
            this.urlTarget = target;
        }

        /**
         * Set a modifier to modify attributes of a link based on an entity
         *
         * @param modifier LinkAttributeModifier instance
         */
        public void setLinkAttributeModifier(LinkAttributeModifier modifier) {
            this.linkAttributeModifier = modifier;
        }

        /**
         * Set a modifier to modify text of a link based on an entity
         *
         * @param modifier LinkTextModifier instance
         */
        public void setLinkTextModifier(LinkTextModifier modifier) {
            this.linkTextModifier = modifier;
        }
    }
}





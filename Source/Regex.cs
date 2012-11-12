﻿
namespace TwitterText {

    /// <summary>
    /// Patterns and regular expressions used by the twitter text methods.
    /// </summary>
    public static class Regex {

        private const string UNICODE_SPACES = "[" +
          "\u0009-\u000d" +     // # White_Space # Cc [5]  <control-0009>..<control-000D>
          "\u0020" +            // White_Space # Zs        SPACE
          "\u0085" +            // White_Space # Cc        <control-0085>
          "\u00a0" +            // White_Space # Zs        NO-BREAK SPACE
          "\u1680" +            // White_Space # Zs        OGHAM SPACE MARK
          "\u180E" +            // White_Space # Zs        MONGOLIAN VOWEL SEPARATOR
          "\u2000-\u200a" +     // # White_Space # Zs [11] EN QUAD..HAIR SPACE
          "\u2028" +            // White_Space # Zl        LINE SEPARATOR
          "\u2029" +            // White_Space # Zp        PARAGRAPH SEPARATOR
          "\u202F" +            // White_Space # Zs        NARROW NO-BREAK SPACE
          "\u205F" +            // White_Space # Zs        MEDIUM MATHEMATICAL SPACE
          "\u3000" +            // White_Space # Zs        IDEOGRAPHIC SPACE
        "]";

        private const string ALNUM_CHARS = "a-zA-Z0-9";
        private const string ALNUM = "[" + ALNUM_CHARS + "]"; // \p{Alnum}
        private const string NOT_ALNUM = "[^" + ALNUM_CHARS + "]"; // \P{Alnum}

        private const string PUNCT_CHARS = "\\p{P}\\p{S}";
        private const string PUNCT = "[" + PUNCT_CHARS + "]"; // \p{Punct}

        private const string LATIN_ACCENTS_CHARS = "\u00c0-\u00d6\u00d8-\u00f6\u00f8-\u00ff" + // Latin-1
                                                    "\u0100-\u024f" + // Latin Extended A and B
                                                    "\u0253\u0254\u0256\u0257\u0259\u025b\u0263\u0268\u026f\u0272\u0289\u028b" + // IPA Extensions
                                                    "\u02bb" + // Hawaiian
                                                    "\u0300-\u036f" + // Combining diacritics
                                                    "\u1e00-\u1eff"; // Latin Extended Additional (mostly for Vietnamese)

        private const string HASHTAG_ALPHA_CHARS = "a-z" + LATIN_ACCENTS_CHARS +
                                                         "\u0400-\u04ff\u0500-\u0527" +  // Cyrillic
                                                         "\u2de0-\u2dff\ua640-\ua69f" +  // Cyrillic Extended A/B
                                                         "\u0591-\u05bf\u05c1-\u05c2\u05c4-\u05c5\u05c7" +
                                                         "\u05d0-\u05ea\u05f0-\u05f4" +  // Hebrew
                                                         "\ufb1d-\ufb28\ufb2a-\ufb36\ufb38-\ufb3c\ufb3e\ufb40-\ufb41" +
                                                         "\ufb43-\ufb44\ufb46-\ufb4f" +  // Hebrew Pres. Forms
                                                         "\u0610-\u061a\u0620-\u065f\u066e-\u06d3\u06d5-\u06dc" +
                                                         "\u06de-\u06e8\u06ea-\u06ef\u06fa-\u06fc\u06ff" + // Arabic
                                                         "\u0750-\u077f\u08a0\u08a2-\u08ac\u08e4-\u08fe" + // Arabic Supplement and Extended A
                                                         "\ufb50-\ufbb1\ufbd3-\ufd3d\\ufd50-\ufd8f\ufd92-\ufdc7\ufdf0-\ufdfb" + // Pres. Forms A
                                                         "\ufe70-\ufe74\ufe76-\ufefc" +  // Pres. Forms B
                                                         "\u200c" +                      // Zero-Width Non-Joiner
                                                         "\u0e01-\u0e3a\u0e40-\u0e4e" +  // Thai
                                                         "\u1100-\u11ff\u3130-\u3185\uA960-\uA97F\uAC00-\uD7AF\uD7B0-\uD7FF" + // Hangul (Korean)
                                                         "\\p{IsHiragana}\\p{IsKatakana}" + // Japanese Hiragana and Katakana
                                                         "\\p{IsCJKUnifiedIdeographs}" + // Japanese Kanji / Chinese Han
                                                         "\u3003\u3005\u303b" +          // Kanji/Han iteration marks
                                                         "\uff21-\uff3a\uff41-\uff5a" +  // full width Alphabet
                                                         "\uff66-\uff9f" +               // half width Katakana
                                                         "\uffa1-\uffdc";                // half width Hangul (Korean)
        private const string HASHTAG_ALPHA_NUMERIC_CHARS = "0-9\uff10-\uff19_" + HASHTAG_ALPHA_CHARS;
        private const string HASHTAG_ALPHA = "[" + HASHTAG_ALPHA_CHARS + "]";
        private const string HASHTAG_ALPHA_NUMERIC = "[" + HASHTAG_ALPHA_NUMERIC_CHARS + "]";

        /* URL related hash regex collection */
        private const string URL_VALID_PRECEEDING_CHARS = "(?:[^A-Z0-9@＠$#＃\u202A-\u202E]|^)";

        private const string URL_VALID_CHARS = "[" + ALNUM_CHARS + LATIN_ACCENTS_CHARS + "]";

        private const string URL_VALID_SUBDOMAIN = "(?:(?:" + URL_VALID_CHARS + "(?:" + URL_VALID_CHARS + "|[\\-_])*)?" + URL_VALID_CHARS + "\\.)";

        private const string URL_VALID_DOMAIN_NAME = "(?:(?:" + URL_VALID_CHARS + "(?:" + URL_VALID_CHARS + "|\\-)*)?" + URL_VALID_CHARS + "\\.)";

        /* Any non-space, non-punctuation characters. \p{Z} = any kind of whitespace or invisible separator. */
        private const string URL_VALID_UNICODE_CHARS = "[.[^" + PUNCT_CHARS + "\\s\\p{Z}\\p{IsGeneralPunctuation}]]";

        private const string URL_VALID_GTLD = "(?:(?:aero|asia|biz|cat|com|coop|edu|gov|info|int|jobs|mil|mobi|museum|name|net|org|pro|tel|travel|xxx)(?=" + NOT_ALNUM + "|$))";
        private const string URL_VALID_CCTLD =
            "(?:(?:ac|ad|ae|af|ag|ai|al|am|an|ao|aq|ar|as|at|au|aw|ax|az|ba|bb|bd|be|bf|bg|bh|bi|bj|bm|bn|bo|br|bs|bt|" +
            "bv|bw|by|bz|ca|cc|cd|cf|cg|ch|ci|ck|cl|cm|cn|co|cr|cs|cu|cv|cx|cy|cz|dd|de|dj|dk|dm|do|dz|ec|ee|eg|eh|" +
            "er|es|et|eu|fi|fj|fk|fm|fo|fr|ga|gb|gd|ge|gf|gg|gh|gi|gl|gm|gn|gp|gq|gr|gs|gt|gu|gw|gy|hk|hm|hn|hr|ht|" +
            "hu|id|ie|il|im|in|io|iq|ir|is|it|je|jm|jo|jp|ke|kg|kh|ki|km|kn|kp|kr|kw|ky|kz|la|lb|lc|li|lk|lr|ls|lt|" +
            "lu|lv|ly|ma|mc|md|me|mg|mh|mk|ml|mm|mn|mo|mp|mq|mr|ms|mt|mu|mv|mw|mx|my|mz|na|nc|ne|nf|ng|ni|nl|no|np|" +
            "nr|nu|nz|om|pa|pe|pf|pg|ph|pk|pl|pm|pn|pr|ps|pt|pw|py|qa|re|ro|rs|ru|rw|sa|sb|sc|sd|se|sg|sh|si|sj|sk|" +
            "sl|sm|sn|so|sr|ss|st|su|sv|sy|sz|tc|td|tf|tg|th|tj|tk|tl|tm|tn|to|tp|tr|tt|tv|tw|tz|ua|ug|uk|us|uy|uz|" +
            "va|vc|ve|vg|vi|vn|vu|wf|ws|ye|yt|za|zm|zw)(?=" + NOT_ALNUM + "|$))";
        private const string URL_PUNYCODE = "(?:xn--[0-9a-z]+)";

        private const string URL_VALID_DOMAIN =
          "(?:" +                                                   // subdomains + domain + TLD
              URL_VALID_SUBDOMAIN + "+" + URL_VALID_DOMAIN_NAME +   // e.g. www.twitter.com, foo.co.jp, bar.co.uk
              "(?:" + URL_VALID_GTLD + "|" + URL_VALID_CCTLD + "|" + URL_PUNYCODE + ")" +
            ")" +
          "|(?:" +                                                  // domain + gTLD
            URL_VALID_DOMAIN_NAME +                                 // e.g. twitter.com
            "(?:" + URL_VALID_GTLD + "|" + URL_PUNYCODE + ")" +
          ")" +
          "|(?:" + "(?<=https?://)" +
            "(?:" +
              "(?:" + URL_VALID_DOMAIN_NAME + URL_VALID_CCTLD + ")" +  // protocol + domain + ccTLD
              "|(?:" +
                URL_VALID_UNICODE_CHARS + "+\\." +                     // protocol + unicode domain + TLD
                "(?:" + URL_VALID_GTLD + "|" + URL_VALID_CCTLD + ")" +
              ")" +
            ")" +
          ")" +
          "|(?:" +                                                  // domain + ccTLD + '/'
            URL_VALID_DOMAIN_NAME + URL_VALID_CCTLD + "(?=/)" +     // e.g. t.co/
          ")";

        private const string URL_VALID_PORT_NUMBER = "[0-9]+"; // .NET does not support possessive quantifiers

        private const string URL_VALID_GENERAL_PATH_CHARS = "[a-z0-9!\\*';:=\\+,.\\$/%#\\[\\]\\-_~\\|&" + LATIN_ACCENTS_CHARS + "]";

        // Allow URL paths to contain balanced parens, used in Wikipedia URLs like /Primer_(film) and in IIS sessions like /S(dfd346)/
        private const string URL_BALANCED_PARENS = "\\(" + URL_VALID_GENERAL_PATH_CHARS + "+\\)";

        // Valid end-of-path chracters (so /foo. does not gobble the period), allow =&# for empty URL parameters and other URL-join artifacts
        private const string URL_VALID_PATH_ENDING_CHARS = "[a-z0-9=_#/\\-\\+" + LATIN_ACCENTS_CHARS + "]|(?:" + URL_BALANCED_PARENS + ")";

        private const string URL_VALID_PATH = "(?:" +
          "(?:" +
            URL_VALID_GENERAL_PATH_CHARS + "*" +
            "(?:" + URL_BALANCED_PARENS + URL_VALID_GENERAL_PATH_CHARS + "*)*" +
            URL_VALID_PATH_ENDING_CHARS +
          ")|(?:@" + URL_VALID_GENERAL_PATH_CHARS + "+/)" +
        ")";

        private const string URL_VALID_URL_QUERY_CHARS = "[a-z0-9!?\\*'\\(\\);:&=\\+\\$/%#\\[\\]\\-_\\.,~\\|]";
        private const string URL_VALID_URL_QUERY_ENDING_CHARS = "[a-z0-9_&=#/]";
        private const string VALID_URL_PATTERN_STRING =
        "(" +                                                            //  $1 total match
          "(" + URL_VALID_PRECEEDING_CHARS + ")" +                       //  $2 Preceeding chracter
          "(" +                                                          //  $3 URL
            "(https?://)?" +                                             //  $4 Protocol (optional)
            "(" + URL_VALID_DOMAIN + ")" +                               //  $5 Domain(s)
            "(?::(" + URL_VALID_PORT_NUMBER + "))?" +                    //  $6 Port number (optional)
            "(/" +
            //URL_VALID_PATH + "*+" +
              URL_VALID_PATH + "*" + // .NET does not support possessive quantifiers
            ")?" +                                                       //  $7 URL Path and anchor
            "(\\?" + URL_VALID_URL_QUERY_CHARS + "*" +                   //  $8 Query string
                    URL_VALID_URL_QUERY_ENDING_CHARS + ")?" +
          ")" +
        ")";

        private const string AT_SIGNS_CHARS = "@\uFF20";
        private const string DOLLAR_SIGN_CHAR = "\\$";
        private const string CASHTAG = "[a-z]{1,6}(?:[._][a-z]{1,2})?";

        public static readonly System.Text.RegularExpressions.Regex VALID_HASHTAG = new System.Text.RegularExpressions.Regex("(^|[^&" + HASHTAG_ALPHA_NUMERIC_CHARS + "])(#|\uFF03)(" + HASHTAG_ALPHA_NUMERIC + "*" + HASHTAG_ALPHA + HASHTAG_ALPHA_NUMERIC + "*)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        public const int VALID_HASHTAG_GROUP_BEFORE = 1;
        public const int VALID_HASHTAG_GROUP_HASH = 2;
        public const int VALID_HASHTAG_GROUP_TAG = 3;
        public static readonly System.Text.RegularExpressions.Regex INVALID_HASHTAG_MATCH_END = new System.Text.RegularExpressions.Regex("^(?:[#＃]|://)");

        public static readonly System.Text.RegularExpressions.Regex AT_SIGNS = new System.Text.RegularExpressions.Regex("[" + AT_SIGNS_CHARS + "]");
        public static readonly System.Text.RegularExpressions.Regex VALID_MENTION_OR_LIST = new System.Text.RegularExpressions.Regex("([^a-z0-9_!#$%&*" + AT_SIGNS_CHARS + "]|^|RT:?)(" + AT_SIGNS + "+)([a-z0-9_]{1,20})(/[a-z][a-z0-9_\\-]{0,24})?", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        public const int VALID_MENTION_OR_LIST_GROUP_BEFORE = 1;
        public const int VALID_MENTION_OR_LIST_GROUP_AT = 2;
        public const int VALID_MENTION_OR_LIST_GROUP_USERNAME = 3;
        public const int VALID_MENTION_OR_LIST_GROUP_LIST = 4;

        public static readonly System.Text.RegularExpressions.Regex VALID_REPLY = new System.Text.RegularExpressions.Regex("^(?:" + UNICODE_SPACES + ")*" + AT_SIGNS + "([a-z0-9_]{1,20})", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        public const int VALID_REPLY_GROUP_USERNAME = 1;

        public static readonly System.Text.RegularExpressions.Regex INVALID_MENTION_MATCH_END = new System.Text.RegularExpressions.Regex("^(?:[" + AT_SIGNS_CHARS + LATIN_ACCENTS_CHARS + "]|://)");

        public static readonly System.Text.RegularExpressions.Regex VALID_URL = new System.Text.RegularExpressions.Regex(VALID_URL_PATTERN_STRING, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        public const int VALID_URL_GROUP_ALL = 1;
        public const int VALID_URL_GROUP_BEFORE = 2;
        public const int VALID_URL_GROUP_URL = 3;
        public const int VALID_URL_GROUP_PROTOCOL = 4;
        public const int VALID_URL_GROUP_DOMAIN = 5;
        public const int VALID_URL_GROUP_PORT = 6;
        public const int VALID_URL_GROUP_PATH = 7;
        public const int VALID_URL_GROUP_QUERY_STRING = 8;

        public static readonly System.Text.RegularExpressions.Regex VALID_TCO_URL = new System.Text.RegularExpressions.Regex("^https?:\\/\\/t\\.co\\/[a-z0-9]+", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        public static readonly System.Text.RegularExpressions.Regex INVALID_URL_WITHOUT_PROTOCOL_MATCH_BEGIN = new System.Text.RegularExpressions.Regex("[-_./]$");

        public static readonly System.Text.RegularExpressions.Regex VALID_CASHTAG = new System.Text.RegularExpressions.Regex("(^|" + UNICODE_SPACES + ")(" + DOLLAR_SIGN_CHAR + ")(" + CASHTAG + ")" + "(?=$|\\s|" + PUNCT + ")", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        public const int VALID_CASHTAG_GROUP_BEFORE = 1;
        public const int VALID_CASHTAG_GROUP_DOLLAR = 2;
        public const int VALID_CASHTAG_GROUP_CASHTAG = 3;
    }
}

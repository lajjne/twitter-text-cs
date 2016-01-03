using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Twitter.Text.Tests {

    [TestClass]
    public class ExtractorTests {

        private Extractor _extractor = new Extractor();

        [TestMethod]
        public void UrlWithIndicesTest() {
            var extracted = _extractor.ExtractUrlsWithIndices("http://t.co url https://www.twitter.com ");
            Assert.AreEqual(extracted[0].Start, 0);
            Assert.AreEqual(extracted[0].End, 11);
            Assert.AreEqual(extracted[1].Start, 16);
            Assert.AreEqual(extracted[1].End, 39);
        }

        [TestMethod]
        public void UrlWithoutProtocolTest() {
            string text = "www.twitter.com, www.yahoo.co.jp, t.co/blahblah, www.poloshirts.uk.com";
            var expected = new String[] { "www.twitter.com", "www.yahoo.co.jp", "t.co/blahblah", "www.poloshirts.uk.com" };
            var actual = _extractor.ExtractUrls(text);
            CollectionAssert.AreEqual(expected, actual);

            var extracted = _extractor.ExtractUrlsWithIndices(text);
            Assert.AreEqual(extracted[0].Start, 0);
            Assert.AreEqual(extracted[0].End, 15);
            Assert.AreEqual(extracted[1].Start, 17);
            Assert.AreEqual(extracted[1].End, 32);
            Assert.AreEqual(extracted[2].Start, 34);
            Assert.AreEqual(extracted[2].End, 47);

            _extractor.ExtractUrlWithoutProtocol = false;
            Assert.IsTrue(_extractor.ExtractUrls(text).Count == 0, "Should not extract URLs w/o protocol");
            _extractor.ExtractUrlWithoutProtocol = true;
        }

        [TestMethod]
        public void URLFollowedByPunctuationsTest() {
            string text = "http://games.aarp.org/games/mahjongg-dimensions.aspx!!!!!!";
            var expected = new String[] { "http://games.aarp.org/games/mahjongg-dimensions.aspx" };
            var actual = _extractor.ExtractUrls(text);
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UrlWithPunctuationTest() {
            String[] urls = new String[] {
               "http://www.foo.com/foo/path-with-period./",
               "http://www.foo.org.za/foo/bar/688.1",
               "http://www.foo.com/bar-path/some.stm?param1=foo;param2=P1|0||P2|0",
               "http://foo.com/bar/123/foo_&_bar/",
               "http://foo.com/bar(test)bar(test)bar(test)",
               "www.foo.com/foo/path-with-period./",
               "www.foo.org.za/foo/bar/688.1",
               "www.foo.com/bar-path/some.stm?param1=foo;param2=P1|0||P2|0",
               "foo.com/bar/123/foo_&_bar/"
             };

            foreach (string url in urls) {
                List<string> extractedUrls = _extractor.ExtractUrls(url);
                Assert.AreEqual(url, extractedUrls[0]);
            }
        }

        [TestMethod]
        public void UrlWithSupplementaryCharactersTest() {
            // insert U+10400 before " http://twitter.com"
            string text = "\U00010400 http://twitter.com \U00010400 http://twitter.com";

            // count U+10400 as 2 characters (as in UTF-16)
            var extracted = _extractor.ExtractUrlsWithIndices(text);
            Assert.AreEqual(extracted.Count, 2);
            Assert.AreEqual(extracted[0].Value, "http://twitter.com");
            Assert.AreEqual(extracted[0].Start, 3);
            Assert.AreEqual(extracted[0].End, 21);
            Assert.AreEqual(extracted[1].Value, "http://twitter.com");
            Assert.AreEqual(extracted[1].Start, 25);
            Assert.AreEqual(extracted[1].End, 43);
        }

        [TestMethod]
        public void ReplyAtTheBeginningTest() {
            string extracted = _extractor.ExtractReplyScreenname("@user reply");
            Assert.AreEqual("user", extracted, "Failed to extract reply at the start");
        }

        [TestMethod]
        public void ReplyWithLeadingSpaceTest() {
            string extracted = _extractor.ExtractReplyScreenname(" @user reply");
            Assert.AreEqual("user", extracted, "Failed to extract reply with leading space");
        }


        [TestMethod]
        public void MentionAtTheBeginningTest() {
            List<string> extracted = _extractor.ExtractMentionedScreennames("@user mention");
            var expected = new String[] { "user" };
            CollectionAssert.AreEqual(expected, extracted);
        }

        [TestMethod]
        public void MentionWithLeadingSpaceTest() {
            List<string> extracted = _extractor.ExtractMentionedScreennames(" @user mention");
            var expected = new String[] { "user" };
            CollectionAssert.AreEqual(expected, extracted);
        }

        [TestMethod]
        public void MentionInMidTextTest() {
            List<string> extracted = _extractor.ExtractMentionedScreennames("mention @user here");
            var expected = new String[] { "user" };
            CollectionAssert.AreEqual(expected, extracted);
        }

        [TestMethod]
        public void MultipleMentionsTest() {
            List<string> extracted = _extractor.ExtractMentionedScreennames("mention @user1 here and @user2 here");

            var expected = new String[] { "user1", "user2" };
            CollectionAssert.AreEqual(expected, extracted);

        }

        [TestMethod]
        public void MentionWithIndicesTest() {
            var extracted = _extractor.ExtractMentionedScreennamesWithIndices(" @user1 mention @user2 here @user3 ");
            Assert.AreEqual(extracted.Count(), 3);
            Assert.AreEqual(extracted[0].Start, 1);
            Assert.AreEqual(extracted[0].End, 7);
            Assert.AreEqual(extracted[1].Start, 16);
            Assert.AreEqual(extracted[1].End, 22);
            Assert.AreEqual(extracted[2].Start, 28);
            Assert.AreEqual(extracted[2].End, 34);
        }

        [TestMethod]
        public void MentionWithSupplementaryCharactersTest() {
            // insert U+10400 before " @mention"
            string text = "\U00010400 @mention \U00010400 @mention";

            // count U+10400 as 2 characters (as in UTF-16)
            var extracted = _extractor.ExtractMentionedScreennamesWithIndices(text);
            Assert.AreEqual(extracted.Count(), 2);
            Assert.AreEqual(extracted[0].Value, "mention");
            Assert.AreEqual(extracted[0].Start, 3);
            Assert.AreEqual(extracted[0].End, 11);
            Assert.AreEqual(extracted[1].Value, "mention");
            Assert.AreEqual(extracted[1].Start, 15);
            Assert.AreEqual(extracted[1].End, 23);
        }

        [TestMethod]
        public void HashtagAtTheBeginningTest() {
            List<string> extracted = _extractor.ExtractHashtags("#hashtag mention");
            var expected = new String[] { "hashtag" };
            CollectionAssert.AreEqual(expected, extracted);
        }

        [TestMethod]
        public void HashtagWithLeadingSpaceTest() {
            List<string> extracted = _extractor.ExtractHashtags(" #hashtag mention");
            var expected = new String[] { "hashtag" };
            CollectionAssert.AreEqual(expected, extracted);
        }

        [TestMethod]
        public void HashtagInMidTextTest() {
            List<string> extracted = _extractor.ExtractHashtags("mention #hashtag here");
            var expected = new String[] { "hashtag" };
            CollectionAssert.AreEqual(expected, extracted);
        }

        [TestMethod]
        public void MultipleHashtagsTest() {
            List<string> extracted = _extractor.ExtractHashtags("text #hashtag1 #hashtag2");
            var expected = new String[] { "hashtag1", "hashtag2" };
            CollectionAssert.AreEqual(expected, extracted);
        }

        [TestMethod]
        public void HashtagWithIndicesTest() {
            var extracted = _extractor.ExtractHashtagsWithIndices(" #user1 mention #user2 here #user3 ");
            Assert.AreEqual(extracted.Count, 3);
            Assert.AreEqual(extracted[0].Start, 1);
            Assert.AreEqual(extracted[0].End, 7);
            Assert.AreEqual(extracted[1].Start, 16);
            Assert.AreEqual(extracted[1].End, 22);
            Assert.AreEqual(extracted[2].Start, 28);
            Assert.AreEqual(extracted[2].End, 34);
        }

        [TestMethod]
        public void HashtagWithSupplementaryCharactersTest() {
            // insert U+10400 before " #hashtag"
            string text = "\U00010400 #hashtag \U00010400 #hashtag";

            // count U+10400 as 2 characters (as in UTF-16)
            var extracted = _extractor.ExtractHashtagsWithIndices(text);
            Assert.AreEqual(extracted.Count, 2);
            Assert.AreEqual(extracted[0].Value, "hashtag");
            Assert.AreEqual(extracted[0].Start, 3);
            Assert.AreEqual(extracted[0].End, 11);
            Assert.AreEqual(extracted[1].Value, "hashtag");
            Assert.AreEqual(extracted[1].Start, 15);
            Assert.AreEqual(extracted[1].End, 23);
        }

    }
}
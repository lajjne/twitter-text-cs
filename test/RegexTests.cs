using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Pattern = System.Text.RegularExpressions.Regex;

namespace Twitter.Text.Tests {

    [TestClass]
    public class RegexTests {

        [TestMethod]
        public void AutoLinkHashtagsTest() {
            AssertCaptureCount(4, Regex.VALID_HASHTAG, "#hashtag");
            AssertCaptureCount(4, Regex.VALID_HASHTAG, "#Azərbaycanca");
            AssertCaptureCount(4, Regex.VALID_HASHTAG, "#mûǁae");
            AssertCaptureCount(4, Regex.VALID_HASHTAG, "#Čeština");
            AssertCaptureCount(4, Regex.VALID_HASHTAG, "#Ċaoiṁín");
            AssertCaptureCount(4, Regex.VALID_HASHTAG, "#Caoiṁín");
            AssertCaptureCount(4, Regex.VALID_HASHTAG, "#ta\u0301im");
            AssertCaptureCount(4, Regex.VALID_HASHTAG, "#hag\u0303ua");
            AssertCaptureCount(4, Regex.VALID_HASHTAG, "#caf\u00E9");
            AssertCaptureCount(4, Regex.VALID_HASHTAG, "#\u05e2\u05d1\u05e8\u05d9\u05ea"); // "#Hebrew"
            AssertCaptureCount(4, Regex.VALID_HASHTAG, "#\u05d0\u05b2\u05e9\u05b6\u05c1\u05e8"); // with marks
            AssertCaptureCount(4, Regex.VALID_HASHTAG, "#\u05e2\u05b7\u05dc\u05be\u05d9\u05b0\u05d3\u05b5\u05d9"); // with maqaf 05be
            AssertCaptureCount(4, Regex.VALID_HASHTAG, "#\u05d5\u05db\u05d5\u05f3"); // with geresh 05f3
            AssertCaptureCount(4, Regex.VALID_HASHTAG, "#\u05de\u05f4\u05db"); // with gershayim 05f4
            AssertCaptureCount(4, Regex.VALID_HASHTAG, "#\u0627\u0644\u0639\u0631\u0628\u064a\u0629"); // "#Arabic"
            AssertCaptureCount(4, Regex.VALID_HASHTAG, "#\u062d\u0627\u0644\u064a\u0627\u064b"); // with mark
            AssertCaptureCount(4, Regex.VALID_HASHTAG, "#\u064a\u0640\ufbb1\u0640\u064e\u0671"); // with pres. form
            AssertCaptureCount(4, Regex.VALID_HASHTAG, "#ประเทศไทย");
            AssertCaptureCount(4, Regex.VALID_HASHTAG, "#ฟรี"); // with mark
            AssertCaptureCount(4, Regex.VALID_HASHTAG, "#日本語ハッシュタグ");
            AssertCaptureCount(4, Regex.VALID_HASHTAG, "＃日本語ハッシュタグ");

            Assert.IsTrue(Regex.VALID_HASHTAG.Match("これはOK #ハッシュタグ").Success);
            Assert.IsTrue(Regex.VALID_HASHTAG.Match("これもOK。#ハッシュタグ").Success);
            Assert.IsFalse(Regex.VALID_HASHTAG.Match("これはダメ#ハッシュタグ").Success);

            Assert.IsFalse(Regex.VALID_HASHTAG.Match("#1").Success);
            Assert.IsFalse(Regex.VALID_HASHTAG.Match("#0").Success);
        }

        [TestMethod]
        public void AutoLinkUsernamesOrListsTest() {
            AssertCaptureCount(5, Regex.VALID_MENTION_OR_LIST, "@username");
            AssertCaptureCount(5, Regex.VALID_MENTION_OR_LIST, "@username/list");
        }

        [TestMethod]
        public void ValidURLTest() {
            AssertCaptureCount(9, Regex.VALID_URL, "http://example.com");
            AssertCaptureCount(9, Regex.VALID_URL, "http://はじめよう.みんな");
            AssertCaptureCount(9, Regex.VALID_URL, "http://はじめよう.香港");
            AssertCaptureCount(9, Regex.VALID_URL, "http://はじめよう.الجزائر");
        }

        [TestMethod]
        public void ValidURLDoesNotCrashOnLongPathsTest() {
            String longPathIsLong = "Check out http://example.com/aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            Assert.IsTrue(Regex.VALID_URL.Match(longPathIsLong).Success, "Failed to correctly match a very long path");
        }

        [TestMethod]
        public void ValidUrlDoesNotTakeForeverOnRepeatedPuctuationAtEndTest() {
            String[] repeatedPaths = {
                "Try http://example.com/path**********************",
                "http://foo.org/bar/foo-bar-foo-bar.aspx!!!!!! Test"
            };

            Stopwatch stopWatch = new Stopwatch();
            foreach (String text in repeatedPaths) {
                stopWatch.Restart();
                bool isValid = Regex.VALID_URL.Match(text).Success;
                stopWatch.Stop();

                Assert.IsTrue(isValid, "Should be able to extract a valid URL even followed by punctuations");
                Assert.IsTrue((stopWatch.ElapsedMilliseconds < 10), "Matching a repeated path end should take less than 10ms (took " + stopWatch.ElapsedMilliseconds + "ms)");
            }
        }

        [TestMethod]
        public void ValidURLWithoutProtocolTest() {
            Assert.IsTrue(Regex.VALID_URL.Match("twitter.com").Success, "Matching a URL with gTLD without protocol.");
            Assert.IsTrue(Regex.VALID_URL.Match("www.foo.co.jp").Success, "Matching a URL with ccTLD without protocol.");
            Assert.IsTrue(Regex.VALID_URL.Match("www.foo.org.za").Success, "Matching a URL with gTLD followed by ccTLD without protocol.");
            Assert.IsTrue(Regex.VALID_URL.Match("http://t.co").Success, "Should not match a short URL with ccTLD without protocol.");
            Assert.IsFalse(Regex.VALID_URL.Match("it.so").Success, "Should not match a short URL with ccTLD without protocol.");
            Assert.IsFalse(Regex.VALID_URL.Match("www.xxxxxxx.baz").Success, "Should not match a URL with invalid gTLD.");
            Assert.IsTrue(Regex.VALID_URL.Match("t.co/blahblah").Success, "Match a short URL with ccTLD and '/' but without protocol.");
        }

        [TestMethod]
        public void InvalidUrlWithInvalidCharacterTest() {
            char[] invalid_chars = new char[] { '\u202A', '\u202B', '\u202C', '\u202D', '\u202E' };
            foreach (char c in invalid_chars) {
                Assert.IsFalse(Regex.VALID_URL.Match("http://twitt" + c + "er.com").Success, "Should not extract URLs with invalid character");
            }
        }

        [TestMethod]
        public void ExtractMentionsTest() {
            AssertCaptureCount(5, Regex.VALID_MENTION_OR_LIST, "sample @user mention");
        }

        [TestMethod]
        public void InvalidMentionsTest() {
            char[] invalid_chars = new char[] { '!', '@', '#', '$', '%', '&', '*' };
            foreach (char c in invalid_chars) {
                Assert.IsFalse(Regex.VALID_MENTION_OR_LIST.Match("f" + c + "@kn").Success, "Failed to ignore a mention preceded by " + c);
            }
        }

        [TestMethod]
        public void ExtractReplyTest() {
            AssertCaptureCount(2, Regex.VALID_REPLY, "@user reply");
            AssertCaptureCount(2, Regex.VALID_REPLY, " @user reply");
            AssertCaptureCount(2, Regex.VALID_REPLY, "\u3000@user reply");
        }

        private void AssertCaptureCount(int expectedCount, Pattern pattern, String sample) {
            Assert.IsTrue(pattern.Match(sample).Success, "Pattern failed to match sample: '" + sample + "'");
            Assert.AreEqual(expectedCount, pattern.Match(sample).Groups.Count, "Does not have " + expectedCount + " captures as expected: '" + sample + "'");
        }
    }
}
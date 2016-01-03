using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Twitter.Text.Tests {

    [TestClass]
    public class ValidatorTests {
        private Validator _validator = new Validator();

        [TestMethod]
        public void BOMCharacterTest() {
            Assert.IsFalse(_validator.IsValidTweet("test \uFFFE"));
            Assert.IsFalse(_validator.IsValidTweet("test \uFEFF"));
        }

        [TestMethod]
        public void InvalidCharacterTest() {
            Assert.IsFalse(_validator.IsValidTweet("test \uFFFF"));
            Assert.IsFalse(_validator.IsValidTweet("test \uFEFF"));
        }

        [TestMethod]
        public void DirectionChangeCharactersTest() {
            Assert.IsFalse(_validator.IsValidTweet("test \u202A test"));
            Assert.IsFalse(_validator.IsValidTweet("test \u202B test"));
            Assert.IsFalse(_validator.IsValidTweet("test \u202C test"));
            Assert.IsFalse(_validator.IsValidTweet("test \u202D test"));
            Assert.IsFalse(_validator.IsValidTweet("test \u202E test"));
        }

        [TestMethod]
        public void AccentCharactersTest() {
            string c = "\u0065\u0301";
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < 139; i++) {
                builder.Append(c);
            }
            Assert.IsTrue(_validator.IsValidTweet(builder.ToString()));
            Assert.IsTrue(_validator.IsValidTweet(builder.Append(c).ToString()));
            Assert.IsFalse(_validator.IsValidTweet(builder.Append(c).ToString()));
        }

        [TestMethod]
        public void MutiByteCharactersTest() {
            string c = "\ud83d\ude02";
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < 139; i++) {
                builder.Append(c);
            }
            Assert.IsTrue(_validator.IsValidTweet(builder.ToString()));
            Assert.IsTrue(_validator.IsValidTweet(builder.Append(c).ToString()));
            Assert.IsFalse(_validator.IsValidTweet(builder.Append(c).ToString()));
        }
    }
}
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using YamlDotNet.RepresentationModel;

namespace Twitter.Text.Tests {

    [TestClass]
    [DeploymentItem(@"..\..\..\twitter-text\conformance\")]
    public class ConformanceTests {
        public TestContext TestContext { get; set; }

        static Autolink autolink = new Autolink { NoFollow = false };
        static Extractor extractor = new Extractor();
        static HitHighlighter highlighter = new HitHighlighter();
        static Validator validator = new Validator();

        [TestMethod]
        public void ExtractMentions() {
            var failures = new List<string>();
            foreach (var test in LoadTests<dynamic>("extract.yml", "mentions")) {
                try {
                    var actual = extractor.ExtractMentionedScreennames(test.Text);
                    CollectionAssert.AreEquivalent(test.Expected, actual);
                } catch (Exception) {
                    failures.Add(test.Description + ": " + test.Text);
                }
            }
            if (failures.Any()) {
                Assert.Fail(string.Join("\n", failures));
            }
        }

        [TestMethod]
        public void ExtractMentionsWithIndices() {
            var failures = new List<string>();
            foreach (var test in LoadTests<dynamic>("extract.yml", "mentions_with_indices")) {
                try {
                    var actual = extractor.ExtractMentionedScreennamesWithIndices(test.Text);
                    for (int i = 0; i < actual.Count; i++) {
                        var entity = actual[i];
                        Assert.AreEqual(test.Expected[i].screen_name, entity.Value);
                        Assert.AreEqual(test.Expected[i].indices[0], entity.Start);
                        Assert.AreEqual(test.Expected[i].indices[1], entity.End);
                    }
                } catch (Exception) {
                    failures.Add(test.Description + ": " + test.Text);
                }
            }
            if (failures.Any()) {
                Assert.Fail(string.Join("\n", failures));
            }
        }

        [TestMethod]
        public void ExtractMentionsOrListsWithIndices() {
            var failures = new List<string>();
            foreach (var test in LoadTests<dynamic>("extract.yml", "mentions_or_lists_with_indices")) {
                try {
                    List<TweetEntity> actual = extractor.ExtractMentionsOrListsWithIndices(test.Text);
                    for (int i = 0; i < actual.Count; i++) {
                        var entity = actual[i];
                        Assert.AreEqual(test.Expected[i].screen_name, entity.Value);
                        Assert.AreEqual(test.Expected[i].list_slug, entity.ListSlug);
                        Assert.AreEqual(test.Expected[i].indices[0], entity.Start);
                        Assert.AreEqual(test.Expected[i].indices[1], entity.End);
                    }
                } catch (Exception) {
                    failures.Add(test.Description + ": " + test.Text);
                }
            }
            if (failures.Any()) {
                Assert.Fail(string.Join("\n", failures));
            }
        }

        [TestMethod]
        public void ExtractReplies() {
            var failures = new List<string>();
            foreach (var test in LoadTests<dynamic>("extract.yml", "replies")) {
                try {
                    string actual = extractor.ExtractReplyScreenname(test.Text);
                    Assert.AreEqual(test.Expected, actual);
                } catch (Exception) {
                    failures.Add(test.Description + ": " + test.Text);
                }
            }
            if (failures.Any()) {
                Assert.Fail(string.Join("\n", failures));
            }
        }

        [TestMethod]
        public void ExtractUrls() {
            var failures = new List<string>();
            foreach (var test in LoadTests<dynamic>("extract.yml", "urls")) {
                try {
                    List<string> actual = extractor.ExtractUrls(test.Text);
                    CollectionAssert.AreEquivalent(test.Expected, actual);
                } catch (Exception) {
                    failures.Add(test.Description + ": " + test.Text);
                }
            }
            if (failures.Any()) {
                Assert.Fail(string.Join("\n", failures));
            }
        }

        [TestMethod]
        public void ExtractUrlsWithIndices() {
            var failures = new List<string>();
            foreach (var test in LoadTests<dynamic>("extract.yml", "urls_with_indices")) {
                try {
                    var actual = extractor.ExtractUrlsWithIndices(test.Text);
                    for (int i = 0; i < actual.Count; i++) {
                        var entity = actual[i];
                        Assert.AreEqual(test.Expected[i].url, entity.Value);
                        Assert.AreEqual(test.Expected[i].indices[0], entity.Start);
                        Assert.AreEqual(test.Expected[i].indices[1], entity.End);
                    }
                } catch (Exception) {
                    failures.Add(test.Description + ": " + test.Text);
                }
            }
            if (failures.Any()) {
                Assert.Fail(string.Join("\n", failures));
            }
        }

        [TestMethod]
        public void ExtractHashtags() {
            var failures = new List<string>();
            foreach (var test in LoadTests<dynamic>("extract.yml", "hashtags")) {
                try {
                    List<string> actual = extractor.ExtractHashtags(test.Text);
                    CollectionAssert.AreEquivalent(test.Expected, actual);
                } catch (Exception) {
                    failures.Add(test.Description + ": " + test.Text);
                }
            }
            if (failures.Any()) {
                Assert.Fail(string.Join("\n", failures));
            }
        }

        [TestMethod]
        public void ExtractHashtagsWithIndices() {
            var failures = new List<string>();
            foreach (var test in LoadTests<dynamic>("extract.yml", "hashtags_with_indices")) {
                try {
                    var actual = extractor.ExtractHashtagsWithIndices(test.Text);
                    for (int i = 0; i < actual.Count; i++) {
                        var entity = actual[i];
                        Assert.AreEqual(test.Expected[i].hashtag, entity.Value);
                        Assert.AreEqual(test.Expected[i].indices[0], entity.Start);
                        Assert.AreEqual(test.Expected[i].indices[1], entity.End);
                    }
                } catch (Exception) {
                    failures.Add(test.Description + ": " + test.Text);
                }
            }
            if (failures.Any()) {
                Assert.Fail(string.Join("\n", failures));
            }
        }

        [TestMethod]
        public void ExtractCashtags() {
            var failures = new List<string>();
            foreach (var test in LoadTests<dynamic>("extract.yml", "cashtags")) {
                try {
                    List<string> actual = extractor.ExtractCashtags(test.Text);
                    CollectionAssert.AreEquivalent(test.Expected, actual);
                } catch (Exception) {
                    failures.Add(test.Description + ": " + test.Text);
                }
            }
            if (failures.Any()) {
                Assert.Fail(string.Join("\n", failures));
            }
        }

        [TestMethod]
        public void ExtractCashtagsWithIndices() {
            var failures = new List<string>();
            foreach (var test in LoadTests<dynamic>("extract.yml", "cashtags_with_indices")) {
                try {
                    var actual = extractor.ExtractCashtagsWithIndices(test.Text);
                    for (int i = 0; i < actual.Count; i++) {
                        var entity = actual[i];
                        Assert.AreEqual(test.Expected[i].cashtag, entity.Value);
                        Assert.AreEqual(test.Expected[i].indices[0], entity.Start);
                        Assert.AreEqual(test.Expected[i].indices[1], entity.End);
                    }
                } catch (Exception) {
                    failures.Add(test.Description + ": " + test.Text);
                }
            }
            if (failures.Any()) {
                Assert.Fail(string.Join("\n", failures));
            }
        }

        [TestMethod]
        public void AutolinkUsernames() {
            var failures = new List<string>();
            foreach (var test in LoadTests<string>("autolink.yml", "usernames")) {
                try {
                    string actual = autolink.AutoLinkUsernamesAndLists(test.Text);
                    Assert.AreEqual(test.Expected, actual);
                } catch (Exception) {
                    failures.Add(test.Description + ": " + test.Text);
                }
            }
            if (failures.Any()) {
                Assert.Fail(string.Join("\n", failures));
            }
        }

        [TestMethod]
        public void AutolinkLists() {
            var failures = new List<string>();
            foreach (var test in LoadTests<string>("autolink.yml", "lists")) {
                try {
                    string actual = autolink.AutoLinkUsernamesAndLists(test.Text);
                    Assert.AreEqual(test.Expected, actual);
                } catch (Exception) {
                    failures.Add(test.Description + ": " + test.Text);
                }
            }
            if (failures.Any()) {
                Assert.Fail(string.Join("\n", failures));
            }
        }

        [TestMethod]
        public void AutolinkHashtags() {
            var failures = new List<string>();
            foreach (var test in LoadTests<string>("autolink.yml", "hashtags")) {
                try {
                    string actual = autolink.AutoLinkHashtags(test.Text);
                    Assert.AreEqual(test.Expected, actual);
                } catch (Exception) {
                    failures.Add(test.Description + ": " + test.Text);
                }
            }
            if (failures.Any()) {
                Assert.Fail(string.Join("\n", failures));
            }
        }

        [TestMethod]
        public void AutolinkUrls() {
            var failures = new List<string>();
            foreach (var test in LoadTests<string>("autolink.yml", "urls")) {
                try {
                    string actual = autolink.AutoLinkUrls(test.Text);
                    Assert.AreEqual(test.Expected, actual);
                } catch (Exception) {
                    failures.Add(test.Description + ": " + test.Text);
                }
            }
            if (failures.Any()) {
                Assert.Fail(string.Join("\n", failures));
            }
        }

        [TestMethod]
        public void AutolinkCashtags() {
            var failures = new List<string>();
            foreach (var test in LoadTests<string>("autolink.yml", "cashtags")) {
                try {
                    string actual = autolink.AutoLinkCashtags(test.Text);
                    Assert.AreEqual(test.Expected, actual);
                } catch (Exception) {
                    failures.Add(test.Description + ": " + test.Text);
                }
            }
            if (failures.Any()) {
                Assert.Fail(string.Join("\n", failures));
            }
        }

        [TestMethod]
        public void AutolinkAll() {
            var failures = new List<string>();
            foreach (var test in LoadTests<string>("autolink.yml", "all")) {
                try {
                    string actual = autolink.AutoLink(test.Text);
                    Assert.AreEqual(test.Expected, actual);
                } catch (Exception) {
                    failures.Add(test.Description + ": " + test.Text);
                }
            }
            if (failures.Any()) {
                Assert.Fail(string.Join("\n", failures));
            }
        }

        [TestMethod]
        public void AutolinkJson() {
            var failures = new List<string>();
            foreach (var test in LoadTests<string>("autolink.yml", "json")) {
                try {
                    string actual = autolink.AutoLink(test.Text);
                    Assert.AreEqual(test.Expected, actual);
                } catch (Exception) {
                    failures.Add(test.Description + ": " + test.Text);
                }
            }
            if (failures.Any()) {
                Assert.Fail(string.Join("\n", failures));
            }
        }

        [TestMethod]
        public void HighlightPlainText() {
            var failures = new List<string>();
            foreach (var test in LoadTests<string>("hit_highlighting.yml", "plain_text")) {
                try {
                    string actual = highlighter.Highlight(test.Text, test.Hits);
                    Assert.AreEqual(test.Expected, actual);
                } catch (Exception) {
                    failures.Add(test.Description + ": " + test.Text);
                }
            }
            if (failures.Any()) {
                Assert.Fail(string.Join("\n", failures));
            }
        }

        [TestMethod]
        public void HighlightWithLinks() {
            var failures = new List<string>();
            foreach (var test in LoadTests<string>("hit_highlighting.yml", "with_links")) {
                try {
                    string actual = highlighter.Highlight(test.Text, test.Hits);
                    Assert.AreEqual(test.Expected, actual);
                } catch (Exception) {
                    failures.Add(test.Description + ": " + test.Text);
                }
            }
            if (failures.Any()) {
                Assert.Fail(string.Join("\n", failures));
            }
        }

        [TestMethod]
        public void ValidateTweets() {
            var failures = new List<string>();
            foreach (var test in LoadTests<bool>("validate.yml", "tweets")) {
                try {
                    bool actual = validator.IsValidTweet(test.Text);
                    Assert.AreEqual(test.Expected, actual);
                } catch (Exception) {
                    failures.Add(test.Description + ": " + test.Text);
                }
            }
            if (failures.Any()) {
                Assert.Fail(string.Join("\n", failures));
            }
        }

        [TestMethod]
        public void ValidateUsernames() {
            var failures = new List<string>();
            foreach (var test in LoadTests<bool>("validate.yml", "usernames")) {
                try {
                    bool actual = validator.IsValidUsername(test.Text);
                    Assert.AreEqual(test.Expected, actual);
                } catch (Exception) {
                    failures.Add(test.Description + ": " + test.Text);
                }
            }
            if (failures.Any()) {
                Assert.Fail(string.Join("\n", failures));
            }
        }

        [TestMethod]
        public void ValidateLists() {
            var failures = new List<string>();
            foreach (var test in LoadTests<bool>("validate.yml", "lists")) {
                try {
                    bool actual = validator.IsValidList(test.Text);
                    Assert.AreEqual(test.Expected, actual);
                } catch (Exception) {
                    failures.Add(test.Description + ": " + test.Text);
                }
            }
            if (failures.Any()) {
                Assert.Fail(string.Join("\n", failures));
            }
        }

        [TestMethod]
        public void ValidateHashTags() {
            var failures = new List<string>();
            foreach (var test in LoadTests<bool>("validate.yml", "hashtags")) {
                try {
                    bool actual = validator.IsValidHashTag(test.Text);
                    Assert.AreEqual(test.Expected, actual);
                } catch (Exception) {
                    failures.Add(test.Description + ": " + test.Text);
                }
            }
            if (failures.Any()) {
                Assert.Fail(string.Join("\n", failures));
            }
        }

        [TestMethod]
        public void ValidateUrls() {
            var failures = new List<string>();
            foreach (var test in LoadTests<bool>("validate.yml", "urls")) {
                try {
                    bool actual = validator.IsValidUrl(test.Text);
                    Assert.AreEqual(test.Expected, actual);
                } catch (Exception) {
                    failures.Add(test.Description + ": " + test.Text);
                }
            }
            if (failures.Any()) {
                Assert.Fail(string.Join("\n", failures));
            }
        }

        [TestMethod]
        public void ValidateUrlsWithoutProtocol() {
            var failures = new List<string>();
            foreach (var test in LoadTests<bool>("validate.yml", "urls_without_protocol")) {
                try {
                    bool actual = validator.IsValidUrl(test.Text);
                    Assert.AreEqual(test.Expected, actual);
                } catch (Exception) {
                    failures.Add(test.Description + ": " + test.Text);
                }
            }
            if (failures.Any()) {
                Assert.Fail(string.Join("\n", failures));
            }
        }

        [TestMethod]
        public void ValidateLengths() {
            var failures = new List<string>();
            int actual = default(int);
            foreach (var test in LoadTests<int>("validate.yml", "lengths")) {
                try {
                    actual = validator.GetTweetLength(test.Text);
                    Assert.AreEqual(test.Expected, actual);
                } catch (Exception) {
                    failures.Add(test.Description + " – " + test.Text + " : expected " + test.Expected + ", actual " + actual + "");
                }
            }
            if (failures.Any()) {
                Assert.Fail(string.Join("\n", failures));
            }
        }

        [TestMethod]
        public void CountryTlds() {
            var failures = new List<string>();
            foreach (var test in LoadTests<List<string>>("tlds.yml", "country")) {
                try {
                    List<string> actual = extractor.ExtractUrls(test.Text);
                    CollectionAssert.AreEqual(test.Expected, actual);
                } catch (Exception) {
                    failures.Add(test.Description + ": " + test.Text);
                }
            }
            if (failures.Any()) {
                Assert.Fail(string.Join("\n", failures));
            }
        }

        [TestMethod]
        public void GenericTlds() {
            var failures = new List<string>();
            foreach (var test in LoadTests<List<string>>("tlds.yml", "generic")) {
                try {
                    List<string> actual = extractor.ExtractUrls(test.Text);
                    CollectionAssert.AreEqual(test.Expected, actual);
                } catch (Exception) {
                    failures.Add(test.Description + ": " + test.Text);
                }
            }
            if (failures.Any()) {
                Assert.Fail(string.Join("\n", failures));
            }
        }

        private IEnumerable<Test<TExpected>> LoadTests<TExpected>(string file, string section) {

            // load yaml file
            var stream = new StreamReader(Path.Combine(TestContext.TestDeploymentDir, file));
            var yaml = new YamlStream();
            yaml.Load(stream);

            // load specified test section
            var root = yaml.Documents[0].RootNode as YamlMappingNode;
            var tests = root.Children[new YamlScalarNode("tests")] as YamlMappingNode;
            var sect = tests.Children.Single(x => x.Key.ToString() == section);

            var items = sect.Value as YamlSequenceNode;
            foreach (YamlMappingNode item in items) {
                // parse test
                var test = new Test<TExpected>();
                test.Description = ConvertNode<string>(item.Children.Single(x => x.Key.ToString() == "description").Value);
                test.Text = ConvertNode<string>(item.Children.Single(x => x.Key.ToString() == "text").Value);
                test.Expected = ConvertNode<TExpected>(item.Children.Single(x => x.Key.ToString() == "expected").Value);
                test.Hits = ConvertNode<List<List<int>>>(item.Children.SingleOrDefault(x => x.Key.ToString() == "hits").Value);
                // return test
                yield return test;
            }
        }

        private dynamic ConvertNode<T>(YamlNode node) {
            dynamic dynnode = node as dynamic;

            if (node is YamlScalarNode) {
                if (string.IsNullOrEmpty(dynnode.Value)) {
                    return null;
                } else if (typeof(T) == typeof(int)) {
                    return int.Parse(dynnode.Value);
                } else if (typeof(T) == typeof(bool)) {
                    return dynnode.Value == "true";
                } else {
                    return dynnode.Value;
                }
            } else if (node is YamlSequenceNode) {
                dynamic list;
                if (typeof(T) == typeof(List<List<int>>)) {
                    list = new List<List<int>>();
                    foreach (var item in dynnode.Children) {
                        list.Add(ConvertNode<List<int>>(item));
                    }
                } else if (typeof(T) == typeof(List<int>)) {
                    list = new List<int>();
                    foreach (var item in dynnode.Children) {
                        list.Add(ConvertNode<int>(item));
                    }
                } else if (typeof(T) == typeof(List<string>)) {
                    list = new List<string>();
                    foreach (var item in dynnode.Children) {
                        list.Add(ConvertNode<string>(item));
                    }
                } else {
                    list = new List<dynamic>();
                    foreach (var item in dynnode.Children) {
                        list.Add(ConvertNode<T>(item));
                    }
                }
                return list;
            } else if (node is YamlMappingNode) {
                dynamic mapnode = new ExpandoObject();
                foreach (var item in ((YamlMappingNode)node).Children) {
                    var key = item.Key.ToString();
                    if (key == "indices") {
                        ((IDictionary<string, object>)mapnode).Add(key, ConvertNode<int>(item.Value));
                    } else {
                        ((IDictionary<string, object>)mapnode).Add(key, ConvertNode<T>(item.Value));
                    }
                }
                return mapnode;
            }
            return null;
        }

        private class Test<TExpected> {
            public string Description { get; set; }
            public string Text { get; set; }
            public TExpected Expected { get; set; }
            public List<List<int>> Hits { get; set; }
        }
    }
}

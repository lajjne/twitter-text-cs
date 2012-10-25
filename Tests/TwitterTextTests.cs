using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YamlDotNet.RepresentationModel;
using TwitterText;
using System.Dynamic;

namespace Tests {

    [TestClass]
    public class TwitterTextTests {
        private Dictionary<string, Dictionary<string, List<dynamic>>> testdict = new Dictionary<string, Dictionary<string, List<dynamic>>>();
        private static Extractor extractor = new Extractor();
        private static Autolink autolink = new Autolink();
        private static HitHighlighter highlighter = new HitHighlighter();
        private static Validator validator = new Validator();
        public TestContext TestContext { get; set; }

        [ClassInitialize()]
        public static void ClassInit(TestContext context) {
            autolink.setNoFollow(false);
        }

        [TestMethod]
        public void ExtractMentionsTest() {
            var failures = new List<string>();
            foreach (var test in LoadTests<dynamic>("extract.yml", "mentions")) {
                try {
                    List<string> actual = extractor.extractMentionedScreennames(test.text);
                    CollectionAssert.AreEquivalent(test.expected, actual);
                } catch (Exception) {
                    failures.Add(test.description + ": " + test.text);
                }
            }
            if (failures.Any()) {
                Assert.Fail(string.Join("\n", failures));
            }
        }

        [TestMethod]
        public void ExtractMentionsWithIndicesTest() {
            var failures = new List<string>();
            foreach (var test in LoadTests<dynamic>("extract.yml", "mentions_with_indices")) {
                try {
                    List<Entity> actual = extractor.extractMentionedScreennamesWithIndices(test.text);
                    for (int i = 0; i < actual.Count; i++) {
                        var entity = actual[i];
                        Assert.AreEqual(test.expected[i].screen_name, entity.getValue());
                        Assert.AreEqual(test.expected[i].indices[0], entity.getStart());
                        Assert.AreEqual(test.expected[i].indices[1], entity.getEnd());
                    }
                } catch (Exception) {
                    failures.Add(test.description + ": " + test.text);
                }
            }
            if (failures.Any()) {
                Assert.Fail(string.Join("\n", failures));
            }
        }

        [TestMethod]
        public void ExtractMentionsOrListsWithIndicesTest() {
            var failures = new List<string>();
            foreach (var test in LoadTests<dynamic>("extract.yml", "mentions_or_lists_with_indices")) {
                try {
                    List<Entity> actual = extractor.extractMentionsOrListsWithIndices(test.text);
                    for (int i = 0; i < actual.Count; i++) {
                        var entity = actual[i];
                        Assert.AreEqual(test.expected[i].screen_name, entity.getValue());
                        Assert.AreEqual(test.expected[i].list_slug, entity.getListSlug());
                        Assert.AreEqual(test.expected[i].indices[0], entity.getStart());
                        Assert.AreEqual(test.expected[i].indices[1], entity.getEnd());
                    }
                } catch (Exception) {
                    failures.Add(test.description + ": " + test.text);
                }
            }
            if (failures.Any()) {
                Assert.Fail(string.Join("\n", failures));
            }
        }

        [TestMethod]
        public void ExtractRepliesTest() {
            var failures = new List<string>();
            foreach (var test in LoadTests<dynamic>("extract.yml", "replies")) {
                try {
                    string actual = extractor.extractReplyScreenname(test.text);
                    Assert.AreEqual(test.expected, actual);
                } catch (Exception) {
                    failures.Add(test.description + ": " + test.text);
                }
            }
            if (failures.Any()) {
                Assert.Fail(string.Join("\n", failures));
            }
        }

        [TestMethod]
        public void ExtractUrlsTest() {
            var failures = new List<string>();
            foreach (var test in LoadTests<dynamic>("extract.yml", "urls")) {
                try {
                    List<string> actual = extractor.extractURLs(test.text);
                    CollectionAssert.AreEquivalent(test.expected, actual);
                } catch (Exception) {
                    failures.Add(test.description + ": " + test.text);
                }
            }
            if (failures.Any()) {
                Assert.Fail(string.Join("\n", failures));
            }
        }

        [TestMethod]
        public void ExtractUrlsWithIndicesTest() {
            var failures = new List<string>();
            foreach (var test in LoadTests<dynamic>("extract.yml", "urls_with_indices")) {
                try {
                    List<Entity> actual = extractor.extractURLsWithIndices(test.text);
                    for (int i = 0; i < actual.Count; i++) {
                        var entity = actual[i];
                        Assert.AreEqual(test.expected[i].url, entity.getValue());
                        Assert.AreEqual(test.expected[i].indices[0], entity.getStart());
                        Assert.AreEqual(test.expected[i].indices[1], entity.getEnd());
                    }
                } catch (Exception) {
                    failures.Add(test.description + ": " + test.text);
                }
            }
            if (failures.Any()) {
                Assert.Fail(string.Join("\n", failures));
            }
        }

        [TestMethod]
        public void ExtractHashtagsTest() {
            var failures = new List<string>();
            foreach (var test in LoadTests<dynamic>("extract.yml", "hashtags")) {
                try {
                    List<string> actual = extractor.extractHashtags(test.text);
                    CollectionAssert.AreEquivalent(test.expected, actual);
                } catch (Exception) {
                    failures.Add(test.description + ": " + test.text);
                }
            }
            if (failures.Any()) {
                Assert.Fail(string.Join("\n", failures));
            }
        }

        [TestMethod]
        public void ExtractHashtagsWithIndicesTest() {
            var failures = new List<string>();
            foreach (var test in LoadTests<dynamic>("extract.yml", "hashtags_with_indices")) {
                try {
                    List<Entity> actual = extractor.extractHashtagsWithIndices(test.text);
                    for (int i = 0; i < actual.Count; i++) {
                        var entity = actual[i];
                        Assert.AreEqual(test.expected[i].hashtag, entity.getValue());
                        Assert.AreEqual(test.expected[i].indices[0], entity.getStart());
                        Assert.AreEqual(test.expected[i].indices[1], entity.getEnd());
                    }
                } catch (Exception) {
                    failures.Add(test.description + ": " + test.text);
                }
            }
            if (failures.Any()) {
                Assert.Fail(string.Join("\n", failures));
            }
        }

        [TestMethod]
        public void ExtractCashtagsTest() {
            var failures = new List<string>();
            foreach (var test in LoadTests<dynamic>("extract.yml", "cashtags")) {
                try {
                    List<string> actual = extractor.extractCashtags(test.text);
                    CollectionAssert.AreEquivalent(test.expected, actual);
                } catch (Exception) {
                    failures.Add(test.description + ": " + test.text);
                }
            }
            if (failures.Any()) {
                Assert.Fail(string.Join("\n", failures));
            }
        }

        [TestMethod]
        public void ExtractCashtagsWithIndicesTest() {
            var failures = new List<string>();
            foreach (var test in LoadTests<dynamic>("extract.yml", "cashtags_with_indices")) {
                try {
                    List<Entity> actual = extractor.extractCashtagsWithIndices(test.text);
                    for (int i = 0; i < actual.Count; i++) {
                        var entity = actual[i];
                        Assert.AreEqual(test.expected[i].cashtag, entity.getValue());
                        Assert.AreEqual(test.expected[i].indices[0], entity.getStart());
                        Assert.AreEqual(test.expected[i].indices[1], entity.getEnd());
                    }
                } catch (Exception) {
                    failures.Add(test.description + ": " + test.text);
                }
            }
            if (failures.Any()) {
                Assert.Fail(string.Join("\n", failures));
            }
        }

        [TestMethod]
        public void AutolinkUsernamesTest() {
            var failures = new List<string>();
            foreach (var test in LoadTests<string>("autolink.yml", "usernames")) {
                try {
                    string actual = autolink.autoLinkUsernamesAndLists(test.text);
                    Assert.AreEqual(test.expected, actual);
                } catch (Exception) {
                    failures.Add(test.description + ": " + test.text);
                }
            }
            if (failures.Any()) {
                Assert.Fail(string.Join("\n", failures));
            }
        }

        [TestMethod]
        public void AutolinkListsTest() {
            var failures = new List<string>();
            foreach (var test in LoadTests<string>("autolink.yml", "lists")) {
                try {
                    string actual = autolink.autoLinkUsernamesAndLists(test.text);
                    Assert.AreEqual(test.expected, actual);
                } catch (Exception) {
                    failures.Add(test.description + ": " + test.text);
                }
            }
            if (failures.Any()) {
                Assert.Fail(string.Join("\n", failures));
            }
        }

        [TestMethod]
        public void AutolinkHashtagsTest() {
            var failures = new List<string>();
            foreach (var test in LoadTests<string>("autolink.yml", "hashtags")) {
                try {
                    string actual = autolink.autoLinkHashtags(test.text);
                    Assert.AreEqual(test.expected, actual);
                } catch (Exception) {
                    failures.Add(test.description + ": " + test.text);
                }
            }
            if (failures.Any()) {
                Assert.Fail(string.Join("\n", failures));
            }
        }

        [TestMethod]
        public void AutolinkUrlsTest() {
            var failures = new List<string>();
            foreach (var test in LoadTests<string>("autolink.yml", "urls")) {
                try {
                    string actual = autolink.autoLinkURLs(test.text);
                    Assert.AreEqual(test.expected, actual);
                } catch (Exception) {
                    failures.Add(test.description + ": " + test.text);
                }
            }
            if (failures.Any()) {
                Assert.Fail(string.Join("\n", failures));
            }
        }

        [TestMethod]
        public void AutolinkCashtagsTest() {
            var failures = new List<string>();
            foreach (var test in LoadTests<string>("autolink.yml", "cashtags")) {
                try {
                    string actual = autolink.autoLinkCashtags(test.text);
                    Assert.AreEqual(test.expected, actual);
                } catch (Exception) {
                    failures.Add(test.description + ": " + test.text);
                }
            }
            if (failures.Any()) {
                Assert.Fail(string.Join("\n", failures));
            }
        }

        [TestMethod]
        public void AutolinkAllTest() {
            var failures = new List<string>();
            foreach (var test in LoadTests<string>("autolink.yml", "all")) {
                try {
                    string actual = autolink.autoLink(test.text);
                    Assert.AreEqual(test.expected, actual);
                } catch (Exception) {
                    failures.Add(test.description + ": " + test.text);
                }
            }
            if (failures.Any()) {
                Assert.Fail(string.Join("\n", failures));
            }
        }

        [TestMethod]
        public void AutolinkJsonTest() {
            var failures = new List<string>();
            foreach (var test in LoadTests<string>("autolink.yml", "json")) {
                try {
                    string actual = autolink.autoLink(test.text);
                    Assert.AreEqual(test.expected, actual);
                } catch (Exception) {
                    failures.Add(test.description + ": " + test.text);
                }
            }
            if (failures.Any()) {
                Assert.Fail(string.Join("\n", failures));
            }
        }

        [TestMethod]
        public void HighlightPlainTextTest() {
            var failures = new List<string>();
            foreach (var test in LoadTests<string>("hit_highlighting.yml", "plain_text")) {
                try {
                    string actual = highlighter.highlight(test.text, test.hits);
                    Assert.AreEqual(test.expected, actual);
                } catch (Exception) {
                    failures.Add(test.description + ": " + test.text);
                }
            }
            if (failures.Any()) {
                Assert.Fail(string.Join("\n", failures));
            }
        }

        [TestMethod]
        public void HighlightWithLinksTest() {
            var failures = new List<string>();
            foreach (var test in LoadTests<string>("hit_highlighting.yml", "with_links")) {
                try {
                    string actual = highlighter.highlight(test.text, test.hits);
                    Assert.AreEqual(test.expected, actual);
                } catch (Exception) {
                    failures.Add(test.description + ": " + test.text);
                }
            }
            if (failures.Any()) {
                Assert.Fail(string.Join("\n", failures));
            }
        }

        [TestMethod]
        public void ValidateTweetsTest() {
            var failures = new List<string>();
            foreach (var test in LoadTests<bool>("validate.yml", "tweets")) {
                try {
                    bool actual = validator.isValidTweet(test.text);
                    Assert.AreEqual(test.expected, actual);
                } catch (Exception) {
                    failures.Add(test.description + ": " + test.text);
                }
            }
            if (failures.Any()) {
                Assert.Fail(string.Join("\n", failures));
            }            
        }

        // Really ugly code to parse the YAML files...
        private IList<dynamic> LoadTests<TExpected>(string file, string type) {
            Dictionary<string, List<dynamic>> dict = null;
            if (!testdict.TryGetValue(file, out dict)) {

                // load yaml file
                var stream = new StreamReader(Path.Combine(TestContext.TestDeploymentDir, file));
                var yaml = new YamlStream();
                yaml.Load(stream);

                // load tests
                var root = yaml.Documents[0].RootNode as YamlMappingNode;
                var tests = root.Children[new YamlScalarNode("tests")] as YamlMappingNode;
                dict = new Dictionary<string, List<dynamic>>();
                foreach (var entry in tests.Children) {
                    var sect = entry.Key as YamlScalarNode;
                    var items = entry.Value as YamlSequenceNode;
                    var list = new List<dynamic>();
                    foreach (YamlMappingNode item in items) {
                        dynamic test = new ExpandoObject();
                        test.description = ConvertNode<string>(item.Children.Single(x => x.Key.ToString() == "description").Value);
                        test.text = ConvertNode<string>(item.Children.Single(x => x.Key.ToString() == "text").Value);
                        test.expected = ConvertNode<TExpected>(item.Children.Single(x => x.Key.ToString() == "expected").Value);
                        test.hits = ConvertNode<List<List<int>>>(item.Children.SingleOrDefault(x => x.Key.ToString() == "hits").Value);
                        list.Add(test);
                    }
                    dict.Add(sect.Value, list);
                }
                testdict.Add(file, dict);
            }

            return dict[type];
        }

        private dynamic ConvertNode<T>(YamlNode node) {
            dynamic dynnode = node as dynamic;

            if (node is YamlScalarNode) {
                if (string.IsNullOrEmpty(dynnode.Value)) {
                    return null;
                } else if (typeof(T) == typeof(int)) {
                    return int.Parse(dynnode.Value);
                } else if (typeof(T) == typeof(bool)) {
                    return bool.Parse(dynnode.Value);
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

    }
}

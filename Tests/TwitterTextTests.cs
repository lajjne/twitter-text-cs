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
        private Extractor extractor = new Extractor();
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void ExtractMentionsTest() {
            var failures = new List<string>();
            foreach (var test in LoadTests("extract.yml", "mentions")) {
                List<string> actual = extractor.extractMentionedScreennames(test.text);
                try {
                    CollectionAssert.AreEquivalent(test.expected, actual);
                } catch (AssertFailedException) {
                    failures.Add(test.description);
                }
            }
            if (failures.Any()) {
                Assert.Fail(string.Join("\n", failures));
            }
        }

        [TestMethod]
        public void ExtractMentionsWithIndicesTest() {
            var failures = new List<string>();
            foreach (var test in LoadTests("extract.yml", "mentions_with_indices")) {
                List<Entity> actual = extractor.extractMentionedScreennamesWithIndices(test.text);
                try {
                    for (int i = 0; i < actual.Count; i++) {
                        var entity = actual[i];
                        Assert.AreEqual(test.expected[i].screen_name, entity.getValue());
                        Assert.AreEqual(test.expected[i].indices[0], entity.getStart());
                        Assert.AreEqual(test.expected[i].indices[1], entity.getEnd());
                    }
                } catch (AssertFailedException) {
                    failures.Add(test.description);
                }
            }
            if (failures.Any()) {
                Assert.Fail(string.Join("\n", failures));
            }
        }

        [TestMethod]
        public void ExtractMentionsOrListsWithIndicesTest() {
            var failures = new List<string>();
            foreach (var test in LoadTests("extract.yml", "mentions_or_lists_with_indices")) {
                List<Entity> actual = extractor.extractMentionsOrListsWithIndices(test.text);
                try {
                    for (int i = 0; i < actual.Count; i++) {
                        var entity = actual[i];
                        Assert.AreEqual(test.expected[i].screen_name, entity.getValue());
                        Assert.AreEqual(test.expected[i].list_slug, entity.getListSlug());
                        Assert.AreEqual(test.expected[i].indices[0], entity.getStart());
                        Assert.AreEqual(test.expected[i].indices[1], entity.getEnd());
                    }
                } catch (AssertFailedException) {
                    failures.Add(test.description);
                }
            }
            if (failures.Any()) {
                Assert.Fail(string.Join("\n", failures));
            }
        }

        [TestMethod]
        public void ExtractRepliesTest() {
            var failures = new List<string>();
            foreach (var test in LoadTests("extract.yml", "replies")) {
                string actual = extractor.extractReplyScreenname(test.text);
                try {
                    Assert.AreEqual(test.expected, actual);
                } catch (AssertFailedException) {
                    failures.Add(test.description);
                }
            }
            if (failures.Any()) {
                Assert.Fail(string.Join("\n", failures));
            }
        }

        [TestMethod]
        public void ExtractUrlsTest() {
            var failures = new List<string>();
            foreach (var test in LoadTests("extract.yml", "urls")) {
                List<string> actual = extractor.extractURLs(test.text);
                try {
                    CollectionAssert.AreEquivalent(test.expected, actual);
                } catch (AssertFailedException) {
                    failures.Add(test.description);
                }
            }
            if (failures.Any()) {
                Assert.Fail(string.Join("\n", failures));
            }
        }

        [TestMethod]
        public void ExtractUrlsWithIndicesTest() {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void ExtractHashtagsTest() {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void ExtractHashtagsWithIndicesTest() {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void ExtractCashtagsTest() {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void ExtractCashtagsWithIndicesTest() {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void AutolinkTest() {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void HighlightTest() {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void ValidateTest() {
            Assert.Inconclusive();
        }

        private IList<dynamic> LoadTests(string file, string type) {
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
                        test.description = ConvertNode<dynamic>(item.Children.Single(x => x.Key.ToString() == "description").Value);
                        test.text = ConvertNode<dynamic>(item.Children.Single(x => x.Key.ToString() == "text").Value);
                        test.expected = ConvertNode<dynamic>(item.Children.Single(x => x.Key.ToString() == "expected").Value);
                        test.hits = ConvertNode<dynamic>(item.Children.SingleOrDefault(x => x.Key.ToString() == "hits").Value);
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
                } else {
                    return dynnode.Value;
                }
            } else if (node is YamlSequenceNode) {
                var list = new List<dynamic>();
                foreach (var item in dynnode.Children) {
                    list.Add(ConvertNode<T>(item));
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

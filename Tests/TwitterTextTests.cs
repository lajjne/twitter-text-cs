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
        private Dictionary<string, Dictionary<string, List<dynamic>>> _testdata = new Dictionary<string, Dictionary<string, List<dynamic>>>();

        public TestContext TestContext { get; set; }

      

        [TestMethod]
        public void ExtractMentionsTest() {
            foreach (var test in LoadTests("extract.yml", "mentions")) {
                var actual = TweetExtensions.ExtractUsernames(test.Text);
                CollectionAssert.AreEquivalent(test.Expected, actual, test.Description);
            }
        }

        [TestMethod]
        public void ExtractMentionsWithIndicesTest() {
            Extractor extractor = new Extractor();
            foreach (var test in LoadTests("extract.yml", "mentions_with_indices")) {
                var actual = extractor.extractMentionedScreennamesWithIndices(test.Text);
                CollectionAssert.AreEquivalent(test.Expected, actual, test.Description);
            }
        }

        [TestMethod]
        public void ExtractMentionsOrListsWithIndicesTest() {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void ExtractRepliesTest() {
            foreach (var test in LoadTests("extract.yml", "replies")) {
                var actual = ((string)test.Text).ExtractReplyUsername();
                Assert.AreEqual(test.Expected, actual, test.Description);
            }
        }

        [TestMethod]
        public void ExtractUrlsTest() {
            Assert.Inconclusive();
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
            if (!_testdata.TryGetValue(file, out dict)) {

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
                        test.Description = ConvertNode(item.Children.Single(x => x.Key.ToString() == "description").Value);
                        test.Text = ConvertNode(item.Children.Single(x => x.Key.ToString() == "text").Value);
                        test.Expected = ConvertNode(item.Children.Single(x => x.Key.ToString() == "expected").Value);
                        test.Hits = ConvertNode(item.Children.SingleOrDefault(x => x.Key.ToString() == "hits").Value);
                        list.Add(test);
                    }
                    dict.Add(sect.Value, list);
                }
                _testdata.Add(file, dict);
            }

            return dict[type];
        }

        private dynamic ConvertNode(YamlNode node) {
            dynamic dynnode = node as dynamic;

            if (node is YamlScalarNode) {
                return string.IsNullOrEmpty(dynnode.Value) ? null : dynnode.Value;
            } else if (node is YamlSequenceNode) {
                var list = new List<dynamic>();
                foreach (var item in dynnode.Children) {
                    list.Add(ConvertNode(item));
                }
                return list;
            } else if (node is YamlMappingNode) {
                dynamic mapnode = new ExpandoObject();
                foreach (var item in ((YamlMappingNode)node).Children) {
                    ((IDictionary<string, object>)mapnode).Add(item.Key.ToString(), ConvertNode(item.Value));
                }
                return mapnode;
            }
            return null;
        }

    }
}

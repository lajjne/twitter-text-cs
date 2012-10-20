using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YamlDotNet.RepresentationModel;
using TwitterText;

namespace Tests {

    public class TestData {
        public string Description { get; set; }
        public string Text { get; set; }
        public List<string> Expected { get; set; }
        public List<string> Actual { get; set; }
    }

    [TestClass]
    public class TwitterTextTests {

        public TestContext TestContext { get; set; }

        public IEnumerable<TestData> LoadTests(string file, string section) {
            // load YAML file
            var stream = new StreamReader(Path.Combine(TestContext.TestDeploymentDir, file));
            var yaml = new YamlStream();
            yaml.Load(stream);

            // list all the items
            var root = (YamlMappingNode)yaml.Documents[0].RootNode;
            var tests = root.Children[new YamlScalarNode("tests")] as YamlMappingNode;
            var items = tests.Children[new YamlScalarNode(section)] as YamlSequenceNode;
            foreach (YamlMappingNode item in items) {
                yield return new TestData {
                    Description = item.Children[new YamlScalarNode("description")].ToString(),
                    Text = item.Children[new YamlScalarNode("text")].ToString(),
                    Expected = ((YamlSequenceNode)item.Children[new YamlScalarNode("expected")]).Children.ToList().ConvertAll(x => x.ToString())
                };
            }
        }

        [TestMethod]
        public void ExtractMentionsTest() {
            foreach (var test in LoadTests("extract.yml", "mentions")) {
                CollectionAssert.AreEquivalent(test.Expected, test.Text.ExtractUsernames(), test.Description);
            }
        }

        [TestMethod]
        public void ExtractMentionsWithIndicesTest() {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void ExtractMentionsOrListsWithIndicesTest() {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void ExtractRepliesTest() {
            Assert.Inconclusive();
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

    }
}

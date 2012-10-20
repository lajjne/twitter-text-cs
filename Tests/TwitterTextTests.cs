using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YamlDotNet.RepresentationModel;

namespace Tests {

    public class TestData {
        public string Description { get; set; }
        public string Text { get; set; }
        public string Expected { get; set; }
    }

    [TestClass]
    public class TwitterTextTests {

        public TestContext TestContext { get; set; }

        public IList<TestData> LoadTests(string file, string section) {
            var tests = new List<TestData>();

            // load YAML file
            var dir = Path.Combine(TestContext.TestRunDirectory, "twitter-text-conformance");
            var stream = new StreamReader(Path.Combine(dir, file));
            var yaml = new YamlStream();
            yaml.Load(stream);

            // Examine the stream
            var root = (YamlMappingNode)yaml.Documents[0].RootNode;

            foreach (var entry in root.Children) {
                Console.WriteLine(((YamlScalarNode)entry.Key).Value);
            }

            //raise  "No such test suite: #{test_type.to_s}" unless yaml["tests"][test_type.to_s]


            // List all the items
            var items = (YamlSequenceNode)root.Children[new YamlScalarNode("section")];
            foreach (YamlMappingNode item in items) {
                Console.WriteLine(
                    "{0}\t{1}",
                    item.Children[new YamlScalarNode("description")],
                    item.Children[new YamlScalarNode("text")]
                );

                tests.Add(new TestData());
            }
            return tests;
        }



        [TestMethod]
        public void ExtractMentionsTest() {
            var tests = LoadTests("extract.yml", "mentions");
            Assert.Fail("Extract mentions failed");
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

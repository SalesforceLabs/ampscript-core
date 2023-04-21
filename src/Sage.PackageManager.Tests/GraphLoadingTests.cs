// Copyright (c) 2023, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

namespace Sage.PackageManager.Tests
{
    public class GraphLoadingTests
    {
        [TestCase("SampleJsonPackage.json")]
        public void LoadJsonGraphs(string filename)
        {
            FileInfo testPath = new FileInfo(Path.Join(Environment.CurrentDirectory, "TestFiles", filename));

            PackageGraph graph = PackageGraph.FromJson(testPath);
        }

        [Test]
        public void SampleJsonTest()
        {
            var graph = PackageGraph.FromJson(new FileInfo(Path.Join(Environment.CurrentDirectory, "TestFiles", "SampleJsonPackage.json")));

            Assert.That(1 == graph.SelectedEntities.Count);

            var asset = graph.GetAsset(1234);
            Assert.IsNotNull(asset);

            Assert.That("Content" == asset.Compile());
        }
    }
}
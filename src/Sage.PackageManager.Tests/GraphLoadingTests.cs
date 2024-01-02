// Copyright (c) 2023, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using Microsoft.Extensions.DependencyInjection;
using Sage.PackageManager.DependencyInjection;

namespace Sage.PackageManager.Tests
{
    public class GraphLoadingTests
    {
        [TestCase("SampleJsonPackage.json")]
        public void LoadJsonGraphs(string filename)
        {
            var collection = new ServiceCollection();

            collection.AddJsonPackageLoader(o =>
            {
                o.Source = new FileInfo(Path.Join(TestContext.CurrentContext.TestDirectory, "TestFiles", filename));
                o.SourceId = "1234";
            });

            ServiceProvider provider = collection.BuildServiceProvider();

            PackageGraph graph = provider.GetRequiredService<IPackageLoader>().Load();
        }

        [Test]
        public void SampleJsonTest()
        {
            var collection = new ServiceCollection();

            collection.AddJsonPackageLoader(o =>
            {
                o.Source = new FileInfo(
                    Path.Join(TestContext.CurrentContext.TestDirectory, "TestFiles", "SampleJsonPackage.json"));
                o.SourceId = "1234";
            });

            ServiceProvider provider = collection.BuildServiceProvider();

            PackageGraph graph = provider.GetRequiredService<IPackageLoader>().Load();

            Assert.That(1 == graph.SelectedEntities.Count);

            Asset? asset = graph.GetAsset(1234);
            Assert.That(asset, Is.Not.Null);

            Assert.That("Content" == asset!.Compile(graph));
        }
    }
}

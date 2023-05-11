// Copyright (c) 2023, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using System.Globalization;
using System.Text.Json;

namespace Sage.PackageManager.Tests
{
    public class ContentCompilerTests
    {
        [TestCase("HtmlPaste")]
        [TestCase("AmpscriptAttributes")]
        [TestCase("TemplateBasedEmail")]
        [TestCase("ComplexTemplateBasedEmail")]
        [TestCase("CONTENTTEMPLATES_1_3_1_ALT_COLUMN")]
        public void ValidateContentCompilation(string filename)
        {
            var expected = new FileInfo(Path.Join(Environment.CurrentDirectory, "TestFiles", $"{filename}.compiled.html"));

            var input = new FileInfo(Path.Join(Environment.CurrentDirectory, "TestFiles", $"{filename}.json"));

            DataObjects.Asset? message = JsonSerializer.Deserialize<DataObjects.Asset>(File.ReadAllText(input.FullName));

            Assert.That(message != null);

            string compiled = ContentCompiler.Compile(message!);

            string expectedContents = File.ReadAllText(expected.FullName).Trim();

            compiled = compiled.Replace("\r\n", "\n").Trim();
            expectedContents = expectedContents.Replace("\r\n", "\n").Trim();
            int compare = string.Compare(compiled, expectedContents);

            if (compare != 0)
            {
                Assert.That(compiled, Is.EqualTo(expectedContents));
            }
        }
    }
}
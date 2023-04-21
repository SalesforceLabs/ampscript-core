// Copyright (c) 2023, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

namespace Sage.PackageManager
{
    /// <summary>
    /// A representation of a packaged category in PackageManager.
    /// </summary>
    public class Category : Entity
    {
        private readonly DataObjects.PackagedCategory _packagedCategory;

        internal Category(DataObjects.PackagedCategory packagedCategory) : base(packagedCategory)
        {
            this._packagedCategory = packagedCategory;
        }
    }
}

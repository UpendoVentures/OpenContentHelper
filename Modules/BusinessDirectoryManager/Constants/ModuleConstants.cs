/*
Copyright © Upendo Ventures, LLC

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and
associated documentation files (the "Software"), to deal in the Software without restriction,
including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so,
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial
portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT
NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES
OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Upendo.Modules.BusinessDirectoryManager.Constants
{
    /// <summary>
    /// Constants used within the module.
    /// </summary>
    public class ModuleConstants
    {
        /// <summary>
        /// The root path for .resx files.
        /// </summary>
        public const string ResxPartialRoot = "~\\DesktopModules\\UpendoBusinessDirectoryManager\\App_LocalResources\\";

        /// <summary>
        /// The attribute name used in XML data nodes.
        /// </summary>
        public const string DataNodeAttributesName = "name";

        /// <summary>
        /// The path to the root data in XML.
        /// </summary>
        public const string RootData = "/root/data";

        /// <summary>
        /// The text suffix for keys ending with a period.
        /// </summary>
        public const string PointText = ".text";

        /// <summary>
        /// The name of the module folder.
        /// </summary>
        public const string ModuleFolderName = "UpendoBusinessDirectoryManager";

        /// <summary>
        /// The name of the supported module.
        /// </summary>
        public const string SupportedModules = "UpendoBusinessDirectoryManager";

        /// <summary>
        /// The text used for editing.
        /// </summary>
        public const string EditText = "EDIT";

        /// <summary>
        /// The prefix used for database tables.
        /// </summary>
        public const string DBTABLE_PREFIX = "bcc";
    }
}
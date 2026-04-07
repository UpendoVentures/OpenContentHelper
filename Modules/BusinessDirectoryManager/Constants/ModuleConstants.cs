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
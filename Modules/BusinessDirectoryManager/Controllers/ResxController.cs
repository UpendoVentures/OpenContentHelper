using DotNetNuke.Instrumentation;
using DotNetNuke.Security;
using DotNetNuke.Services.Localization;
using DotNetNuke.Web.Api;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Xml;
using Upendo.Modules.BusinessDirectoryManager.Constants;

namespace Upendo.Modules.BusinessDirectoryManager.Controllers
{
    /// <summary>
    /// Controller for retrieving localized resource strings.
    /// </summary>
    [SupportedModules(ModuleConstants.SupportedModules)]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
    public class ResxController : DnnApiController
    {
        private static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof(ResxController));

        /// <summary>
        /// Retrieves localized resource strings from a .resx file.
        /// </summary>
        /// <param name="filename">The name of the .resx file (without the .resx extension).</param>
        /// <returns>An HTTP response containing the localized resource strings in JSON format.</returns>
        [HttpGet]
        [ActionName("GetResx")]
        public HttpResponseMessage GetResx(string filename)
        {
            try
            {
                var resx = new JObject();

                var resxRoot = $"{ModuleConstants.ResxPartialRoot}{filename}.resx";
                var filepath = HttpContext.Current.Server.MapPath(resxRoot);
                var resxDoc = new XmlDocument();
                resxDoc.Load(filepath);

                foreach (XmlNode dataNode in resxDoc.DocumentElement.SelectNodes(ModuleConstants.RootData))
                {
                    var key = dataNode.Attributes[ModuleConstants.DataNodeAttributesName].Value;
                    var val = Localization.GetString(key.ToString(), resxRoot);

                    if (key.EndsWith(ModuleConstants.PointText, StringComparison.InvariantCultureIgnoreCase)) key = key.Substring(0, key.Length - 5);
                    key = key.Replace(".", "_");

                    resx.Add(key, val);
                }

                return Request.CreateResponse(resx);
            }
            catch (Exception e)
            {
                LogError(e);
                Console.WriteLine(e);
                throw;
            }
        }

        #region Private Helper Methods 

        private static void LogError(Exception ex)
        {
            if (ex != null)
            {
                Logger.Error(ex.Message, ex);

                // TODO: Uncomment when we move the common logic to a shared library
                //var ctlDnn = new DnnDataHelper();
                //ctlDnn.LogErrorToAdminLog(ex);

                if (ex.InnerException != null)
                {
                    Logger.Error(ex.InnerException.Message, ex.InnerException);
                }
            }
        }

        #endregion
    }
}

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

using System.Data.SqlClient;
using DotNetNuke.Common.Utilities;

namespace Upendo.SkinObjects.OpenContentHelper.Helpers
{
    public class ConfigBase
    {
        public static string GetConnectionString()
        {
            // DNN standard:
            // - ConnectionStrings["SiteSqlServer"] in web.config
            // DotNetNuke.Common.Utilities.Config has helpers, but this is explicit.
            return System.Configuration.ConfigurationManager.ConnectionStrings[Common.Constants.ConnectionStringName].ConnectionString;
        }
        
        public static SqlConnection GetOpenConnection()
        {
            // Uses the DNN site DB connection string
            // DNN typically stores it under "SiteSqlServer"
            var cs = Config.GetConnectionString();
            var cn = new SqlConnection(cs);
            cn.Open();
            return cn;
        }
    }
}
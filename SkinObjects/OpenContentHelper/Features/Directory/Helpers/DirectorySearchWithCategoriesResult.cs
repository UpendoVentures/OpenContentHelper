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

using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Upendo.OpenContentHelper.Features.Directory.Models;
using Upendo.SkinObjects.OpenContentHelper.Helpers;

namespace Upendo.OpenContentHelper.Features.Directory.Helpers
{
    public sealed class DirectorySearchWithCategoriesResult
    {
        public PagedResult<CompanyDirectoryItem> Results { get; set; }

        // Backward-compatible: use Dictionary<int, List<CategoryDto>>
        public Dictionary<int, List<CategoryDto>> CategoriesByCompanyId { get; set; }

        public DirectorySearchWithCategoriesResult()
        {
            Results = new PagedResult<CompanyDirectoryItem>();
            CategoriesByCompanyId = new Dictionary<int, List<CategoryDto>>();
        }

        // ----------------------------
        // SearchCompaniesWithCategoryNames(portalId, req)
        // ----------------------------
        public static DirectorySearchWithCategoriesResult SearchCompaniesWithCategoryNames(int portalId, DirectorySearchRequest req)
        {
            // First: get the paged results using your existing method
            PagedResult<CompanyDirectoryItem> results = ChamberMembershipController.SearchCompanies(portalId, req);

            DirectorySearchWithCategoriesResult output = new DirectorySearchWithCategoriesResult();
            output.Results = results;
            output.CategoriesByCompanyId = new Dictionary<int, List<CategoryDto>>();

            // If empty, return quickly
            if (results == null || results.Items == null || results.Items.Count == 0)
            {
                return output;
            }

            // Build distinct companyIds (no LINQ)
            List<int> companyIds = new List<int>();
            for (int i = 0; i < results.Items.Count; i++)
            {
                CompanyDirectoryItem it = results.Items[i];
                if (it != null)
                {
                    int id = it.CompanyId;
                    if (!companyIds.Contains(id))
                    {
                        companyIds.Add(id);
                    }
                }
            }

            if (companyIds.Count == 0)
            {
                return output;
            }

            // Build parameterized IN list
            List<SqlParameter> parms = new List<SqlParameter>();
            StringBuilder inList = new StringBuilder();

            for (int i = 0; i < companyIds.Count; i++)
            {
                string paramName = "@Id" + i.ToString();
                SqlParameter p = new SqlParameter(paramName, SqlDbType.Int);
                p.Value = companyIds[i];

                parms.Add(p);

                if (i > 0)
                {
                    inList.Append(", ");
                }
                inList.Append(paramName);
            }

            string sql =
                "SELECT\r\n" +
                "    cc.CompanyId,\r\n" +
                "    cat.CategoryId,\r\n" +
                "    cat.CategoryName,\r\n" +
                "    cat.Slug,\r\n" +
                "    cc.IsPrimary\r\n" +
                "FROM dbo.bcc_CompanyCategory cc\r\n" +
                "INNER JOIN dbo.bcc_Category cat ON cat.CategoryId = cc.CategoryId\r\n" +
                "WHERE cc.CompanyId IN (" + inList.ToString() + ")\r\n" +
                "  AND cat.IsActive = 1\r\n" +
                "ORDER BY\r\n" +
                "    cc.CompanyId ASC,\r\n" +
                "    cc.IsPrimary DESC,\r\n" +
                "    cat.CategoryName ASC;";

            Dictionary<int, List<CategoryDto>> map = new Dictionary<int, List<CategoryDto>>();

            using (SqlConnection cn = ConfigBase.GetOpenConnection())
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                // Add parameters (these are unique instances, used only on this command)
                for (int i = 0; i < parms.Count; i++)
                {
                    cmd.Parameters.Add(parms[i]);
                }

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        int companyId = rdr.GetInt32(0);

                        List<CategoryDto> list;
                        if (!map.TryGetValue(companyId, out list))
                        {
                            list = new List<CategoryDto>();
                            map[companyId] = list;
                        }

                        CategoryDto dto = new CategoryDto();
                        dto.CategoryId = rdr.GetInt32(1);
                        dto.CategoryName = rdr.GetString(2);
                        dto.Slug = rdr.GetString(3);

                        list.Add(dto);
                    }
                }
            }

            output.CategoriesByCompanyId = map;
            return output;
        }
    }
}

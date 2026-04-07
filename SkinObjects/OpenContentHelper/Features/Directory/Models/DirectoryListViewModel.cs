using System.Collections.Generic;
using Upendo.OpenContentHelper.Features.Directory.Helpers;

namespace Upendo.OpenContentHelper.Features.Directory.Models
{
    public sealed class DirectoryListViewModel
    {
        public DirectorySearchRequest Request { get; set; }
        public List<CategoryDto> Categories { get; set; }
        public List<CompanyDirectoryItem> Items { get; set; }
        public DirectorySearchWithCategoriesResult Search { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int From { get; set; }
        public int To { get; set; }
        public string RedirectUrl { get; set; }
    }
}
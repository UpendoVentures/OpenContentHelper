using System.Collections.Generic;

namespace Upendo.Modules.BusinessDirectoryManager.Data
{
    public class BusinessDirectoryListResponseDto
    {
        public List<BusinessDirectoryListItemDto> Items { get; set; } = new List<BusinessDirectoryListItemDto>();

        public int TotalItems { get; set; }

        public int ActiveCount { get; set; }

        public int PublicCount { get; set; }

        public int FeaturedCount { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }
    }
}
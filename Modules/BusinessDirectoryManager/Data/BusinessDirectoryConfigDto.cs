using System.Collections.Generic;

namespace Upendo.Modules.BusinessDirectoryManager.Data
{
    public class BusinessDirectoryConfigDto
    {
        public IEnumerable<PortalOptionDto> Portals { get; set; }

        public IEnumerable<CategoryOptionDto> Categories { get; set; }

        public bool IsSuperUser { get; set; }
    }
}
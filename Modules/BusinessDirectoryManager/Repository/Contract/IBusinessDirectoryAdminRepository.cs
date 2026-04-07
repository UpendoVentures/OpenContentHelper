using System.Collections.Generic;
using Upendo.Modules.BusinessDirectoryManager.Data;

namespace Upendo.Modules.BusinessDirectoryManager.Repository.Contract
{
    public interface IBusinessDirectoryAdminRepository
    {
        BusinessDirectoryConfigDto GetConfig(int portalId, bool includeCategories = true);

        BusinessDirectoryListResponseDto GetBusinessesPage(BusinessDirectoryListRequestDto request);

        BusinessDirectoryEditDto GetBusinessForEdit(int portalId, int companyId);

        SlugAvailabilityDto CheckSlugAvailability(int portalId, string slug, int? companyId);

        int SaveBusiness(BusinessDirectorySaveRequest request, int currentUserId);
    }
}
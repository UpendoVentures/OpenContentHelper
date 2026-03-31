using System.Collections.Generic;
using Upendo.Modules.UpendoEventsForm.Data;

namespace Upendo.Modules.UpendoEventsForm.Repository.Contract
{
    public interface IEventAdminRepository
    {
        EventFormConfigDto GetConfig(int portalId);

        EventFormEditDto GetEventForEdit(int portalId, int eventId);

        int SaveEvent(EventFormSaveRequest request, int currentUserId);

        CategoryOptionDto CreateCategory(int portalId, string name);

        TagOptionDto CreateTag(int portalId, string name);

        bool IsSlugAvailable(int portalId, string slug, int? eventId);

        IList<LocationSuggestionDto> SearchVenueSuggestions(int portalId, string query, int limit);

        IList<EventFileOptionDto> GetFilesByFolder(int portalId, int folderId);

        EventFileOptionDto GetFileById(int portalId, int fileId);
    }
}
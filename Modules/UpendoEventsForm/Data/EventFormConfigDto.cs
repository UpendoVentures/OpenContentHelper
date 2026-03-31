using System.Collections.Generic;

namespace Upendo.Modules.UpendoEventsForm.Data
{
    public class EventFormConfigDto
    {
        public IEnumerable<PortalOptionDto> Portals { get; set; }

        public IEnumerable<CategoryOptionDto> Categories { get; set; }

        public IEnumerable<TagOptionDto> Tags { get; set; }

        public IEnumerable<UserOptionDto> Organizers { get; set; }

        public IEnumerable<FolderOptionDto> Folders { get; set; }

        public IEnumerable<EventFileOptionDto> Files { get; set; }

        public IEnumerable<TimeZoneOptionDto> TimeZoneOptions { get; set; }

        public string DefaultTimeZoneId { get; set; }

        public bool IsSuperUser { get; set; }
    }
}
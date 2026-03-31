namespace Upendo.Modules.UpendoEventsForm.Data
{
    public class CategoryOptionDto
    {
        public int EventCategoryId { get; set; }

        public int? PortalId { get; set; }

        public string CategoryName { get; set; }

        public string Slug { get; set; }

        public string Description { get; set; }

        public int SortOrder { get; set; }

        public bool IsActive { get; set; }
    }
}
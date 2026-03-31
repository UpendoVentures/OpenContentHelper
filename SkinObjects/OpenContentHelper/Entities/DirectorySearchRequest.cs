
namespace Upendo.SkinObjects.OpenContentHelper.Entities
{
    public sealed class DirectorySearchRequest
    {
        public string Query { get; set; } = string.Empty;
        public string CategorySlug { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public byte? MembershipStatus { get; set; } = null;  // 1,2,3
        public bool FeaturedOnly { get; set; } = false;

        public string Sort { get; set; } = "default"; // default | name | featured
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 24;

        public int? RandomSeed { get; set; }
    }
}

namespace Upendo.OpenContentHelper.Features.Directory.Models
{
    public sealed class CategoryDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
    }
}
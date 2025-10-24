namespace WebApplication1.Models
{
    public class NewsArticle
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime PublishedDate { get; set; }

        public string? ThumbnailUrl { get; set; }

        public List<StockEffect> Effects { get; set; } = new List<StockEffect>();
    }
}

namespace WebApplication1.Models
{
    public class StockEffect
    {
        public int Id { get; set; }
        public string Ticker { get; set; }
        public decimal PriceChange { get; set; }

        // Foreign key to link back to the news article
        public int NewsArticleId { get; set; }
    }
}

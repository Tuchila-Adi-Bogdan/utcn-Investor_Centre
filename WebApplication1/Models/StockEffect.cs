namespace InvestorCenter.Models
{
    public class StockEffect
    {
        public int Id { get; set; }
        public string Ticker { get; set; }
        public decimal PriceChange { get; set; }

        // FK to the news article
        public int NewsArticleId { get; set; }

        public DateTime ExpirationDate { get; set; }
    }

}

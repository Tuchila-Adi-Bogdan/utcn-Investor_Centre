namespace InvestorCenter.Models
{
    public class PriceHistory
    {
        public int Id { get; set; }
        public int StockId { get; set; }
        public decimal Price { get; set; }
        public DateTime Timestamp { get; set; }
    }
}

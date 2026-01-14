using System.ComponentModel.DataAnnotations.Schema;

namespace InvestorCenter.Models
{
    public class PortfolioItem
    {
        public int Id { get; set; }

        // Link to the User
        public string UserId { get; set; }

        // Link to the Stock
        public int StockId { get; set; }
        public Stock Stock { get; set; } // Navigation property

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal AveragePrice { get; set; } // Optional: To track profit/loss later
    }
}

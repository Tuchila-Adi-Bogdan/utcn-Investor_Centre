using System.ComponentModel.DataAnnotations.Schema;

namespace InvestorCenter.Models
{
    public class PortfolioItem
    {
        public int Id { get; set; }

        // FK the User
        public string UserId { get; set; }

        // FK to the Stock
        public int StockId { get; set; }
        public Stock Stock { get; set; }

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal AveragePrice { get; set; }
    }
}

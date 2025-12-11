using InvestorCenter.Models;

namespace InvestorCenter.ViewModels
{
    public class HomeViewModel
    {
        public IEnumerable<Stock> Stocks { get; set; }
        public IEnumerable<NewsArticle> NewsArticles { get; set; }
        public Dictionary<int, List<PriceHistory>> RecentHistory { get; set; }
        public Dictionary<int, decimal> PercentageChanges { get; set; }
    }
}

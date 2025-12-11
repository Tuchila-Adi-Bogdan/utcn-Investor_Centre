using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using InvestorCenter.Data;
using InvestorCenter.Hubs;
using InvestorCenter.Models;
using InvestorCenter.Services;

public class StockPriceService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IHubContext<StockHub> _hubContext;
    private readonly StockUpdateSettings _settings;

    public StockPriceService(IServiceProvider serviceProvider, IHubContext<StockHub> hubContext, StockUpdateSettings settings)
    {
        _serviceProvider = serviceProvider;
        _hubContext = hubContext;
        _settings = settings;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<InvestorCenterContext>();
                var stocks = await context.Stocks.ToListAsync(stoppingToken);
                var random = new Random();

                foreach (var stock in stocks)
                {
                    var changePercent = (decimal)(random.NextDouble() * 0.02 - 0.01);
                    stock.Price += stock.Price * changePercent;

                    var historyRecord = new PriceHistory
                    {
                        StockId = stock.Id,
                        Price = stock.Price,
                        Timestamp = DateTime.UtcNow
                    };
                    await context.PriceHistories.AddAsync(historyRecord, stoppingToken);
                }

                await context.SaveChangesAsync(stoppingToken);

                foreach (var stock in stocks)
                {
                    var latestHistory = await context.PriceHistories
                        .Where(p => p.StockId == stock.Id && p.Timestamp >= DateTime.UtcNow.AddMinutes(-1))
                        .OrderBy(p => p.Timestamp)
                        .ToListAsync(stoppingToken);

                    decimal percentChange = 0;
                    if (latestHistory.Count >= 2)
                    {
                        var lastPrice = latestHistory[^1].Price;
                        var prevPrice = latestHistory[^2].Price;
                        if (prevPrice != 0) percentChange = ((lastPrice - prevPrice) / prevPrice) * 100;
                    }

                    await _hubContext.Clients.All.SendAsync("ReceivePriceUpdate", stock.Id, stock.Price, latestHistory, percentChange, stoppingToken);
                }
            }

            await Task.Delay(_settings.UpdateDelayMs, stoppingToken);
        }
    }
}
using LuxuryResort.Data;
using Microsoft.EntityFrameworkCore;

namespace LuxuryResort.Services
{
    public class PaymentPendingCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<PaymentPendingCleanupService> _logger;
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(5);
        private readonly int _timeoutMinutes;

        public PaymentPendingCleanupService(IServiceProvider serviceProvider, ILogger<PaymentPendingCleanupService> logger, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _timeoutMinutes = configuration.GetValue<int>("Payment:PendingTimeoutMinutes", 30);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<LuxuryResortContext>();

                    var cutoff = DateTime.Now.AddMinutes(-_timeoutMinutes);
                    var expired = await db.Bookings
                        .Where(b => b.Status == "Payment_Pending" && b.BookingDate <= cutoff)
                        .ToListAsync(stoppingToken);

                    if (expired.Count > 0)
                    {
                        foreach (var b in expired)
                        {
                            b.Status = "Cancelled"; // transaction failed due to timeout
                        }
                        await db.SaveChangesAsync(stoppingToken);
                        _logger.LogInformation("Cancelled {Count} expired pending bookings.", expired.Count);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error running payment pending cleanup.");
                }

                await Task.Delay(_interval, stoppingToken);
            }
        }
    }
}




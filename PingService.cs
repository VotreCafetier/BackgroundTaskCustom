using StatusAPI;
using System.Net.NetworkInformation;

namespace StatusAPICore
{
    public class PingService : BackgroundService
    {
        private readonly ILogger<PingService> _logger;
        private readonly ICacheService _cacheService;

        public PingService(ILogger<PingService> logger, 
            ICacheService cacheService)
        {
            _logger = logger;
            _cacheService = cacheService;
        }

        public async Task<string> Ping(string address)
        {
            try
            {
                Ping myPing = new();
                PingReply reply = await myPing.SendPingAsync(address, 2000);
                if (reply == null) return "No response";
                return "up";
            }
            catch
            {
                return "There was an error";
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Job starts
            _logger.LogInformation("Starting {jobName}", nameof(PingService));

            // Continue until the app shuts down
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await _cacheService.RefreshDashboardCacheAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Job {jobName} threw an exception", nameof(PingService));
                }

                await Task.Delay(5000);
            }

            // Job ends
            _logger.LogInformation("Stopping {jobName}", nameof(PingService));
            _cacheService.RemoveDashboardCache();
        }
    }
}

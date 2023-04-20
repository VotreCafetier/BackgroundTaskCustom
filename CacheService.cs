using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using NetworkMapLibrary;
using StatusAPI.DTO;
using StatusAPICore;

namespace StatusAPI
{
    public interface ICacheService
    {
        Task RefreshDashboardCacheAsync();
        void RemoveDashboardCache();
    }

    public class CacheService : ICacheService
    {
        private readonly ILogger<PingService> _logger;
        private readonly IConfiguration _configuration;
        private readonly DBContext _context;
        private readonly IDistributedCache _cache;

        public CacheService(ILogger<PingService> logger,
            IConfiguration configuration,
            DBContext context,
            IDistributedCache cache)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _cache = cache;
        }

        public async Task RefreshDashboardCacheAsync()
        {
            // fetch cards
            List<Card> cards = await _context.Cards.ToListAsync();
            List<CardDTO> dto = new();

            // map cards
            cards.ForEach(c => {
                dto.Add(new CardDTO()
                {
                    Id = c.Id,
                    IPAddress = c.IPAddress,
                    Port = c.Port,
                    Status = ""
                });
            });
            byte[] encodedcard = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(dto));

            await _cache.SetAsync(_configuration["StatusCacheName"], encodedcard, new DistributedCacheEntryOptions());

            _logger.LogInformation("{cacheKey} cache refreshed", _configuration["Cache"]);
        }

        public void RemoveDashboardCache()
        {
            _cache.Remove(_configuration["Cache"]);
        }
    }
}

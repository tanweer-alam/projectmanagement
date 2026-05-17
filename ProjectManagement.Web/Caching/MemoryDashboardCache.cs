using Microsoft.Extensions.Caching.Memory;
using ProjectManagement.Application.Common.Caching;
using ProjectManagement.Application.DTOs;

namespace ProjectManagement.Web.Caching
{
    public class MemoryDashboardCache : IDashboardCache
    {
        private readonly IMemoryCache _memoryCache;

        public MemoryDashboardCache(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public bool TryGet(out DashboardDto? dashboard)
        {
            return _memoryCache.TryGetValue(CacheKeys.Dashboard, out dashboard);
        }

        public void Set(DashboardDto dashboard)
        {
            _memoryCache.Set(
                CacheKeys.Dashboard,
                dashboard,
                new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                });
        }

        public void Remove()
        {
            _memoryCache.Remove(CacheKeys.Dashboard);
        }
    }
}

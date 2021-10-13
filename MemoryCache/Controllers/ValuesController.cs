using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace MemoryCache.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private IMemoryCache _memoryCache;
        public ValuesController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        [HttpGet("v1/{key}")]
        public IActionResult GetV1(string key)
        {
            /// Get Or Crete - Format 1
            if (!_memoryCache.TryGetValue(key, out DateTime? cacheValue))
            {
                cacheValue = DateTime.Now;

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(10)); // TimeSpan.FromSeconds(10) sets expiration time for inactivity

                _memoryCache.Set(key, cacheValue, cacheOptions);
            }

            return Ok(
                new
                {
                    DataAtual = DateTime.Now,
                    DataCache = cacheValue
                });
        }

        [HttpGet("v2/{key}")]
        public IActionResult GetV2(string key)
        {
            /// Get Or Crete - Format 2
            if (!_memoryCache.TryGetValue(key, out DateTime? cacheValue))
            {
                cacheValue = DateTime.Now;
                _memoryCache.Set(key, cacheValue, TimeSpan.FromSeconds(10)); // expiration time for inactivity

            }

            return Ok(
                new
                {
                    DataAtual = DateTime.Now,
                    DataCache = cacheValue
                });
        }

        [HttpGet("v3/{key}")]
        public IActionResult GetV3(string key)
        {
            DateTime? cacheValue;
            /// Get Or Crete - Format 3
            cacheValue = _memoryCache.GetOrCreate(key, entry =>
            {
                entry.SetSlidingExpiration(TimeSpan.FromSeconds(10)); // expiration time for inactivity
                entry.SetAbsoluteExpiration(TimeSpan.FromSeconds(30)); // absolute expiration time
                return DateTime.Now;
            });

            return Ok(
                new
                {
                    DataAtual = DateTime.Now,
                    DataCache = cacheValue
                });
        }
        
        [HttpGet("v4/{key}")]
        public async Task<IActionResult> GetV4(string key)
        {
            DateTime cacheValue;
            /// Get Or Crete - Format 4 -> Format 3 async
            cacheValue = await _memoryCache.GetOrCreateAsync(key, entry =>
            {
                entry.SetSlidingExpiration(TimeSpan.FromSeconds(10));
                return Task.FromResult(DateTime.Now);
            });

            return Ok(
                new
                {
                    DataAtual = DateTime.Now,
                    DataCache = cacheValue
                });
        }

        [HttpGet("v5/{key}")]
        public async Task<IActionResult> GetV5(string key)
        {
            /// Only Get
            var cacheValue = _memoryCache.Get<DateTime?>(key);

            return Ok(
                new
                {
                    DataAtual = DateTime.Now,
                    DataCache = cacheValue
                });
        }
    }
}

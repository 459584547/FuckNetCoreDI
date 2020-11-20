using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using log4net;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Abstractions;
using WebApplicationTest.DAL;
using WebApplicationTest.Extension;

namespace WebApplicationTest.Services
{
    [Authorized]
    public class TestService2
    {
        public TestDbContext testDbContext { set; get; }
        public ILog _logger { set; get; }
        public IDistributedCache cache { set; get; }
        public IRedisCacheClient  redisCacheClient { set; get; }
        public IRedisDatabase redisDatabase { set; get; }

        public object x2()
        {
            _logger.Debug("TestService2---xxx22");
            _logger.Warn("Warn");

            redisDatabase.AddAsync("abc", "abc", TimeSpan.FromSeconds(100)).Wait();

            var s = redisDatabase.Database.Sort("cities");
            redisDatabase.Database.SortedSetAdd("cities", "a", 1.23, CommandFlags.None);


            cache.SetString("key", "123", new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(20)
            }); 
            return new
            {
                a = testDbContext.Xzqh2.Select(a => a),
                b = testDbContext.Xzqhs.ToList(),
                c = cache.GetString("key"),
                d = redisDatabase.GetAsync<string>("abc").Result
        };
        }
    }
}

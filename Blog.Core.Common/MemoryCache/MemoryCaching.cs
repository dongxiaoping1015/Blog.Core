﻿using System;
using Microsoft.Extensions.Caching.Memory;

namespace Blog.Core.Common.MemoryCache
{
    public class MemoryCaching : ICaching
    {
        private readonly IMemoryCache _cache;
        public MemoryCaching(IMemoryCache cache)
        {
            _cache = cache;
        }
        public object Get(string cacheKey)
        {
            return _cache.Get(cacheKey);
        }

        public void Set(string cacheKey, object cacheValue)
        {
            _cache.Set(cacheKey, cacheValue, TimeSpan.FromSeconds(7200));
        }
    }
}

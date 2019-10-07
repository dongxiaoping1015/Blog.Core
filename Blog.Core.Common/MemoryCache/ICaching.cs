using System;
namespace Blog.Core.Common.MemoryCache
{
    public interface ICaching
    {
        object Get(string cacheKey);
        void Set(string cacheKey, object cacheValue);
    }
}

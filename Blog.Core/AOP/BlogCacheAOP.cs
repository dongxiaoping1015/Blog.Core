using System;
using System.Linq;
using Blog.Core.Common;
using Blog.Core.Common.MemoryCache;
using Castle.DynamicProxy;

namespace Blog.Core.AOP
{
    /// <summary>
    /// 面向切面的缓存使用
    /// </summary>
    public class BlogCacheAOP : CacheAOPBase
    {
        // 通过注入的方式,把缓存操作接口通过构造函数注入
        private readonly ICaching _cache;
        public BlogCacheAOP(ICaching cache)
        {
            _cache = cache;
        }

        // Intercept方法是拦截的关键所在,也是IIntercaptor接口中的唯一定义
        public override void Intercept(IInvocation invocation)
        {
            // 获取自定义缓存键
            var cacheKey = CustomCacheKey(invocation);
            // 根据key获取相应的缓存值
            var cacheValue = _cache.Get(cacheKey);
            if (cacheValue != null)
            {
                // 将当前获取到的缓存值赋值给当前执行方法
                invocation.ReturnValue = cacheValue;
                return;
            }
            // 去执行当前的方法
            invocation.Proceed();
            // 存入缓存
            if (!string.IsNullOrWhiteSpace(cacheKey))
            {
                _cache.Set(cacheKey, invocation.ReturnValue);
            }
        }

        /// <summary>
        /// 自定义缓存的key
        /// </summary>
        /// <param name="invocation"></param>
        /// <returns></returns>
        protected string CustomCacheKey(IInvocation invocation)
        {
            var typeName = invocation.TargetType.Name;
            var methodName = invocation.Method.Name;
            var methodArguments = invocation.Arguments.Select(GetArgumentValue).Take(3).ToList();

            string key = $"{typeName}:{methodName}";
            foreach (var param in methodArguments)
            {
                key = $"{key}{param}:";
            }
            return key.TrimEnd(':');
        }

        /// <summary>
        /// object 转 string
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        protected string GetArgumentValue(object arg)
        {
            if (arg is DateTime || arg is DateTime?)
                return ((DateTime)arg).ToString("yyyyMMddHHmmss");
            if (arg is string || arg is ValueType || arg is Nullable)
                return arg.ToString();
            if (arg != null)
            {
                if (arg.GetType().IsClass)
                {
                    return Common.Helper.MD5Helper.MD5Encrypt16(Newtonsoft.Json.JsonConvert.SerializeObject(arg));
                }
            }
            return string.Empty;
        }
    }
}

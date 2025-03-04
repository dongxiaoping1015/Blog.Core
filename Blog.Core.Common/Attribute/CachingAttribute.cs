﻿using System;
namespace Blog.Core.Common
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class CachingAttribute : Attribute
    {
        /// <summary>
        /// 缓存绝对过期时间(分钟)
        /// </summary>
        public int AbsoluteExpiration { get; set; } = 30;
    }
}

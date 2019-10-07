using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Blog.Core.Model;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Blog.Core.Common;

namespace Blog.Core
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();

            // 创建可用于解析作用域服务的新 Microsoft.Extensions.DependencyInjection.IServiceScope。
            //using (var scope = host.Services.CreateScope())
            //{
            //    var services = scope.ServiceProvider;
            //    var loggerFactory = services.GetRequiredService<ILoggerFactory>();

            //    try
            //    {
            //        // 从 system.IServicec提供程序获取 T 类型的服务。
            //        // 为了大家的数据安全，这里先注释掉了，大家自己先测试玩一玩吧。
            //        // 数据库连接字符串是在 Model 层的 Seed 文件夹下的 MyContext.cs 中
            //        var configuration = services.GetRequiredService<IConfiguration>();
            //        if (configuration.GetSection("AppSettings")["SeedDBEnabled"].ObjToBool())
            //        {
            //            var myContext = services.GetRequiredService<MyContext>();
            //            DBSeed.SeedAsync(myContext).Wait();
            //        }
            //    }
            //    catch (Exception e)
            //    {
            //        var logger = loggerFactory.CreateLogger<Program>();
            //        logger.LogError(e, "Error occured seeding the Database.");
            //        throw;
            //    }
            //}

            // 运行 web 应用程序并阻止调用线程, 直到主机关闭。
            // 创建完 WebHost 之后，便调用它的 Run 方法，而 Run 方法会去调用 WebHost 的 StartAsync 方法
            // 将Initialize方法创建的Application管道传入以供处理消息
            // 执行HostedServiceExecutor.StartAsync方法
            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                //指定要由 web 主机使用的启动类型。相当于注册了一个IStartup服务。可以自定义启动服务，比如.UseStartup(typeof(StartupDevelopment).GetTypeInfo().Assembly.FullName)
                .UseUrls("http://localhost:8081")
                .UseStartup<Startup>();
    }
}

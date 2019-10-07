using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Extras.DynamicProxy;
using Blog.Core.AOP;
using Blog.Core.AuthHelper.OverWrite;
using Blog.Core.Common.MemoryCache;
using Blog.Core.IServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;

namespace Blog.Core
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<ICaching, MemoryCaching>();

            // log日志注入
            // services.AddSingleton<ILoggerHelper, LogHelper>();

            services.AddMvc()
                .AddMvcOptions(o => o.EnableEndpointRouting = false)
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            #region 初始化DB
            services.AddScoped<Blog.Core.Model.DBSeed>();
            services.AddScoped<Blog.Core.Model.MyContext>();
            #endregion

            #region Swagger
            var basePath = Microsoft.DotNet.PlatformAbstractions.ApplicationEnvironment.ApplicationBasePath;
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v0.1.0",
                    Title = "Blog.Core API",
                    Description = "框架说明文档",
                    TermsOfService = "None",
                    Contact = new Swashbuckle.AspNetCore.Swagger.Contact
                    {
                        Name = "Blog.Core",
                        Email = "Blog.Core@xxx.com",
                        Url = "https://www.jianshu.com/u/c965a074d484"
                    }
                });
                
                var xmlPath = Path.Combine(basePath, "Blog.Core.xml");
                c.IncludeXmlComments(xmlPath, true);
                var xmlModelPath = Path.Combine(basePath, "Blog.Core.Model.xml");//这个就是Model层的xml文件名
                c.IncludeXmlComments(xmlModelPath);

                #region Token绑定到ConfigureServices
                var security = new Dictionary<string, IEnumerable<string>> { { "Blog.Core", new string[] { } }, };
                c.AddSecurityRequirement(security);
                c.AddSecurityDefinition("Blog.Core", new ApiKeyScheme
                {
                    Description = "JWT授权(数据将在请求头中进行传输) 直接在下框中输入Bearer {token}（注意两者之间是一个空格）\"",
                    Name = "Authorization", //jwt默认存放Authorization信息的位置(请求头中)
                    In = "header",  //jwt默认存放Authorization信息的位置(请求头中)
                    Type = "apiKey"
                });
                #endregion

            });
            #endregion

            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("Client", policy => policy.RequireRole("Client").Build());
            //    options.AddPolicy("Admin", policy => policy.RequireRole("Admin").Build());
            //    options.AddPolicy("SystemOrAdmin", policy => policy.RequireRole("Admin", "System"));
            //});

            var audienceConfig = Configuration.GetSection("Audience");
            var symmetricKeyAsBase64 = audienceConfig["Secret"];
            var keyByteArray = Encoding.ASCII.GetBytes(symmetricKeyAsBase64);
            var signingKey = new SymmetricSecurityKey(keyByteArray);

            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            // 令牌验证参数
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,
                ValidateIssuer = true,
                ValidIssuer = audienceConfig["Issuer"],
                ValidateAudience = true,
                ValidAudience = audienceConfig["Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                RequireExpirationTime = true,
            };

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                o.TokenValidationParameters = tokenValidationParameters;
            });

            // Core自带依赖注入
            //services.AddScoped<ISysUserInfoServices, SysUserInfoServices>();

            #region AutoFac 容器
            // 实例化 AutoFac 容器
            var builder = new ContainerBuilder();
            // 注册要通过反射创建的组件
            // 单个服务注册
            //builder.RegisterType<SysUserInfoServices>().As<ISysUserInfoServices>();

            // 注册拦截器
            builder.RegisterType<BlogLogAOP>();//可以直接替换其他拦截器！一定要把拦截器进行注册
            builder.RegisterType<BlogCacheAOP>();

            // 程序集注册
            // 注入实现类层而不是接口层
            //var assemblysServices = Assembly.Load("Blog.Core.Services");
            //var basePath = Microsoft.DotNet.PlatformAbstractions.ApplicationEnvironment.ApplicationBasePath;

            var servicesDllFile = Path.Combine(basePath, "Blog.Core.Services.dll");
            var assemblysServices = Assembly.LoadFrom(servicesDllFile);//直接采用加载文件的方法  ※※★※※ 如果你是第一次下载项目，请先F6编译，然后再F5执行，※※★※※


            // 指定以扫描程序集中的类型注册为提供所有其实现的接口
            builder.RegisterAssemblyTypes(assemblysServices)
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope()
                .EnableInterfaceInterceptors() //对目标类型启用接口拦截.拦截器将被确定,通过在类或接口上截取属性,或添加InterceptedBy()
                .InterceptedBy(typeof(BlogLogAOP)); //将拦截器添加到要注入容器的接口或者类上

            //var assemblysRepository = Assembly.Load("Blog.Core.Repository");
            //builder.RegisterAssemblyTypes(assemblysRepository).AsImplementedInterfaces();

            #region Repository.dll 注入，有对应接口
            var repositoryDllFile = Path.Combine(basePath, "Blog.Core.Repository.dll");
            var assemblysRepository = Assembly.LoadFrom(repositoryDllFile);
            builder.RegisterAssemblyTypes(assemblysRepository).AsImplementedInterfaces();
            #endregion

            // 将services填充到Autofac容器生成器中
            builder.Populate(services);

            // 使用已进行的组件登记创建新容器
            var ApplicationContainer = builder.Build();

            #endregion

            // 第三方IOC接管Core内置DI容器
            return new AutofacServiceProvider(ApplicationContainer);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        [Obsolete]
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            #region Swagger
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ApiHelp V1");
            });
            #endregion

            #region 开启认证中间件

            // 自定义认证中间件
            // app.UseJwtTokenAuth();

            // 官方认证中间件
            app.UseAuthentication();

            #endregion

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
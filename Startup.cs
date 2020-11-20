using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using StackExchange.Redis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Caching.Distributed;
using WebApplicationTest.DAL;
using WebApplicationTest.Extension;
using WebApplicationTest.Middleware;
using StackExchange.Redis.Extensions.Core;
using StackExchange.Redis.Extensions.Newtonsoft;
using StackExchange.Redis.Extensions.Core.Configuration;

namespace WebApplicationTest
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Controller 中的 属性注入
            services.Replace(ServiceDescriptor.Transient<IControllerActivator, ServiceBasedControllerActivator>());
            //数据库上下文注入
            services.AddDbContext<TestDbContext>(option => option.UseNpgsql(Configuration.GetConnectionString("Default")), ServiceLifetime.Transient);

            //两种redis 注入方式，第二种方法丰富
            //1.注入 IDistributedCache
            services.AddStackExchangeRedisCache(options => options.Configuration = Configuration.GetValue<String>("Redis"));
            //2.注入 IRedisCacheClient 和 IRedisDatabase
            services.AddStackExchangeRedisExtensions<NewtonsoftSerializer>(Configuration.GetSection("Redis_2").Get<RedisConfiguration>());

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebApplicationTest", Version = "v1" });
            });
            
        }
        public void ConfigureContainer(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterAssemblyTypes(AppDomain.CurrentDomain.GetAssemblies())
                         .Where(t => t.GetCustomAttribute<AuthorizedAttribute>() != null)
                         .InstancePerDependency()
                         .PropertiesAutowired();
            //Controller 中的 属性注入
            containerBuilder.RegisterTypes(
                typeof(Startup).Assembly.GetExportedTypes()
                .Where(type => typeof(ControllerBase).IsAssignableFrom(type)).ToArray()
                ).PropertiesAutowired();
            containerBuilder.RegisterModule<MiddlewareModule>();
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        { 
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApplicationTest v1"));
            }
            //.net5中间件加了接口IMiddleware,实例直接从IServiceProvider中获取，
            //所以中间件的属性也注入进去了
            app.UseMiddleware<ExceptionHandlerMiddleware>();
  
            app.UseRouting();  

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

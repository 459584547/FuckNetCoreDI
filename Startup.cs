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
            //Controller �е� ����ע��
            services.Replace(ServiceDescriptor.Transient<IControllerActivator, ServiceBasedControllerActivator>());
            //���ݿ�������ע��
            services.AddDbContext<TestDbContext>(option => option.UseNpgsql(Configuration.GetConnectionString("Default")), ServiceLifetime.Transient);

            //����redis ע�뷽ʽ���ڶ��ַ����ḻ
            //1.ע�� IDistributedCache
            services.AddStackExchangeRedisCache(options => options.Configuration = Configuration.GetValue<String>("Redis"));
            //2.ע�� IRedisCacheClient �� IRedisDatabase
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
            //Controller �е� ����ע��
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
            //.net5�м�����˽ӿ�IMiddleware,ʵ��ֱ�Ӵ�IServiceProvider�л�ȡ��
            //�����м��������Ҳע���ȥ��
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

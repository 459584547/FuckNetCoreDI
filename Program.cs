using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WebApplicationTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureLogging((context, loggingbuilder) =>
                {
                    loggingbuilder.AddFilter("System", LogLevel.Warning); //���˵�ϵͳĬ�ϵ�һЩ��־
                    loggingbuilder.AddFilter("Microsoft", LogLevel.Warning);//���˵�ϵͳĬ�ϵ�һЩ��־
                    //���Log4Net
                    //var path = Directory.GetCurrentDirectory() + "\\log4net.config"; 
                    //������������ʾlog4net.config�������ļ�����Ӧ�ó����Ŀ¼�£�Ҳ����ָ�������ļ���·��
                    loggingbuilder.AddLog4Net();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}

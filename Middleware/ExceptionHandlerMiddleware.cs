using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using log4net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WebApplicationTest.Extension;
using WebApplicationTest.Services;

namespace WebApplicationTest.Middleware
{
    [Authorized]
    public class ExceptionHandlerMiddleware: IMiddleware
    {
        public ILog _logger { get; set; }
        public TestService2 testService2 { get; set; }
        
        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = context.Response;
            response.ContentType = "application/json";
            response.StatusCode = (int)HttpStatusCode.InternalServerError;
            var result = JsonConvert.SerializeObject(new
            {
                // customize as you need
                error = new
                {
                    message = exception.Message,
                    exception = exception.GetType().Name
                }
            });
            await response.WriteAsync(result);
            //serilog
            _logger.Error("ERROR FOUND"+result);
        }

        public Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                _logger.Debug($"开始处理接口！！！--参数{context.Request.QueryString}");
                Task task = default;
                Stream originalBody = context.Response.Body;
                try
                {
                    using (var ms = new MemoryStream())
                    {
                        context.Response.Body = ms;
                        var fWatch = new Stopwatch();
                        fWatch.Start();
                        task = next.Invoke(context);
                        fWatch.Stop();
                        ms.Position = 0;
                        string responseBody = new StreamReader(ms).ReadToEnd();
                        _logger.Debug($"接口处理完成,用时[{fWatch.ElapsedMilliseconds}ms]！！！--结果{responseBody}");
                        ms.Position = 0;
                        ms.CopyToAsync(originalBody);
                    }
                }
                finally
                {
                    context.Response.Body = originalBody;
                }
                return task;
            }
            catch (Exception ex)
            {
                return HandleExceptionAsync(context, ex);
            }
        }
    }
}

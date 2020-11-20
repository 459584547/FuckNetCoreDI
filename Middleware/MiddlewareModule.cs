using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using Autofac.Core.Resolving.Pipeline;
using WebApplicationTest.Extension;

namespace WebApplicationTest.Middleware
{
    [Authorized]
    public class MiddlewareModule : Module
    {
        //public  Log4NetMiddleware middleware { get; set; }

        //public MiddlewareModule(IResolveMiddleware middleware)
        //{
        //    this.middleware = middleware;
        //}

        protected override void AttachToComponentRegistration(IComponentRegistryBuilder componentRegistryBuilder, IComponentRegistration registration)
        {
            // Attach to the registration's pipeline build.
            registration.PipelineBuilding += (sender, pipeline) =>
            {
                // Add our middleware to the pipeline.
                pipeline.Use(new Log4NetMiddleware());
            };
        }
    }
}

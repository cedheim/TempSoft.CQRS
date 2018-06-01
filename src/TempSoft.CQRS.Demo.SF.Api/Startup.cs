using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TempSoft.CQRS.Commands;
using TempSoft.CQRS.Demo.Configuration;
using TempSoft.CQRS.Infrastructure;

namespace TempSoft.CQRS.Demo.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration, FluentBootstrapper bootstrapper)
        {
            Configuration = configuration;
            Bootstrapper = bootstrapper;
        }

        public FluentBootstrapper Bootstrapper { get; }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Add(new ServiceDescriptor(typeof(ICommandRouter), Bootstrapper.Resolve<ICommandRouter>()));
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}

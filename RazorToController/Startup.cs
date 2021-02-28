using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace RazorToController
{
    public class SampleAsyncPageFilter : IAsyncPageFilter
    {
        private readonly IConfiguration _config;

        public SampleAsyncPageFilter(IConfiguration config)
        {
            _config = config;
        }

        public Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
        {
            var key = _config["UserAgentID"];
            context.HttpContext.Request.Headers.TryGetValue("user-agent",
                                                            out StringValues value);

            Trace.Write($"Action {context.ActionDescriptor.DisplayName} ");
            if (context.HandlerMethod.MethodInfo.CustomAttributes.Any(a => a.AttributeType == typeof(HiSecure)))
            {
                context.HttpContext.Response.StatusCode = 401;
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context,
                                                      PageHandlerExecutionDelegate next)
        {
            // Do post work.
            await next.Invoke();
        }
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class HiSecure : Attribute
    {
     
    }

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
            services.AddRazorPages();
            services.AddMvc(options => {
                options.EnableEndpointRouting = false;
                options.Filters.Add(new SampleAsyncPageFilter(Configuration));
            });
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //app.Use(async (context, next) => {
                
            //    Endpoint endpoint = context.GetEndpoint();

            //    //YourFilterAttribute filter = endpoint.Metadata.GetMetadata<YourFilterAttribute>();
                

            //    Trace.WriteLine($"Method: { context.Request.Path} Meta {endpoint?.Metadata} ");
            //    await next.Invoke();
            //});

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapDefaultControllerRoute();
                endpoints.MapControllers();
            });

            app.UseMvcWithDefaultRoute();
        }
    }
}

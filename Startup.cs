using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ChatBotWithWS.Services;
using Microsoft.AspNetCore.Http;

namespace ChatBotWithWS
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }
        public static IServiceProvider Services{get;private set;}

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {


            services.AddSingleton<IChatService, ChatService>();
            services.AddSingleton<ICSXEvalService, CSXEvalService>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            
            // services.AddJsEngineSwitcher(options =>
            //     options.DefaultEngineName = ChakraCoreJsEngine.EngineName
            // )
            //     .AddChakraCore()
            //     .AddMsie(options => {
            //         options.UseEcmaScript5Polyfill = true;
            //         options.UseJson2Library = true;
            //     })
            //     .AddJint();

            // services.AddReact();

                        // Add framework services.
            services.AddMvc();

            Services = services.BuildServiceProvider();
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            
            // app.UseReact(config => {
            //     config.DisableServerSideRendering().AddScript("js/wschat.jsx");
            // });
            
            app.UseStaticFiles();
            
            app.UseWebSockets();

            // app.UseReact(config => {
            //     config.AddScript("~/js/WSChat.jsx");
            // });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MediaGoat.Services;
using MediaGoat.Utility.Lucene;
using System.IO;
using MediaGoat.Utility.Configuration;

namespace MediaGoat
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
            services.AddMvc();

            services.AddSingleton<LuceneIndexerThread>(sp => new LuceneIndexerThread(sp.GetService<ILuceneIndexer>()));
            services.AddTransient<IDocumentMapper>(sp => new AutoPropertyDocumentMapper());
            services.AddTransient<DocumentCollectionPropertyMapper>(sp => new DocumentCollectionPropertyMapper(sp.GetService<IDocumentMapper>()));
            services.AddTransient<IMediaSearchService>(sp => new MediaSearchService(sp.GetService<IConfiguration>(), sp.GetService<DocumentCollectionPropertyMapper>()));
            //services.AddTransient<ILuceneIndexer>(sp => new LuceneJsonIndexer<Song>(sp.GetService<IConfiguration>(), sp.GetService<DocumentCollectionPropertyMapper>()));
            services.AddTransient<ILuceneIndexer>(sp => new LuceneSongFileIndexer(Configuration.GetLuceneMediaPaths(), Configuration.GetLuceneIndexPath(),  sp.GetService<DocumentCollectionPropertyMapper>()));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    HotModuleReplacement = true,
                    ReactHotModuleReplacement = true
                });
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });
            });

            app.ApplicationServices.GetService<LuceneIndexerThread>().Run();
        }
    }
}

// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;

namespace Aspnet
{
  using Microsoft.AspNetCore.Builder;
  using Microsoft.AspNetCore.Hosting;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.AspNetCore.Mvc.DataAnnotations;
  using Microsoft.EntityFrameworkCore;
  using Microsoft.Extensions.Configuration;
  using Microsoft.Extensions.DependencyInjection;
  using Videotheque.Data;

  public class Startup
  {
    public IConfigurationRoot Configuration { get; set; }

    public Startup(IHostingEnvironment env)
    {
      var builder = new ConfigurationBuilder()
        .SetBasePath(env.ContentRootPath)
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddEnvironmentVariables();

      this.Configuration = builder.Build();
    }

    // This method gets called by the runtime. Use this method to add services
    // to the container.
    // For more information on how to configure your application, visit
    // https://go.microsoft.com/fwlink/?LinkID=398940
    // CA1822: "Mark members as static"
    #pragma warning disable CA1822
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddDbContext<AppDbContext>(options =>
          options.UseSqlServer(
            this.Configuration
            .GetConnectionString("DefaultConnection")));

      // Includes support for Razor Pages and controllers.
      services.AddMvc()
        .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

      // // Used in Pages/Customer/Create.cshtml.cs
      // services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

      services.AddSingleton<IValidationAttributeAdapterProvider,
        RequiredAttributeAdapterProvider>();
    }

    // This method gets called by the runtime. Use this method to configure the
    // HTTP request pipeline.
    // CA1822: "Mark members as static"
    #pragma warning disable CA1822
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.UseStatusCodePages();
      // S1075: Refactor your code not to use hardcoded absolute paths or URIs.
      #pragma warning disable S1075
      app.UseStatusCodePagesWithReExecute("/Status{0}");

      app.UseMvc();
      app.UseStaticFiles();

      // app.Run(async (context) =>
      //   {
      //   await context.Response.WriteAsync("Hello World2!")
      //   .ConfigureAwait(false);
      //   });
    }
  }
}

// vim:sw=2:ts=2:et:fileformat=dos

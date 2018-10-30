// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;

namespace Aspnet
{
  using Microsoft.AspNetCore.Builder;
  using Microsoft.AspNetCore.Hosting;
  // using Microsoft.AspNetCore.Http;
  using Microsoft.Extensions.DependencyInjection;

  public class Startup
  {
    // This method gets called by the runtime. Use this method to add services
    // to the container.
    // For more information on how to configure your application, visit
    // https://go.microsoft.com/fwlink/?LinkID=398940
    #pragma warning disable CA1822
    public void ConfigureServices(IServiceCollection services)
    {
      // Includes support for Razor Pages and controllers.
      services.AddMvc();
    }

    // This method gets called by the runtime. Use this method to configure the
    // HTTP request pipeline.
    #pragma warning disable CA1822
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.UseMvc();

      // app.Run(async (context) =>
      //   {
      //   // https://medium.com/bynder-tech/c-why-you-should-use-configureawait-false-in-your-library-code-d7837dce3d7f
      //   await context.Response.WriteAsync("Hello World2!")
      //   .ConfigureAwait(false);
      //   });
    }
  }
}

// vim:sw=2:ts=2:et:fileformat=dos

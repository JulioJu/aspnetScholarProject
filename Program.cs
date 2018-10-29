// using System;
// using System.Collections.Generic;
// using System.IO;
// using System.Linq;
// using System.Threading.Tasks;
// using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.Logging;

namespace Aspnet
{
  using Microsoft.AspNetCore;
  using Microsoft.AspNetCore.Hosting;

  #pragma warning disable S1118
  public sealed class Program
  {
    public static void Main(string[] args)
    {
      CreateWebHostBuilder(args).Build().Run();
    }

    public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
      WebHost.CreateDefaultBuilder(args)
      .UseStartup<Startup>();
  }
}

// vim: sw=2 ts=2 et: set ++fileformat=dos

// Namespace defines a scope for organizing related types and namespaces (same concept as C++)
namespace CSharpTutorial;

// 'using' keyword imports a namespace or defines other 'using' directives. This would be similar to an #include <file>
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// Spin up some threads generating random numbers
// Write some test classes
// Write some test interfaces


class Program
{
  // Since .NET 6 templates use 'top-level statements', where the compiler wraps the top level code in a generated class and entry point.
  // The .csproj file defines "ImplicitUsings", which automatically includes a certain subset of libraries from the .NET SDK.
  // The subset of libraries included depends on the template; a Console app is different to a Webapp.
  // Think of how the Arduino libaries kept the main function hidden away in C++ sources, and how I needed to set an implicit include for specific Arduino projects in VSCode.

  // The C++ equivalent for this is typedef std::function<int>(void) cbFunctionType;
  public delegate int PostHandler();
  static void Main(string[] args)
  {
    Console.WriteLine("Hello, World!");
    if (args.Length > 0)
    {
      Console.WriteLine($"Got {args.Length} args wow");
      foreach (string arg in args)
      {
        Console.WriteLine($"Got arg {arg}");
      }
    }
    JobsData jobsDb = new();
    jobsDb.Init();
    JobsData.Job newJob = new()
    {
      Id = Guid.NewGuid(),
      Site = "Wellington",
      Status = JobsData.JobStatus.Pending,
      UpdatedAt = DateTime.Now
    };
    jobsDb.InsertNewJob(newJob);
    jobsDb.QueryJobs();

    /* Move Web App into separate thread / async call */

    WebApplicationBuilder builder = WebApplication.CreateBuilder();
    // 'Kestrel' is the web server built into ASP.Net core.
    // In prod, Kestrel sits behind a reverse proxy such as nginx
    // builder.WebHost.ConfigureKestrel(options =>
    // {
      // options.ListenLocalhost(6969);
      // options.ListenLocalhost(6070, o => o.UseHttps());
    // });

    // TODO: Learn more about OpenApi
    builder.Services.AddOpenApi();
    WebApplication webApp = builder.Build();

    // IP & Port number can be set in a few ways.
    // The default is currently set in 'appsettings.json'
    // webApp.Urls is a vector, so multiple addresses/ports can be specified (assuming the host has the allocated IP address)
    // webApp.Urls.Add("https://localhost:30034");

    if (webApp.Environment.IsDevelopment())
    {
      // What exactly is this doing?
      var thing = webApp.MapOpenApi();
    }

    // This just redirects HTTP to HTTPS requests
    webApp.UseHttpsRedirection();

    webApp.MapGet("/endpointname", () =>
    {
      return 42;
    }).WithName("GetEndpointName");

    webApp.MapGet("/api/jobs", () =>
    {
      return "Get";
    }).WithName("GetApiJobs");

    webApp.MapPut("/api/jobs", () =>
    {
      return 0;
    }).WithName("PutApiJobs");

    // Can optionally assign a PostHandler delegate to 'handler' and pass it to the Post mapping.
    // PostHandler handler = HandlePostRequest;
    // webApp.MapPost("/api/jobs", handler).WithName("PostApiJobs");

    // Can also just pass a function directly
    webApp.MapPost("/api/jobs", HandlePostRequest).WithName("PostApiJobs");
    webApp.Run();
    // Ctrl+C signal closes an application gracefully (at least in this instance)
    Console.WriteLine("Closing webapp");
    jobsDb.Close();
  }

  private static int HandlePostRequest()
  {
    return 1;
  }
}

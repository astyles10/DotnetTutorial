// Namespace defines a scope for organizing related types and namespaces (same concept as C++)
namespace CSharpTutorial;

// 'using' keyword imports a namespace or defines other 'using' directives. This would be similar to an #include <file>
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
  private static JobsDatabase _jobsDatabase = new();
  private static JobsWebApi _jobsApi = new();

  static void Main(string[] args)
  {
    if (!InitialiseJobsDatabase())
    {
      return;
    }

    ParseCommandLineArgs(ref args);
    _jobsDatabase.GetAllJobStatuses();
    _jobsDatabase.GetJobStatusForSite("Wodonga");
    // Next steps: Extend the Web API and integrate it with the database
    // Create a simple Web UI to interact with the API. There's probably some .NET MVC thing to use
    // Probably extend and modify the database
    _jobsApi.Initialise();
    _jobsApi.Run();

    // Can also just pass a function directly
    // Ctrl+C signal closes an application gracefully (at least in this instance)
    Console.WriteLine("Closing webapp");
    _jobsDatabase.Close();
  }

  private static void ParseCommandLineArgs(ref string[] args)
  {
    foreach (string arg in args)
    {
      if (arg == "--seed-db")
      {
        JobDbSeeder seeder = new(ref _jobsDatabase);
        seeder.Seed();
      }
    }
  }

  private static bool InitialiseJobsDatabase()
  {
    // TODO: At some point after filling functionality, implement try catches properly
    try
    {
      _jobsDatabase.Init();
      return true;
    }
    catch
    {
      return false;
    }
  }
}

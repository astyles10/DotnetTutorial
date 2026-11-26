using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CSharpTutorial;

class JobsWebApi
{
  private WebApplication _webApi = WebApplication.Create();
  // private string HostAddress { get; set; } = "";
  // private UInt16 Port { get; set; } = 0;
  public bool Running { get; set; } = false;
  public bool Initialised { get; private set; } = false;

  public JobsWebApi()
  {
    // Move to a seperate function to allow configuring multiple interfaces.
    // HostAddress = hostAddress;
    // Port = port;
  }

  public void Initialise()
  {
    BuildWebApi();
    ConfigureWebApi();
    SetupApiEndpoints();
    Initialised = true;
  }

  private void BuildWebApi()
  {
    WebApplicationBuilder builder = WebApplication.CreateBuilder();
    builder.Services.AddOpenApi();
    _webApi = builder.Build();
  }

  private void ConfigureWebApi()
  {
    if (_webApi.Environment.IsDevelopment())
    {
      _webApi.MapOpenApi(); // Still need to look at OpenAPI..
    }
    _webApi.UseHttpsRedirection();
  }

  private void SetupApiEndpoints() // How extensible/configurable should this be?
  // Could inherit from a Web API base class instead with a protected ConfigureWebApiEndpoint(struct ApiEndpoint) function
  {
    // What is the '?' actually doing?
    _webApi?.MapGet("/endpointname", () =>
    {
      return 42;
    }).WithName("GetEndpointName");

    _webApi?.MapGet("/api/jobs", () =>
    {
      return "Get";
    }).WithName("GetApiJobs");

    _webApi?.MapPut("/api/jobs", () =>
    {
      return 0;
    }).WithName("PutApiJobs");
  }

  // TODO: Make this async/separate thread with condition variable
  public void Run()
  {
    _webApi.Run();
  }
}
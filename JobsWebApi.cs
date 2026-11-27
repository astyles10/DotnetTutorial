using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CSharpTutorial;

class JobsWebApi
{
  private WebApplication _webApi = WebApplication.Create();
  private readonly JobsDatabase jobsDatabase;
  // private string HostAddress { get; set; } = "";
  // private UInt16 Port { get; set; } = 0;
  public bool Running { get; set; } = false;
  public bool Initialised { get; private set; } = false;

  public JobsWebApi(ref JobsDatabase database)
  {
    jobsDatabase = database;
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

  public record CreateJobRequest(string Site, JobStatus Status);

  private void SetupApiEndpoints() // How extensible/configurable should this be?
  // Could inherit from a Web API base class instead with a protected ConfigureWebApiEndpoint(struct ApiEndpoint) function
  {
    // What is the '?' actually doing?
    // A: Similar syntax to Javascript where the succeeding calls will only be invoked if the object is not null
    _webApi?.MapGet("/api/jobs/sites/{site}", (string site) =>
    {
      Job job = jobsDatabase.GetJobStatusForSite(site);
      return job;
    }).WithName("GetApiJobs");

    // 
    _webApi?.MapPost("/api/jobs", (CreateJobRequest req) =>
    {
      // How get json data?
      Console.WriteLine("Got a POST request wow");
      Job job = new()
      {
        Id = Guid.NewGuid(),
        Site = req.Site,
        Status = req.Status,
        UpdatedAt = DateTime.Now
      };
      bool insertSuccess = jobsDatabase.InsertNewJob(job);
      if (insertSuccess)
      {
        return Results.Created($"/api/jobs/{job.Id}", job); 
      }
      return Results.BadRequest("Failed to create new job.");
    })
    .WithName("PostApiJobs");
  }

  // TODO: Make this async/separate thread with condition variable
  public void Run()
  {
    _webApi.Run();
  }
}
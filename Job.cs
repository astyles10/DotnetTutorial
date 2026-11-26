namespace CSharpTutorial;

public struct Job
{
  public Job()
  {
    Site = "";
  }
  public Guid Id {get; set;}
  public string Site {get; set;} = "";
  public JobStatus Status {get; set;} = JobStatus.Undefined;
  public DateTime UpdatedAt {get; set;}
}
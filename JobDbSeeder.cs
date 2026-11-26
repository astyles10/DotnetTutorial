namespace CSharpTutorial;

class JobDbSeeder
{
  private readonly JobsDatabase _database;
  private readonly string[] _siteLocations = [
    "Wellington",
    "Sydney",
    "Melbourne",
    "Brisbane",
    "Perth",
    "Pilbara",
    "Albury",
    "Wodonga",
    "Hobart",
    "Shanghai"
  ];

  public JobDbSeeder(ref JobsDatabase jobsDb)
  {
    _database = jobsDb;
  }

  public void Seed()
  {
    if (!_database.Running)
    {
      return;
    }
    foreach (var site in _siteLocations)
    {
      Random rng = new();
      Job job = new()
      {
        Id = Guid.NewGuid(),
        Site = site,
        Status = (JobStatus)rng.Next((int)JobStatus.Undefined, (int)JobStatus.Closed),
        UpdatedAt = DateTime.Now
      };

      _database.InsertNewJob(job);
    }
  }
}
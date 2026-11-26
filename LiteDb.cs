using Microsoft.Data.Sqlite;

namespace CSharpTutorial
{
  class JobsDatabase
  {
    private readonly SqliteConnection _database;
    public bool Running { get; set; } = false;

    // Constructors the same as C++ of course. No header files are used, everything is encapsulated in a namespace
    public JobsDatabase()
    {
      _database = new SqliteConnection("Data Source=./Jobs.db");
    }

    public bool Init()
    {
      try
      {
        _database.Open();
        ConstructTables();
        Running = true;
        return true;
      } catch (Exception e)
      {
        Console.WriteLine($"Failed to open database: {e}");
        return false;
      }
    }

    public void Close()
    {
      // Should check if any transactions are pending here
      Running = false;
      _database.Close();
    }

    private void ConstructTables()
    {
      SqliteCommand command = _database.CreateCommand();
      command.CommandText = """
      CREATE TABLE IF NOT EXISTS jobs (
        id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
        jobid UUID NOT NULL,
        site varchar(255) NOT NULL,
        status INTEGER NOT NULL,
        updated TIMESTAMP NOT NULL
      )
      """;
      command.ExecuteNonQuery();
    }

    public bool InsertNewJob(Job job)
    {
      SqliteCommand insertCommand = _database.CreateCommand();
      // The Job ID guid could be called a transaction ID instead..
      // Then the transaction ID can be linked to other items.
      insertCommand.CommandText = """
      INSERT INTO jobs (jobid, site, status, updated)
      VALUES ($uuid, $site, $status, $updated)
      """;
      insertCommand.Parameters.AddWithValue("$uuid", job.Id);
      insertCommand.Parameters.AddWithValue("$site", job.Site);
      insertCommand.Parameters.AddWithValue("$status", job.Status);
      insertCommand.Parameters.AddWithValue("$updated", job.UpdatedAt);
      insertCommand.ExecuteNonQuery();
      return true;
    }

    public bool GetAllJobStatuses()
    {
      // Test function to get number of rows
      SqliteCommand command = _database.CreateCommand();
      command.CommandText = """
      SELECT last_insert_rowid();
      """;
      var numberOfRows = command.ExecuteScalar();
      if (numberOfRows == null)
      {
        return false;
      }

      Console.WriteLine($"number of rows: {(long)numberOfRows}");
      command.CommandText = """
      SELECT jobid FROM jobs
      WHERE id = $id;
      """;
      command.Parameters.AddWithValue("$id", numberOfRows);
      using var reader = command.ExecuteReader();
      Console.WriteLine($"Field count: {reader.FieldCount}");
      while (reader.Read())
      {
        Console.WriteLine($"{reader.GetString(0)}");
      }
      return true;
    }

    public void GetJobStatus(Guid jobGuid)
    {
      SqliteCommand command = _database.CreateCommand();
      command.CommandText = """
      SELECT (site status updated) FROM jobs
      WHERE jobid = $jobid
      """;
      command.Parameters.AddWithValue("$jobid", jobGuid);
      var reader = command.ExecuteReader();
      Console.WriteLine($"Field count: {reader.FieldCount}");
      while (reader.Read())
      {
        var site = reader.GetString(0);
        var status = (JobStatus)reader.GetInt64(1);
        var updatedAt = reader.GetString(2);
        Console.WriteLine($"{site} is status {status}, last updated {updatedAt}");
      }
    }

    public void GetJobStatusForSite(string site)
    {
      SqliteCommand command = _database.CreateCommand();
      command.CommandText = """
      SELECT jobid, status, updated FROM jobs
      WHERE site = $site
      """;
      command.Parameters.AddWithValue("$site", site);
      var reader = command.ExecuteReader();
      Console.WriteLine($"Field count: {reader.FieldCount}");
      while (reader.Read())
      {
        var jobId = reader.GetString(0);
        var status = (JobStatus)reader.GetInt64(1);
        var updatedAt = reader.GetString(2);
        Console.WriteLine($"{site} Id: {jobId} has status '{status}', last updated {updatedAt}");
      }
    }
  }
}

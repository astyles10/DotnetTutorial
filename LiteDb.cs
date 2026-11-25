using System.Data;
using System.Runtime.CompilerServices;
using Microsoft.Data.Sqlite;

namespace CSharpTutorial
{
  class JobsData
  {
    public enum JobStatus
    {
      Undefined,
      Pending,
      Open,
      Running,
      Closed
    }

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

    // private readonly SQLiteConnection database;
    private readonly SqliteConnection database;
    // Constructors the same as C++ of course, but no header file
    public JobsData()
    {
      database = new SqliteConnection("Data Source=./Jobs.db");
    }

    public bool Init()
    {
      try
      {
        database.Open();

        ConstructTables();
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
      database.Close();
    }

    private void ConstructTables()
    {
      SqliteCommand command = database.CreateCommand();
      // 
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
      SqliteCommand insertCommand = database.CreateCommand();
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

    public bool QueryJobs()
    {
      SqliteCommand command = database.CreateCommand();
      command.CommandText = """
      SELECT last_insert_rowid();
      """;
      var numberOfRows = (long)command.ExecuteScalar();
      Console.WriteLine($"number of rows: {numberOfRows}");
      command.CommandText = """
      SELECT * FROM jobs
      WHERE id = $id;
      """;
      command.Parameters.AddWithValue("$id", numberOfRows);
      using var reader = command.ExecuteReader();
      while (reader.Read())
      {
        Console.WriteLine($"{reader.ToString()}");
      }
      return true;
    }
  }
}

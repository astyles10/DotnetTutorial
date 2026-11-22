using System.Data;
using Microsoft.Data.Sqlite;
using Microsoft.VisualBasic;

namespace LiteDb
{
  class JobsData
  {
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

        ConstructTableSchemas();
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

    private void ConstructTableSchemas()
    {
      SqliteCommand command = database.CreateCommand();
      command.CommandText = """
      CREATE TABLE jobs (
        id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT;
        
      )
      """;
    }

    public enum JobStatus
    {
      Undefined,
      Unopened,
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

    public bool InsertNewJob(Job job)
    {
      
    }
  }
}

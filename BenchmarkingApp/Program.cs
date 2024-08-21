using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.InProcess.Emit;
using Microsoft.Data.SqlClient;

[MemoryDiagnoser]
public class Program
{
    public static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run<Program>();
    }

    [Benchmark(Baseline = true)]
    public void InitApp()
    {
        SqlConnectionStringBuilder builder = new()
        {
            DataSource = "127.0.0.1,1433",
            InitialCatalog = "MoviesAndActors",
            TrustServerCertificate = true,
            UserID = "SA",
            Password = Environment.GetEnvironmentVariable("mssql_pass").ToString(),
        };
        string connectionString = builder.ConnectionString;

        ReadMethod(new SqlConnection(connectionString));

    }

    [Benchmark]
    public void InitAppNoStringBuilder()
    {
        string server = "127.0.0.1,1433";
        string database = "MoviesAndActors";
        string username = "SA";
        string pwd = Environment.GetEnvironmentVariable("mssql_pass").ToString();

        ReadMethod(new SqlConnection(
            $"Server = {server};" +
            $"TrustServerCertificate = True;" +
            $"Database = {database};" +
            $"User Id = {username};" +
            $"Password = {pwd};"));

    }

    public static void ReadMethod(SqlConnection c)
    {

        Console.WriteLine("Due to limited functionality only one column will be displayed.\n");
        c.Open();
        string sql = $"SELECT * FROM Actor";
        var cmd = new SqlCommand(sql, c);  // Creating a command object
        var reader = cmd.ExecuteReader();  // Embeds data into ExecuteReader object

        Console.WriteLine($"List of Actors:\n");
        while (reader.Read())  // Checks each instance from each line
        {
            Console.WriteLine(reader.GetString(1));
        }

        reader.Close();
        c.Close();

    }
}
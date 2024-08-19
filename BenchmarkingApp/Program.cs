using Microsoft.Data.SqlClient;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Hello");
        //using SqlConnection conn = InitApp();
        //ReadMethod(conn);
        //using SqlConnection conn1 = InitAppNoStringBuilder();
        //ReadMethod(conn);
    }

    public static SqlConnection InitApp()
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

        // conn.Open();

        return new SqlConnection(connectionString);
    }

    public static SqlConnection InitAppNoStringBuilder()
    {
        string server = "127.0.0.1,1433";
        string database = "MoviesAndActors";
        string username = "SA";
        string pwd = Environment.GetEnvironmentVariable("mssql_pass").ToString();

        return new SqlConnection(
            $"Server = {server};" +
            $"TrustServerCertificate = True;" +
            $"Database = {database};" +
            $"User Id = {username};" +
            $"Password = {pwd};");
    }

    public static string? GetTable()
    {
        string? res = "";
        do
        {
            Console.Clear();
            Console.Write("Please enter which table you would like to query: ");
            res = Console.ReadLine();

        } while (!ValidateTable(res));

        return res;
    }

    public static bool ValidateTable(string table)
    {
        List<string> validTables = ["ACTOR", "MOVIE", "GENRE", "ROLE_IN_MOVIE", "MOVIE_GENRE"];
        if (validTables.Contains(table.Trim().ToUpper()))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static void ReadMethod(SqlConnection c)
    {
        string? table = GetTable();
        Console.Clear();
        Console.WriteLine("Due to limited functionality only one column will be displayed.\n");
        c.Open();
        string? res = "";
        string sql = $"SELECT * FROM {table}";
        var cmd = new SqlCommand(sql, c);  // Creating a command object
        var reader = cmd.ExecuteReader();  // Embeds data into ExecuteReader object

        Console.WriteLine($"List of {table.ToLower()}s:\n");
        while (reader.Read())  // Checks each instance from each line
        {
            Console.WriteLine(reader.GetString(1));
        }

        reader.Close();
        c.Close();
        do
        {
            Console.Write("\nPress 1 to return to the CRUD menu or 2 to exit: ");
            res = Console.ReadLine();
        }
        while (res != "1" && res != "2");
        if (res == "1")
        {
            return;
        }
        else
        {
            Console.WriteLine("\nGoodbye");
            return;
        }
    }
}
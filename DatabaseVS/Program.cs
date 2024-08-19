using Azure;
using Azure.Core;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Globalization;
using System.IO;
using static System.Net.Mime.MediaTypeNames;

public class Program
{
    public static void Main(string[] args)
    {
        Menu();
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

    public static void ExitMenu()
    {
        string? res = "";
        do
        {
            Console.Write("\nPress 1 to return to the CRUD menu or 2 to exit: ");
            res = Console.ReadLine();
        }
        while (res != "1" && res != "2");
        if (res == "1")
        {
            CrudMenu();
        }
        else
        {
            Console.WriteLine("\nGoodbye");
            return;
        }
    }

    public static string ValidateCol(string col)
    {
        string? res = "";
        do
        {
            Console.Write($"Please enter {col}: ");
            res = Console.ReadLine();
        } while (col.Length < 2 && col.Length > 50);

        TextInfo myTI = new CultureInfo("en-gb", false).TextInfo;
        return myTI.ToTitleCase(res);
    }

    public static string ValidateDate(string col)
    {
        string? res = "";
        DateTime dateVal;
        do
        {
            Console.Write($"Please enter {col}: ");
            res = Console.ReadLine();
        } while (!DateTime.TryParse(res, out dateVal));

        return dateVal.ToString();
    }

    public static string ValidateYear(string col)
    {
        string? res = "";
        Int16 yearVal;
        do
        {
            Console.Write($"Please enter {col}: ");
            res = Console.ReadLine();
        } while (!Int16.TryParse(res, out yearVal) || yearVal < 1888);

        return yearVal.ToString();
    }

    public static string ValidateLength(string col)
    {
        string? res = "";
        Int16 runningTime;
        do
        {
            Console.Write($"Please enter {col}: ");
            res = Console.ReadLine();
        } while (!Int16.TryParse(res, out runningTime) || runningTime < 0);

        return runningTime.ToString();
    }

    private static string ValidateRating(string col)
    {
        string? res = "";
        decimal rating;
        do
        {
            Console.Write($"Please enter {col}: ");
            res = Console.ReadLine();
        } while (!Decimal.TryParse(res, out rating) || rating < 0 || rating >= 10);

        return rating.ToString();
    }

    private static string[] GetUpdateInfo(string table)
    {
        string[] actorColumnList = ["First_Name", "Last_Name", "Date_Of_Birth", "Country_Of_Birth"];
        string[] movieColumnList = ["Title", "Description", "Release_Year", "Running_Length_Mins", "IMDb_Rating", "Reviews", "Director"];
        string chosenString = "";
        string? res;
        if (table == "Actor")
        {
            for(int i = 0; i < actorColumnList.Length; i++)
            {
                Console.WriteLine($"Would you like to update the {actorColumnList[i]} column?");
                res = Console.ReadLine();
                if (res.ToLower() == "y")
                {
                    chosenString += actorColumnList[i] + " ";
                }
            }
            

        }
        else if (table == "Movie")
        {
            for (int i = 0; i < movieColumnList.Length; i++)
            {
                Console.WriteLine($"Would you like to update the {movieColumnList[i]} column?");
                res = Console.ReadLine();
                if (res.ToLower() == "y")
                {
                    chosenString += movieColumnList[i] + " ";
                }
            }

        }

        else
        {
            Console.WriteLine("Only the Actor and Movie tables are supported for dynamic SQL queries at the moment.");
        }

        return chosenString.Split(new string[] { "\n" },StringSplitOptions.RemoveEmptyEntries);
    }

    public static string GetDeleteInfo(string table)
    {
        string colName;
        if (table == "Actor")
        {
            colName = "First_Name";
        }
        else
        {
            colName = "Title";
        }
        string? res = "";
        do
        {
            Console.Write($"Please enter the {colName} of the record you would like to update/remove: ");
            res = Console.ReadLine();
        } while (res.Length < 2 && res.Length > 250);
        return res;
    }

    public static string[] GetActorInfo()
    {
        string[] info = new string[4];
        string[] columns = ["First_Name", "Last_Name", "Date_Of_Birth", "Country_Of_Birth"];
        for (int i = 0; i < 4; i++)
        {
            if (columns[i] == "Date_Of_Birth")
            {
                info[i] = ValidateDate(columns[i]);
            }
            else
            {
                info[i] = ValidateCol(columns[i]);
            }
        }

        return info;
    }

    private static string[] GetMovieInfo()
    {
        string[] info = new string[7];
        string[] columns = ["Title", "Description", "Release_Year", "Running_Length_Mins", "IMDb_Rating", "Reviews", "Director"];
        for (int i = 0; i < 7; i++)
        {
            if (columns[i] == "Release_Year")
            {
                info[i] = ValidateYear(columns[i]);
            }
            else if (columns[i] == "Running_Length_Mins")
            {
                info[i] = ValidateLength(columns[i]);
            }
            else if (columns[i] == "IMDb_Rating")
            {
                info[i] = ValidateRating(columns[i]);
            }
            else
            {
                info[i] = ValidateCol(columns[i]);
            }
        }

        return info;
    }

    public static void CreateMethod(SqlConnection c)
    {
        string? table = GetTable();
        Console.Clear();
        // Console.WriteLine("Due to limited functionality only one column will be displayed.\n");
        string? res = "";
        string sql = "";
        string[] values;
        var cmd = new SqlCommand();
        if (table == "Actor")
        {
            sql = $"INSERT INTO {table} (First_Name, Last_Name, Date_Of_Birth, Country_Of_Birth) VALUES (@First_Name, @Last_Name, @Date_Of_Birth, @Country_Of_Birth)";
            values = GetActorInfo();
            cmd = new SqlCommand(sql, c);  // Creating a command object
            cmd.Parameters.AddWithValue("@First_Name", values[0]);
            cmd.Parameters.AddWithValue("@Last_Name", values[1]);
            cmd.Parameters.AddWithValue("@Date_Of_Birth", values[2]);
            cmd.Parameters.AddWithValue("@Country_Of_Birth", values[3]);
        }
        else if (table == "Movie")
        {
            sql = $"INSERT INTO {table} (Title, Description, Release_Year, Running_Length_Mins, IMDb_Rating, Reviews, Director) VALUES (@Title, @Description, @Release_Year, @Running_Length_Mins, @IMDb_Rating, @Reviews, @Director)";
            values = GetMovieInfo();
            cmd = new SqlCommand(sql, c);  // Creating a command object
            cmd.Parameters.AddWithValue("@Title", values[0]);
            cmd.Parameters.AddWithValue("@Description", values[1]);
            cmd.Parameters.AddWithValue("@Release_Year", values[2]);
            cmd.Parameters.AddWithValue("@Running_Length_Mins", values[3]);
            cmd.Parameters.AddWithValue("@IMDb_Rating", values[4]);
            cmd.Parameters.AddWithValue("@Reviews", values[5]);
            cmd.Parameters.AddWithValue("@Director", values[6]);
        }
        else
        {
            Console.WriteLine("Only the Actor and Movie tables are supported at this point in time.");
            Thread.Sleep(2000);
            CrudMenu();
        }

        c.Open();
        var reader = cmd.ExecuteNonQuery();  // Executes INSERT/UPDATE/DELETE command

        Console.WriteLine($"Added new record to {table.ToLower()}.\n");

        c.Close();
        do
        {
            Console.Write("\nPress 1 to return to the CRUD menu or 2 to exit: ");
            res = Console.ReadLine();
        }
        while (res != "1" && res != "2");
        if (res == "1")
        {
            CrudMenu();
        }
        else
        {
            Console.WriteLine("\nGoodbye");
            return;
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
            CrudMenu();
        }
        else
        {
            Console.WriteLine("\nGoodbye");
            return;
        }
    }

    public static void UpdateMethod(SqlConnection c)
    {
        string? table = GetTable();
        Console.Clear();
        Console.WriteLine("Due to limited functionality only one column can be used to update a record.\n");
        string? res;
        string sql;
        string getName = GetDeleteInfo(table);
        string[] values = GetUpdateInfo(table);
        Console.WriteLine(values.Length);
        foreach (var v in values)
        {
            Console.WriteLine(v);
        }
        var cmd = new SqlCommand();
        //if (table == "Actor")
        //{
        //    sql = $"UPDATE {table} SET {values} WHERE First_Name = (@First_Name)";
        //    cmd = new SqlCommand(sql, c);  // Creating a command object

        //    cmd.Parameters.AddWithValue("@First_Name", getName);
        //}
        //else if (table == "Movie")
        //{
        //    sql = $"UPDATE {table} SET WHERE Title = (@Title)";
        //    cmd = new SqlCommand(sql, c);  // Creating a command object

        //    cmd.Parameters.AddWithValue("@Title", getName);
        //}
        //else
        //{
        //    Console.WriteLine("Only the Actor and Movie tables are supported at this point in time.");
        //    Thread.Sleep(2000);
        //    CrudMenu();
        //}

        //c.Open();
        //var reader = cmd.ExecuteNonQuery();  // Executes INSERT/UPDATE/DELETE command

        //if (reader == 1)
        //{
        //    Console.WriteLine($"Updated record in {table.ToLower()}.\n");
        //}
        //else
        //{
        //    Console.WriteLine($"No record found, {table.ToLower()} unchanged.\n");
        //}

        //c.Close();
        do
        {
            Console.Write("\nPress 1 to return to the CRUD menu or 2 to exit: ");
            res = Console.ReadLine();
        }
        while (res != "1" && res != "2");
        if (res == "1")
        {
            CrudMenu();
        }
        else
        {
            Console.WriteLine("\nGoodbye");
            return;
        }
    }

    public static void DeleteMethod(SqlConnection c)
    {
        string? table = GetTable();
        Console.Clear();
        Console.WriteLine("Due to limited functionality only one column can be used to delete a record.\n");
        string? res = "";
        string sql = "";
        string getName = GetDeleteInfo(table);
        var cmd = new SqlCommand();
        if (table == "Actor")
        {
            sql = $"DELETE FROM {table} WHERE First_Name = (@First_Name)";
            cmd = new SqlCommand(sql, c);  // Creating a command object

            cmd.Parameters.AddWithValue("@First_Name", getName);
        }
        else if (table == "Movie")
        {
            sql = $"DELETE FROM {table} WHERE Title = (@Title)";
            cmd = new SqlCommand(sql, c);  // Creating a command object

            cmd.Parameters.AddWithValue("@Title", getName);
        }
        else
        {
            Console.WriteLine("Only the Actor and Movie tables are supported at this point in time.");
            Thread.Sleep(2000);
            CrudMenu();
        }

        c.Open();
        var reader = cmd.ExecuteNonQuery();  // Executes INSERT/UPDATE/DELETE command

        if (reader == 1)
        {
            Console.WriteLine($"Deleted record in {table.ToLower()}.\n");
        }
        else
        {
            Console.WriteLine($"No record found, {table.ToLower()} unchanged.\n");
        }

        c.Close();
        do
        {
            Console.Write("\nPress 1 to return to the CRUD menu or 2 to exit: ");
            res = Console.ReadLine();
        }
        while (res != "1" && res != "2");
        if (res == "1")
        {
            CrudMenu();
        }
        else
        {
            Console.WriteLine("\nGoodbye");
            return;
        }
    }

    public static void Menu()
    {
        string? response = "";
        do
        {
            Console.Clear();
            Console.WriteLine("""
        * * * * * * * * * *
        *     M e n u     *
        * * * * * * * * * *

        Welcome to this CRUD program, please choose one of the numbers below to proceed:

        1. Carry out CRUD operation.

        2. View list of tables in current database.
            
        3. Exit program.

        """);

            response = Console.ReadLine();
        } while ((response != "1") && (response != "2") && (response != "3"));

        if (response == "1")
        {
            Console.WriteLine("Doing Crud stuff");
            CrudMenu();
        }
        else if (response == "2")
        {
            Console.WriteLine("Show list of tables");
            using SqlConnection conn = InitApp();
            ShowTables(conn);


        }
        else
        {
            Console.WriteLine("\nGoodbye");
            return;
        }
    }

    public static void CrudMenu()
    {
        string? response = "";
        do
        {
            Console.Clear();
            Console.WriteLine("""
        * * * * * * * * * *
        *     C R U D     *
        * * * * * * * * * *

        Once again, please choose one of the numbers below to proceed:

        1. Create new record.

        2. Read records.

        3. Update existing record.

        4. Delete record.

        5. Return to main menu.
            
        6. Exit program.

        """);

            response = Console.ReadLine();
        } while ((response != "1") && (response != "2") && (response != "3") && (response != "4") && (response != "5") && (response != "6"));

        switch (response)
        {
            case "1":
                {
                    Console.WriteLine("Creating...");
                    using SqlConnection conn = InitApp();
                    CreateMethod(conn);
                    break;
                }
            case "2":
                {
                    Console.WriteLine("Reading...");
                    using SqlConnection conn = InitApp();
                    ReadMethod(conn);
                    break;
                }
            case "3":
                {
                    Console.WriteLine("Updating...");
                    using SqlConnection conn = InitApp();
                    UpdateMethod(conn);
                    break;
                }
            case "4":
                {
                    Console.WriteLine("Deleting...");
                    using SqlConnection conn = InitApp();
                    DeleteMethod(conn);
                    break;
                }
            case "5":
                {
                    Menu();
                    break;
                }
            default:
                {
                    Console.WriteLine("\nGoodbye");
                    return;
                }
        }
    }


    public static void ShowTables(SqlConnection c)
    {
        Console.Clear();
        c.Open();
        string? res = "";
        string sql = "SELECT * FROM MoviesAndActors.INFORMATION_SCHEMA.TABLES";
        var cmd = new SqlCommand(sql, c);  // Creating a command object
        var reader = cmd.ExecuteReader();  // Embeds data into ExecuteReader object

        Console.WriteLine("List of tables:\n");
        while (reader.Read())  // Checks each instance from each line
        {
            Console.WriteLine($"-{reader.GetString(2)}");
        }

        reader.Close();
        c.Close();
        do
        {
            Console.Write("\nPress 1 to return to the main menu or 2 to exit: ");
            res = Console.ReadLine();
        }
        while (res != "1" && res != "2");
        if (res == "1")
        {
            Menu();
        }
        else
        {
            Console.WriteLine("\nGoodbye");
            return;
        }
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
}
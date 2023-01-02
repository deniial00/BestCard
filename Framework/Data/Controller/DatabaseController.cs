using System.Text.Json;
using Framework.Data.Models;
using Npgsql;

namespace Framework.Data.Controller;

public class DatabaseController
{
    private readonly string DatabseUser = "docker";
    private readonly string DatabasePassword = "docker";
    private readonly string DatabaseHost = "localhost";
    private readonly string DatabaseDatabase = "BestCard";
    private readonly string DatabasePort = "5432";


    private static DatabaseController? Instance;
    private readonly NpgsqlConnection Connection;

    private DatabaseController()
    {
        Console.Write("Acessing database ... ");
        var connString = $"Host={DatabaseHost};Port={DatabasePort};Username={DatabseUser};Password={DatabasePassword};Database={DatabaseDatabase}";
        try
        {
            Connection = new NpgsqlConnection(connString);
            Connection.Open();
            if (Connection == null || Connection.State == System.Data.ConnectionState.Closed)
                throw new NpgsqlException("Error could not create connection");
        }
        catch(Exception ex)
        {
            Console.WriteLine($"Acces to database failed with error: {ex.Message}");
        }

        

        Console.Write("OK!\n");
    }

    public static DatabaseController GetInstance()
    {
        if (Instance == null)
            Instance = new DatabaseController();

        return Instance;
    }

    public NpgsqlConnection GetConnection()
    {
        return Connection;
    }

    public (int, NpgsqlTransaction) ExecuteNonReadQuery(string query, List<NpgsqlParameter> parameterList)
    {
        var tran = Connection.BeginTransaction();
        int affectedRows = 0;
        try
        {
            var command = new NpgsqlCommand(query, Connection, tran);
            
            command.Parameters.AddRange(parameterList.ToArray());

            affectedRows = command.ExecuteNonQuery();

            //tran.Commit();
        }
        catch (Exception ex)
        {
            Console.Write($"Error at Executing query:\n{query}");
            Console.WriteLine($"ERROR MESSAGE: {ex.Message}");
            Console.WriteLine("Paramter:");

            foreach(var param in parameterList)
            {
                Console.WriteLine($"{param.ParameterName}: {param.Value}");
            }
            //tran.Rollback();
            throw new NpgsqlException("Query failed", ex);
        }

        return (affectedRows, tran);
    }

    public int LogMessage()
    {
        return 0;
    }
}

using System.Text.Json;
using Framework.Data.Models;
using Framework.Net.Controller;
using Framework.Net.Models;
using Newtonsoft.Json;
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

        Connection = new NpgsqlConnection(connString);
        Connection.Open();

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

    public bool CheckConnection()
    {
        if (Connection == null || Connection.State == System.Data.ConnectionState.Closed)
            return false;
        else
            return true;
    }

    public (int, NpgsqlTransaction) ExecuteNonReadQuery(string query, List<NpgsqlParameter>? parameterList)
    {
        var tran = Connection.BeginTransaction();
        int affectedRows = 0;
        try
        {
            var command = new NpgsqlCommand(query, Connection, tran);
            if (parameterList is not null)
                command.Parameters.AddRange(parameterList.ToArray());

            affectedRows = command.ExecuteNonQuery();

            //tran.Commit();
        }
        catch (Exception ex)
        {
            Console.Write($"Error at Executing query:\n{query}");
            Console.WriteLine($"ERROR MESSAGE: {ex.Message}");
            Console.WriteLine("Paramter:");
            if (parameterList is not null)
            {
                foreach(var param in parameterList)
                {
                    Console.WriteLine($"{param.ParameterName}: {param.Value}");
                }
            }
            //tran.Rollback();
            throw new NpgsqlException("Query failed", ex);
        }

        return (affectedRows, tran);
    }

    public void ResetDatabase()
    {
        string resetDB =
            @"TRUNCATE TABLE bestcard.users RESTART IDENTITY;
            TRUNCATE TABLE bestcard.cards RESTART IDENTITY;
            TRUNCATE TABLE bestcard.battles RESTART IDENTITY;";
        var resetDBCmd = new NpgsqlCommand(resetDB, Connection);
        resetDBCmd.ExecuteNonQuery();
    }

    public void InitDatabase()
    {
        var kienboec = new UserCredentials("kienboec", "daniel");
        var altenhof = new UserCredentials("altenhof", "markus");
        var admin = new UserCredentials("admin", "istrator");
        UserService.CreateUser(admin, true);
        UserService.CreateUser(kienboec);
        UserService.CreateUser(altenhof);

        List<CardModel> package1 = new()
        {
            new CardModel("845f0dc7-37d0-426e-994e-43fc3ac83c08", "WaterGoblin", 10.0f),
            new CardModel("99f8f8dc-e25e-4a95-aa2c-782823f36e2a", "Dragon", 50.0f),
            new CardModel("e85e3976-7c86-4d06-9a80-641c2019a79f", "WaterSpell", 20.0f),
            new CardModel("1cb6ab86-bdb2-47e5-b6e4-68c5ab389334", "Ork", 45.0f),
            new CardModel("dfdd758f-649c-40f9-ba3a-8657f4b3439f", "FireSpell", 25.0f)
        };

        CardService.AddPackage(package1.ToArray());

        List<CardModel> package2 = new()
        {
            new CardModel("134212c2-fd7a-4600-b313-122b02322fd5", "FireGoblin", 12.0f),
            new CardModel("19f8f8dc-e25e-4a95-aa2c-782823f36ede", "WaterDragon", 35.0f),
            new CardModel("f85e3976-7c86-4d06-9a80-641c2019a79e", "WaterSpell", 20.0f),
            new CardModel("1cb6ab86-bdb2-47e5-b6e4-68c5ab38fv56", "FireOrk", 30.0f),
            new CardModel("4fvj758f-da9c-40f9-ba3a-8657f4b3da24", "FireSpell", 25.0f)
        };

        CardService.AddPackage(package2.ToArray());

    }

    public int LogMessage()
    {
        return 0;
    }
}

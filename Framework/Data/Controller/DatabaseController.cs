using System.Text.Json;
using Npgsql;

namespace Framework.Controller;

class DatabaseController
{
    private readonly string _databaseUser = "postgres";
    private readonly string _databasePassword = "postgres";
    private readonly string _databaseHost = "localhost";
    private readonly string _databaseDatabase = "postgres";
    private readonly string _databasePort = "5432";


    private static DatabaseController? _databaseController;
    private NpgsqlConnection _databaseConnection;

    private DatabaseController()
    {
        var connString = $"Host={_databaseHost};Port={_databasePort};Username={_databaseUser};Password={_databasePassword};Database={_databaseDatabase}";

        _databaseConnection = new NpgsqlConnection(connString);
        _databaseConnection.Open();
        //if()
    }

    public static DatabaseController GetInstance()
    {
        if (_databaseController == null)
            _databaseController = new DatabaseController();

        return _databaseController;
    }

    public NpgsqlDataReader? Query(string query)
    {
        using var cmd = new NpgsqlCommand(query, _databaseConnection);

        var reader = cmd.ExecuteReader();

        return reader;
    }
}

using System.Security.Cryptography;
using System.Text;
using Npgsql;

using Framework.Net.Models;

namespace Framework.Data.Controller;

public static class UserService
{

    public static int CreateUser(UserCredentials credentials)
    {
        var db = DatabaseController.GetInstance();
        var connection = db.GetConnection();
        var paramList = new List<NpgsqlParameter>();
        var tran = connection.BeginTransaction();

        // check if user already Exists
        bool userExists = UserService.CheckUserExists(credentials.Username);

        if (userExists)
            throw new ArgumentException("User already exists");

        string createUserQuery =
            @"INSERT INTO bestcard.users
            (
                ""username"",
                ""password_hash"",
                ""created_at""
            )
            VALUES
            (
                @Username,
                @Password,
                NOW()
            )
            RETURNING id;";

        paramList.Add(new NpgsqlParameter("@Password", credentials.Password));
        paramList.Add(new NpgsqlParameter("@Username", credentials.Username));
        paramList.Add(new NpgsqlParameter("@Username", credentials.Username));

        var createUserCmd = new NpgsqlCommand(createUserQuery,connection, tran);
        createUserCmd.Parameters.AddRange(paramList.ToArray());

        var userID = createUserCmd.ExecuteScalar();

        if (userID is null || (int) userID < 1)
        {
            tran.Rollback();
            throw new Exception("Query failed");
        }

        return (int) userID;
    }

    public static bool CheckUserExists(string username)
    {
        var connection = DatabaseController.GetInstance().GetConnection();

        string checkUserNameExistsQuery = "SELECT COUNT(*) as cnt FROM bestcard.users WHERE username = @Username;";
        var checkUserCmd = new NpgsqlCommand(checkUserNameExistsQuery, connection);
        checkUserCmd.Parameters.Add(new NpgsqlParameter("@Username", username));

        var count = checkUserCmd.ExecuteScalar();

        if (count is not null && (Int64) count > 0)
            return true;
        return false;
    }

    // TODO: also check if admin user
    public static bool AuthenticateUser(UserCredentials cred, string token = "")
    {
        // if admin user
        if (token == "Basic admin-mtcgToken")
            return true;

        var connection = DatabaseController.GetInstance().GetConnection();

        string getHashedPasswordQuery = "SELECT password_hash FROM bestcard.users WHERE username = @Username;";
        var getHashedPasswordCmd = new NpgsqlCommand(getHashedPasswordQuery, connection);
        getHashedPasswordCmd.Parameters.Add(new NpgsqlParameter("@Username", cred.Username));

        var password = getHashedPasswordCmd.ExecuteScalar();
        
        if (password is not null)
        {
            string pass = (string) password;

            if(pass.Trim() == cred.Password)
                return true;
        }

        return false;
    }

    public static string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            // Hash the password and return the hash as a hexadecimal string
            var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            string returnVal = BitConverter.ToString(hash).Replace("-", string.Empty).ToLowerInvariant();
            return returnVal;
        }
    }

    public static string GenerateToken64()
    {
        const string src = "abcdefghijklmnopqrstuvwxyz0123456789";
        int length = 64;
        var sb = new StringBuilder();
        Random rng = new Random();
        for (var i = 0; i < length; i++)
        {
            var c = src[rng.Next(0, src.Length)];
            sb.Append(c);
        }
        return sb.ToString();
    }
}


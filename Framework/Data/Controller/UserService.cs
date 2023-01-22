using System.Security.Cryptography;
using System.Text;
using Npgsql;

using Framework.Net.Models;
using Framework.Data.Models;

namespace Framework.Data.Controller;

public static class UserService
{

    public static int CreateUser(UserCredentials credentials, bool isAdmin = false)
    {
        var db = DatabaseController.GetInstance();
        var connection = db.GetConnection();
        var paramList = new List<NpgsqlParameter>();

        // check if user already Exists
        bool userExists = CheckUserExists(credentials.Username);

        if (userExists)
            throw new ArgumentException("User already exists");

        var tran = connection.BeginTransaction();

        string createUserQuery =
            @"INSERT INTO bestcard.users
            (
                ""username"",
                ""password_hash"",
                ""created_at"",
                ""is_admin""
            )
            VALUES
            (
                @Username,
                @Password,
                NOW(),
                @isAdmin
            )
            RETURNING user_id;";

        paramList.Add(new NpgsqlParameter("@Password", credentials.Password));
        paramList.Add(new NpgsqlParameter("@Username", credentials.Username));
        paramList.Add(new NpgsqlParameter("@isAdmin", isAdmin));

        var createUserCmd = new NpgsqlCommand(createUserQuery,connection, tran);
        createUserCmd.Parameters.AddRange(paramList.ToArray());

        var userID = createUserCmd.ExecuteScalar();

        if (userID is null || (int) userID < 1)
        {
            tran.Rollback();
            throw new Exception("Query failed");
        }

        tran.Commit();

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

    // returns userId
    public static int AuthenticateUser(UserCredentials cred, string token = "")
    {
        // if admin user
        if (token == "Basic admin-mtcgToken")
            return 1;

        var connection = DatabaseController.GetInstance().GetConnection();

        string getHashedPasswordQuery = "SELECT password_hash, user_id FROM bestcard.users WHERE username = @username;";
        var getHashedPasswordCmd = new NpgsqlCommand(getHashedPasswordQuery, connection);
        getHashedPasswordCmd.Parameters.Add(new NpgsqlParameter("@username", cred.Username));

        object password, userId;
        using (var reader = getHashedPasswordCmd.ExecuteReader())
        {
            reader.Read();
            password = reader.GetString(0);
            userId = reader.GetInt32(1);
        }
        
        if (password is not null)
        {
            string pass = (string) password;

            if(pass.Trim() == cred.Password)
                return (int) userId;
        }

        return 0;
    }
    public static UserModel? GetUser(int? userId, string? username = null)
    {
        var conn = DatabaseController.GetInstance().GetConnection();

        string getUserQuery = "SELECT * FROM bestcard.users WHERE";
        var param = new NpgsqlParameter();

        if (username is not null)
        {
            getUserQuery += " username = @username";
            param.ParameterName = "@username";
            param.Value = username;
        } else
        {
            getUserQuery += " user_id = @userid";
            param.ParameterName = "@userid";
            param.Value = userId;
        }

        var getUserCmd = new NpgsqlCommand(getUserQuery, conn);
        getUserCmd.Parameters.Add(param);
        var user = new UserModel();

        using (var reader = getUserCmd.ExecuteReader())
        {
            reader.Read();

            if (!reader.HasRows)
                return user;

            user.UserId = reader.GetInt32(reader.GetOrdinal("user_id"));
            user.Username = reader.GetString(reader.GetOrdinal("username"));
            user.IsAdmin = reader.GetBoolean(reader.GetOrdinal("is_admin"));
            user.Credits = reader.GetInt32(reader.GetOrdinal("credits"));
        }

        return user;
    }

    public static int UpdateUserCredits(int userId, int updateAmount)
    {
        var conn = DatabaseController.GetInstance().GetConnection();

        var tran = conn.BeginTransaction();
        string updateCreditsQuery = "UPDATE bestcard.users SET credits = credits + @credits WHERE user_id = @userid";
        var updateCreditsCmd = new NpgsqlCommand(updateCreditsQuery, conn, tran);
        updateCreditsCmd.Parameters.Add(new NpgsqlParameter("@credits", updateAmount));
        updateCreditsCmd.Parameters.Add(new NpgsqlParameter("@userid", userId));

        int res = updateCreditsCmd.ExecuteNonQuery();

        if (res != 1)
        {
            tran.Rollback();
            return -1;
        }

        tran.Commit();
        return 1;
    }

    public static string GetUsername(int userId)
    {

        var connection = DatabaseController.GetInstance().GetConnection();

        string getUsernameQuery = "SELECT username FROM Bestcard.users WHERE user_id = @user_id";
        var getUsernameCmd = new NpgsqlCommand(getUsernameQuery, connection);
        getUsernameCmd.Parameters.Add(new NpgsqlParameter("@user_id", userId));

        var res = getUsernameCmd.ExecuteScalar();

        if (res is not null && res is string)
            return (string) res;
        else
            throw new ArgumentException("Could not retrieve username");
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

    public static string GenerateToken(int length)
    {
        const string src = "abcdefghijklmnopqrstuvwxyz0123456789";

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


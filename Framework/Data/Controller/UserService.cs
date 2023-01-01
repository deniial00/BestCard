using System.Security.Cryptography;
using System.Text;
using Framework.Data.Models;
using Npgsql;

namespace Framework.Data.Controller;

public class UserService
{

	private static UserService? Instance;

	private UserService()
	{

	}

	public static UserService GetInstance()
	{
		if (Instance == null)
			Instance = new UserService();

		return Instance;
	}

    public int CreateUser(UserCredentials credentials)
    {
        var db = DatabaseController.GetInstance();
        var paramList = new List<NpgsqlParameter>();

        // check if user already Exists
        bool userExists = CheckUserExists(credentials.Username);

        if (userExists)
            throw new ArgumentException("User already exists");

        string createUserQuery = @"INSERT INTO bestcard.users
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
                       );";

        paramList.Add(new NpgsqlParameter("@Password", credentials.Password));
        paramList.Add(new NpgsqlParameter("@Username", credentials.Username));


        (int rowCount, NpgsqlTransaction tran) = db.ExecuteNonReadQuery(createUserQuery, paramList);

        if (rowCount != 1)
        {
            tran.Rollback();
            throw new Exception("Query failed");
        }

        tran.Commit();

        string selectUserIDQuery = "SELECT id FROM bestcard.users WHERE username = @Username";

        var selectUserIDCmd = new NpgsqlCommand(selectUserIDQuery, db.GetConnection());
        selectUserIDCmd.Parameters.Add(new NpgsqlParameter("@Username", credentials.Username));

        Int32 userID = (Int32) selectUserIDCmd.ExecuteScalar();

        return (int) userID;
    }

    public bool CheckUserExists(string username)
    {
        var db = DatabaseController.GetInstance();

        string checkUserNameExistsQuery = "SELECT COUNT(*) as cnt FROM bestcard.users WHERE username = @Username;";
        var checkUserCmd = new NpgsqlCommand(checkUserNameExistsQuery, db.GetConnection());
        checkUserCmd.Parameters.Add(new NpgsqlParameter("@Username", username));

        var count = checkUserCmd.ExecuteScalar();

        if (count is not null && (Int64) count > 0)
            return true;

        return false;
    }

    public bool AuthenticateUser(UserCredentials cred)
    {
        var db = DatabaseController.GetInstance();

        string getHashedPasswordQuery = "SELECT password_hash FROM bestcard.users WHERE username = @Username;";
        var getHashedPasswordCmd = new NpgsqlCommand(getHashedPasswordQuery, db.GetConnection());
        getHashedPasswordCmd.Parameters.Add(new NpgsqlParameter("@Username", cred.Username));

        var password = (string) getHashedPasswordCmd.ExecuteScalar();

        if (password is not null && password == cred.Password)
            return true;

        return false;
    }

    //public User? GetUser()
    //{

    //}

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


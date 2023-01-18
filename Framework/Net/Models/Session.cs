using Framework.Data.Models;
using Newtonsoft.Json;
using System.Net.Sockets;

namespace Framework.Net.Models;

public class Session
{
    public string Token;

    public int? UserID;

    public string? UserName;

    public bool IsLoggedIn;

    public DateTime LastAction;

    public Session() { }

    public Session(string token)
    {
        Token = token;
        UserID = null;
        IsLoggedIn = false;
        LastAction = DateTime.Now;
    }

    public Session(UserCredentials cred, string token)
    {
        Token = token;
        UserName = cred.Username;
        IsLoggedIn = false;
        LastAction = DateTime.Now;
    }

    // TODO: Rework admin user session
    public static Session AdminUserSession()
    {
        var sess = new Session();
        sess.UserID = 1;
        sess.Token = "Basic admin-mtcgToken";
        sess.IsLoggedIn = true;
        sess.LastAction = DateTime.Now;
        return sess;
    }
}

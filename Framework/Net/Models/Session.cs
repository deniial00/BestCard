using Newtonsoft.Json;
using System.Net.Sockets;

namespace Framework.Networking.Models;

class Session
{
    public string Token;

    public int? UserID;

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

    public static Session AdminUserSession()
    {
        var sess = new Session();
        sess.UserID = 1;
        sess.Token = "mtcgToken";
        sess.IsLoggedIn = true;
        sess.LastAction = DateTime.Now;
        return sess;
    }
}

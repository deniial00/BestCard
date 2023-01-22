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

    public bool IsAdmin;

    public DateTime LastAction;

    public Session() { }

    public Session(string token)
    {
        Token = token;
        UserID = null;
        IsLoggedIn = false;
        IsAdmin = false;
        LastAction = DateTime.Now;
    }

    public Session(string username, string token)
    {
        Token = token;
        UserName = username;
        IsLoggedIn = false;
        IsAdmin = false;
        LastAction = DateTime.Now;
    }

}

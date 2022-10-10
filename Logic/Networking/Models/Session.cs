using Newtonsoft.Json;
using System.Net.Sockets;

namespace Logic.Networking.Models;

class Session
{
    public string Token;

    public int? UserID;

    public bool IsLoggedIn;

    public Session(string token)
    {
        Token = token;
        UserID = null;
        IsLoggedIn = false;
    }
}

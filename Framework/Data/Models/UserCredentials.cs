using Framework.Data.Controller;
using Newtonsoft.Json;
using System;
namespace Framework.Data.Models;

public class UserCredentials
{
    [JsonProperty("Username")]
    public string Username { get; set; }

    [JsonProperty("Password")]
    public string Password { get; set; }

    public UserCredentials() { }

    [JsonConstructor]
    public UserCredentials(string name, string password)
    {
        Username = name;
        Password = UserService.HashPassword(password);
    }
}


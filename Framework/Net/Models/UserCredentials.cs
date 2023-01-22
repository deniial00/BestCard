using Framework.Data.Controller;
using Newtonsoft.Json;
using System;
namespace Framework.Net.Models;

public class UserCredentials
{
    [JsonProperty("Username")]
    public string Username { get; set; }

    [JsonProperty("Password")]
    public string Password { get; set; }

    public UserCredentials()
    {
        Username = "";
        Password = "";
    }

    [JsonConstructor]
    public UserCredentials(string name, string password)
    {
        Username = name;
        if (password is not null && password != "")
            Password = UserService.HashPassword(password);
    }
}


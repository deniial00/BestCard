using Framework.Data.Controller;
using Newtonsoft.Json;
using System;
namespace Framework.Data.Models;

public class UserCredentialModel
{
    [JsonProperty("Username")]
    public string Username { get; set; }

    [JsonProperty("Password")]
    public string Password { get; set; }

    public UserCredentialModel()
    {
        Username = "";
        Password = "";
    }

    [JsonConstructor]
    public UserCredentialModel(string name, string password)
    {
        Username = name;
        Password = UserService.HashPassword(password);
    }
}


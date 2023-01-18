using System;
using System.Net;
using Newtonsoft.Json;
using System.Text;

namespace Framework.Net.Models;

public class UserNotLoggedInException : Exception
{
	public UserNotLoggedInException()
		: base("User not logged in")
	{
	}

	public UserNotLoggedInException(Exception inner)
		: base("User not logged in", inner)
	{

	}

    public UserNotLoggedInException(HttpListenerResponse res)
        : base("User not logged in")
    {
        res.StatusCode = 401;

        res.ContentEncoding = Encoding.UTF8;
        res.Headers.Add(HttpRequestHeader.ContentType, "application/json");
        byte[] buffer = Array.Empty<byte>();
        string jsonString = JsonConvert.SerializeObject($"{{ \"Error\": \"No active Session. Please log in.\" }}");
        buffer = Encoding.UTF8.GetBytes(jsonString);
        res.OutputStream.WriteAsync(buffer);
    }

    public UserNotLoggedInException(string message, HttpListenerResponse res)
        : base("User not logged in")
    {
        res.StatusCode = 401;

        res.ContentEncoding = Encoding.UTF8;
        res.Headers.Add(HttpRequestHeader.ContentType, "application/json");
        byte[] buffer = Array.Empty<byte>();
        string jsonString = JsonConvert.SerializeObject($"{{ \"Error\": \"{message}\" }}");
        buffer = Encoding.UTF8.GetBytes(jsonString);
        res.OutputStream.WriteAsync(buffer);
    }
}


using System.Net;
using Framework.Data.Controller;
using Framework.Data.Models;
using Framework.Networking.Controller;
using Framework.Networking.Models;

try
{
    var server = new ServerController();
    var db = DatabaseController.GetInstance();

    // Create User
    server.AddRoute("/users", HttpMethod.Post, true, async (req, res) =>
    {
        UserCredentials cred;
        try
        {
            cred = server.RequestToObject<UserCredentials>(req);

            if (cred is null)
                throw new Exception("Could not parse json");

            //cred.Password = UserService.HashPassword(cred.Password);
            UserService.GetInstance().CreateUser(cred);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Coult not create user: {ex.Message}");
            await server.SendResponseAsync(res, 500, $"User not created: {ex.Message}");
            return;
        }

        await server.SendResponseAsync(res, 201, $"User {cred.Username} created");
    });

    // low priority
    //server.AddRoute("/users/*", HttpMethod.Get, true, (HttpListenerRequest req, HttpListenerResponse res) =>
    //{
    //    try
    //    {

    //    }
    //    catch(Exception ex)
    //    {
    //        Console.WriteLine($"Coult not create User with message: {ex.Message}");
    //        server.SendResponse(res, 500, $"User not created: {ex.Message}");
    //        return;
    //    }

    //    server.SendResponse(res, 201, "User created");
    //});

    // Login User
    server.AddRoute("/sessions", HttpMethod.Post, false, async (req, res) =>
    {
        UserCredentials cred = new();
        Session session = new();
        try
        {
            // get token from headers
            var tokenHeader = req.Headers.Get("Authorization");
            string token = "";
            if (tokenHeader is not null && tokenHeader.Contains('-'))
                token = tokenHeader.Substring(tokenHeader.LastIndexOf('-') + 1);

            // check if token already has a session
            cred = server.RequestToObject<UserCredentials>(req);
            session = server.CheckSession(cred, token);

            if (session.IsLoggedIn)
                throw new ArgumentException("User already logged in");

            session.IsLoggedIn = UserService.GetInstance().AuthenticateUser(cred);

            if (!session.IsLoggedIn)
                throw new InvalidOperationException("Invalid Credentials");

            // set cookie
            res.Cookies.Add(new Cookie("Authorization", $"Basic {cred.Username}-{session.Token}"));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Could not authenticate user: {ex.Message}");
            await server.SendResponseAsync(res, 401, $"User could not be authenticated: {ex.Message}");
            return;
        }

        await server.SendResponseAsync(res, 200, $"User {cred.Username} authenticated");
    });

    //// Create Package
    //server.AddRoute("/packages", HttpMethod.Get, (req, res) =>
    //{
    //    // check auth
    //    dynamic json = ServerController.RequestToJson(req);

    //});

    //// Buy Package
    //server.AddRoute("/transactions", HttpMethod.Get, (req, res) =>
    //{
    //    // check auth


    //});

    await server.HandleRequestsAsync();

}
catch (Exception ex)
{
    Console.WriteLine(ex);
}


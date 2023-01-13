using System.Net;
using Framework.Data.Controller;
using Framework.Data.Models;
using Framework.Net.Controller;
using Framework.Net.Models;

try
{
    var server = new ServerController();
    var db = DatabaseController.GetInstance();

    // Create User
    server.AddRoute("/users", HttpMethod.Post, true, async (req, res) =>
    {
        try
        {
            UserCredentialModel cred = server.RequestToObject<UserCredentialModel>(req.InputStream);

            if (cred is null)
                throw new Exception("Could not parse json");

            //cred.Password = UserService.HashPassword(cred.Password);
            UserService.GetInstance().CreateUser(cred);
            await server.SendResponseAsync(res, 201, $"User {cred.Username} created");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Coult not create user: {ex.Message}");

            int statusCode = ex.Message.CompareTo("User already exists") == 0 ? 409 : 500;
            await server.SendResponseAsync(res, statusCode, $"User not created: {ex.Message}");
        }

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
        try
        {
            // check if token already has a session
            var session = server.CheckSession(req);
            var cred = server.RequestToObject<UserCredentialModel>(req.InputStream);

            if (session is null)
                session = server.CreateSession(cred, res);

            if (session.IsLoggedIn)
                throw new ArgumentException("User already logged in");
            else 
                session.IsLoggedIn = UserService.GetInstance().AuthenticateUser(cred);

            // still not logged in? Invalid cred!
            if (!session.IsLoggedIn)
                throw new InvalidOperationException("Invalid Credentials");
            

            await server.SendResponseAsync(res, 200, $"{{ \"Message\": \"User\" {cred.Username} authenticated,\"Token\" :"+session.Token+"}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Could not authenticate user: {ex.Message}");
            await server.SendResponseAsync(res, 400, $"User could not be authenticated: {ex.Message}");
        }

    });

    //// Create Package
    server.AddRoute("/packages", HttpMethod.Get, true, async (req, res) =>
    {
        try
        {
            CardModel[] cards = server.RequestToObject<CardModel[]>(req.InputStream);

            bool success = CardService.AddPackage(cards);

            if (success)
                await server.SendResponseAsync(res, 200, $"{{ \"Message\": \"Package created\"}}");

        } catch (Exception ex)
        {
            Console.WriteLine($"Could not create Package: {ex.Message}");
            await server.SendResponseAsync(res, 400, $"Package not created: {ex.Message}");
        }

    });

    // Buy Package
    server.AddRoute("/transactions/packages", HttpMethod.Get, true, (req, res) =>
    {
        try
        {
            var session = server.GetSession(server.GetTokenOfRequest(req));
            if (session is null)
                throw new UserNotLoggedInException();

            bool success = CardService.AcquirePackage((int) session.UserID);
        }
        catch (Exception ex)
        {

        }
    });

    await server.HandleRequestsAsync();

}
catch (Exception ex)
{
    Console.WriteLine(ex);
}


using System.Net;
using Framework.Battle.Controller;
using Framework.Data.Controller;
using Framework.Data.Models;
using Framework.Net.Controller;
using Framework.Net.Models;
using Newtonsoft.Json;

try
{
    var server = new ServerController();
    var battle = new BattleController();

    var db = DatabaseController.GetInstance();

    // Create User
    server.AddRoute("/users", "POST", false, async (ctx) =>
    {
        try
        {
            UserCredentials cred = server.RequestToObject<UserCredentials>(ctx.Request.InputStream);

            if (cred is null)
                throw new Exception("Could not parse json");

            //cred.Password = UserService.HashPassword(cred.Password);
            UserService.CreateUser(cred);
            await server.SendResponseAsync(ctx.Response, 201, $"User {cred.Username} created");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Coult not create user: {ex.Message}");

            int statusCode = ex.Message.CompareTo("User already exists") == 0 ? 409 : 500;
            await server.SendResponseAsync(ctx.Response, statusCode, $"{{ \"Error\": \"User not created: {ex.Message}\"}}");
        }

    });

    // Login User
    server.AddRoute("/sessions", "POST", false, async (ctx) =>
    {
        try
        {
            // check if token already has a session
            var session = server.CheckSession(ctx);
            var cred = server.RequestToObject<UserCredentials>(ctx.Request.InputStream);

            if (session is null)
                session = server.CreateSession(cred, ctx.Response);

            if (session.IsLoggedIn)
                throw new ArgumentException("User already logged in");
            else 
                session.IsLoggedIn = UserService.AuthenticateUser(cred);

            // still not logged in? Invalid cred!
            if (!session.IsLoggedIn)
                throw new InvalidOperationException("Invalid Credentials");
            

            await server.SendResponseAsync(ctx.Response, 200, $"{{ \"Message\": \"User\" {cred.Username} authenticated,\"Token\": \"{ session.Token}\" }}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Could not authenticate user: {ex.Message}");
            await server.SendResponseAsync(ctx.Response, 400, $"{{ \"Error\": \"User could not be authenticated: {ex.Message}\"}}");
        }

    });

    // Create Package
    // TODO: check if admin
    server.AddRoute("/packages", "POST", true, async (ctx) =>
    {
        try
        {
            CardModel[] cards = server.RequestToObject<CardModel[]>(ctx.Request.InputStream);

            CardService.AddPackage(cards);

            await server.SendResponseAsync(ctx.Response, 200, $"{{ \"Message\": \"Package created\"}}");

        } catch (Exception ex)
        {
            Console.WriteLine($"Could not create Package: {ex.Message}");
            await server.SendResponseAsync(ctx.Response, 400, $"{{ \"Error\": \"Could not create package: {ex.Message}\"}}");
        }
    });

    // Buy Package
    server.AddRoute("/transactions/packages", "POST", true, async (ctx) =>
    {
        try
        {
            var session = server.GetSession(ctx);

            CardService.AcquirePackage((int) session.UserID);

            await server.SendResponseAsync(ctx.Response, 200, $"{{ \"Message\": \"Package acquired\"}}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Could not create Package: {ex.Message}");
            await server.SendResponseAsync(ctx.Response, 400, $"{{ \"Error\": \"{ex.Message}\"}}");
        }
    });


    // Get Cards of user
    server.AddRoute("/cards", "GET", true, async (ctx) =>
    {
        try
        {
            var session = server.GetSession(ctx);

            List<CardModel> cards = CardService.GetCardsByUserId((int) session.UserID);

            string json = JsonConvert.SerializeObject(cards);

            await server.SendResponseAsync(ctx.Response, 200, json);

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Could not create Package: {ex.Message}");
            await server.SendResponseAsync(ctx.Response, 400, $"{{ \"Error\": \"Could not create package: {ex.Message}\"}}");
        }

    });


    // Get current Deck of user
    server.AddRoute("/deck", "GET", true, async (ctx) =>
    {
        try
        {
            var session = server.GetSession(ctx);

            List<CardModel> cards = CardService.GetDeckByUserId((int)session.UserID);

            string json = JsonConvert.SerializeObject(cards);

            await server.SendResponseAsync(ctx.Response, 200, json);

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Could not retrieve deck: {ex.Message}");
            await server.SendResponseAsync(ctx.Response, 400, $"{{ \"Error\": \"{ex.Message}\"}}");
        }

    });

    // Set Deck by card ids of user
    server.AddRoute("/deck", "PUT", true, async (ctx) =>
    {
        try
        {
            var session = server.GetSession(ctx);

            List<string> cards = server.RequestToObject<List<string>>(ctx.Request.InputStream);

            CardService.SetDeckByUserId((int) session.UserID, cards);

            await server.SendResponseAsync(ctx.Response, 200, $"{{ \"Message\": \"Cards selected\" }}");

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Could not create Package: {ex.Message}");
            await server.SendResponseAsync(ctx.Response, 400, $"{{ \"Error\": \"Could not select cards: {ex.Message}\"}}");
        }

    });

    server.AddRoute("/battles", "POST", true, async (ctx) =>
    {
        try
        {
            var session = server.GetSession(ctx);

            

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Could not create Package: {ex.Message}");
            await server.SendResponseAsync(ctx.Response, 400, $"{{ \"Error\": \"Could not select cards: {ex.Message}\"}}");
        }
    });


    await server.HandleRequestsAsync();
}
catch (Exception ex)
{
    Console.WriteLine(ex);
}


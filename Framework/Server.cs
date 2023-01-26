using System.Net;
using Newtonsoft.Json;
using Framework.Data.Controller;
using Framework.Data.Models;
using Framework.Net.Controller;
using Framework.Net.Models;
using Newtonsoft.Json.Linq;

try
{
    var server = ServerController.GetInstance();
    var bc = BattleController.GetInstance();
    var db = DatabaseController.GetInstance();

    int battleTimeoutSeconds = 30;


    // Create User
    server.AddRoute("/users", "POST", false, async (ctx) =>
    {
        try
        {
            UserCredentials cred = server.RequestToObject<UserCredentials>(ctx.Request.InputStream);

            if (cred is null)
                throw new Exception("Could not parse json");

            UserService.CreateUser(cred);
            await server.SendResponseAsync(ctx.Response, 201, $"{{ \"message\": \"User {cred.Username} created\" }}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Coult not create user: {ex.Message}");

            int statusCode = ex.Message.CompareTo("User already exists") == 0 ? 409 : 500;
            await server.SendResponseAsync(ctx.Response, statusCode, $"{{ \"error\": \"User not created: {ex.Message}\" }}");
        }

    });

    // Get User
    server.AddRoute("/users", "GET", true, async (ctx) =>
    {
        try
        {
            var sess = server.GetSession(ctx);

            if (sess is null || sess.UserID is null)
                throw new UserNotLoggedInException();

            UserModel user = UserService.GetUser((int) sess.UserID);

            if (user is null)
                throw new Exception("User not found");

            string json = JsonConvert.SerializeObject(user);
            await server.SendResponseAsync(ctx.Response, 200, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Coult not get User: {ex.Message}");

            await server.SendResponseAsync(ctx.Response, 500, $"{{ \"error\": \"Could not fetch user: {ex.Message}\" }}");
        }

    });

    // Login User
    server.AddRoute("/sessions", "POST", false, async (ctx) =>
    {
        try
        {
            var cred = server.RequestToObject<UserCredentials>(ctx.Request.InputStream);
            var session = server.CreateSession(cred.Username);
            ctx.Response.SetCookie(new Cookie("Authorization", $"Basic {session.Token}"));

            session.UserID = UserService.AuthenticateUser(cred);

            if (session.UserID > 0)
                session.IsLoggedIn = true;
            

            // still not logged in? Invalid cred!
            if (!session.IsLoggedIn)
                throw new InvalidOperationException("Invalid Credentials");
            

            await server.SendResponseAsync(ctx.Response, 200, $"{{ \"message\": \"User {cred.Username} authenticated\", \"token\": \"{session.Token}\" }}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Could not authenticate user: {ex.Message}");
            await server.SendResponseAsync(ctx.Response, 400, $"{{ \"error\": \"User could not be authenticated: {ex.Message}\"}}");
        }

    });

    // Create Package
    server.AddRoute("/packages", "POST", true, async (ctx) =>
    {
        try
        {
            server.CheckAdmin(ctx.Request);
            CardModel[] cards = server.RequestToObject<CardModel[]>(ctx.Request.InputStream);

            CardService.AddPackage(cards);

            await server.SendResponseAsync(ctx.Response, 200, $"{{ \"message\": \"Package created\"}}");

        } catch (Exception ex)
        {
            Console.WriteLine($"Could not create Package: {ex.Message}");
            await server.SendResponseAsync(ctx.Response, 400, $"{{ \"error\": \"Could not create package: {ex.Message}\"}}");
        }
    });

    // Buy Package
    server.AddRoute("/transactions/packages", "POST", true, async (ctx) =>
    {
        try
        {
            var session = server.GetSession(ctx);

            CardService.AcquirePackage((int) session.UserID);

            await server.SendResponseAsync(ctx.Response, 200, $"{{ \"message\": \"Package acquired\" }}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Could not create Package: {ex.Message}");
            await server.SendResponseAsync(ctx.Response, 400, $"{{ \"error\": \"{ex.Message}\"}}");
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
            Console.WriteLine($"Could retrieve cards: {ex.Message}");
            await server.SendResponseAsync(ctx.Response, 400, $"{{ \"error\": \"Could retrieve cards: {ex.Message}\" }}");
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
            await server.SendResponseAsync(ctx.Response, 400, $"{{ \"error\": \"{ex.Message}\"}}");
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

            await server.SendResponseAsync(ctx.Response, 200, $"{{ \"message\": \"Cards selected\" }}");

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Could not create Package: {ex.Message}");
            await server.SendResponseAsync(ctx.Response, 400, $"{{ \"error\": \"Could not select cards: {ex.Message}\"}}");
        }

    });

    server.AddRoute("/battles", "POST", true, async (ctx) =>
    {
        try
        {
            var session = server.GetSession(ctx);
            var userId = session.UserID;

            if (userId is null)
                throw new UserNotLoggedInException();


            var battle = bc.AddPlayerToLobby((int) userId);

            string jsonResponse = "";

            int checkCount = 0;
            while (!battle.ResultsAvailable)
            {
                if (checkCount >= battleTimeoutSeconds / 0.5)
                    throw new BattleException("No avaiable opponent. Timeout after 30s.");

                if (battle.ResultsAvailable)
                    jsonResponse = battle.ToJsonString();
                
                checkCount++;

                Thread.Sleep(500);
            }

            await server.SendResponseAsync(ctx.Response, 200, battle.ToJsonString());

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Could not find battle: {ex.Message}");
            await server.SendResponseAsync(ctx.Response, 400, $"{{ \"error\": \"Could not find Battle: {ex.Message}\"}}");
        }
    });

    server.AddRoute("/gift", "POST", true, async (ctx) =>
    {
        try
        {
            var session = server.GetSession(ctx);

            var cred = server.RequestToObject<UserCredentials>(ctx.Request.InputStream);

            var receiverUser = UserService.GetUser(null, cred.Username);

            if (receiverUser is null)
                throw new ArgumentException("User not found");

            CardService.AcquirePackage(receiverUser.UserId, (int)session.UserID);

            await server.SendResponseAsync(ctx.Response, 200, $"{{ \"message\": \"Package gifted to {receiverUser.Username}\" }}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Could not gift Package: {ex.Message}");
            await server.SendResponseAsync(ctx.Response, 400, $"{{ \"error\": \"{ex.Message}\"}}");
        }
    });

    server.AddRoute("/stats", "GET", true, async (ctx) =>
    {
        try
        {
            var session = server.GetSession(ctx);

            var cred = server.RequestToObject<UserCredentials>(ctx.Request.InputStream);

            if (session.UserID is null)
                throw new UserNotLoggedInException();

            var stats = BattleService.GetStatistics((int) session.UserID);

            string jsonResponse = JsonConvert.SerializeObject(stats);

            await server.SendResponseAsync(ctx.Response, 200, jsonResponse);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Could not get stats: {ex.Message}");
            await server.SendResponseAsync(ctx.Response, 400, $"{{ \"error\": \"{ex.Message}\"}}");

        }
    });


    await server.Listen();
}
catch (Exception ex)
{
    Console.WriteLine(ex);
}


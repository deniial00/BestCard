using Framework.Net.Controller;
using Framework.Net.Models;
using Framework.Data.Controller;
using Framework.Data.Models;
namespace BestCard.Test;

public class ServerTests
{
    ServerController server;

    // Read statement (maybe postgres db? => change constructor
    // check token (if 64 char)
    // RequestToObject => test with all data.models
    // invalidate session => check if not in Sessions Dict
    // Get Token of request = simply build a req with only header
    // createUser
    // check if exists
    // authenticate user fail (only use authenticateUser func)
    // authenticate user correct
    // trygetRoute => test with /Sessions
    // try battle controller aswell => ?

    [SetUp]
    public void Setup()
    {
        server = new ServerController();
    }

    [Test]
    public void DatabaseAccessTest()
    {
        var db = DatabaseController.GetInstance();

        bool success = db.CheckConnection();

        if (success)
            Assert.Pass();
        else
            Assert.Fail();
    }

    [Test]
    public void CheckAdminUserSession()
    {
        var cred = new UserCredentialModel("denial", "password");
        var expectedSession = Session.AdminUserSession();

        Assert.That(expectedSession.Token, Is.EqualTo("Basic admin-mtcgToken"));
        Assert.That(expectedSession.UserID, Is.EqualTo(1));
        Assert.That(expectedSession.IsLoggedIn, Is.EqualTo(true));
    }

    [Test]
    public void RouteModelConstructorTest()
    {
        var route = new Route("/test", HttpMethod.Get, true, (req, res) =>
        {
            
        });

        if (route is not null)
            Assert.Pass();
        else
            Assert.Fail();
    }

    [Test]
    public void SessionModelConstructorTest()
    {
        var token = UserService.GenerateToken64();
        var session = new Session(token);

        if (session.Token == token && session.IsLoggedIn == false)
            Assert.Pass();
        else
            Assert.Fail();
    }

    [Test]
    public void UserCredentialsModelConstructorTest()
    {
        var cred = new UserCredentialModel("admin", "admin");

        if (cred is not null && cred.Username == "admin" && cred.Password != "admin")
            Assert.Pass();
        else
            Assert.Fail();
    }
}

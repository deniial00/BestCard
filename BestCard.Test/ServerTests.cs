using Framework.Data.Models.Cards;
using Framework;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BestCard.Test;

[TestFixture]
public class BattleTests
{
    ServerController server = ServerController.GetInstance();
    DatabaseController data = DatabaseController.GetInstance();

    [OneTimeSetUp]
    public void SetUpBattleTest()
    {
        data.ResetDatabase();
        data.InitDatabase();
    }

    [Test, Order(1)]
    public void BattleTest()
    {
        // get cards for user1
        var user1 = UserService.GetUser(null, FrameworkEnviroment.User1.Username);

        if (user1 is null)
            throw new Exception("User1 not found");

        CardService.AcquirePackage(user1.UserId);

        var cards1 = CardService.GetCardsByUserId(user1.UserId);

        List<string> cardIds1 = new();
        cards1.ForEach(card => {
            if (cardIds1.Count == 4)
                return;
            cardIds1.Add(card.CardId);
        });

        CardService.SetDeckByUserId(user1.UserId, cardIds1);

        // get cards for user2
        var user2 = UserService.GetUser(null, FrameworkEnviroment.User1.Username);

        if (user2 is null)
            throw new Exception("User2 not found");

        CardService.AcquirePackage(user2.UserId);

        var cards2 = CardService.GetCardsByUserId(user2.UserId);

        List<string> cardIds2 = new();
        cards2.ForEach(card => {
            if (cardIds2.Count == 4)
                return;
            cardIds2.Add(card.CardId);
        });

        CardService.SetDeckByUserId(user2.UserId, cardIds2);

        // battle user1 vs user2
        var battleC = BattleController.GetInstance();

        battleC.AddPlayerToLobby(user1.UserId);
        var battle = battleC.AddPlayerToLobby(user2.UserId);

        if (battle.ResultsAvailable == true)
            Assert.Pass();
    }

    [Test, Order(2)]
    public void AddPlayerToLobbyTest()
    {
        var battleController = BattleController.GetInstance();

        var battle = battleController.AddPlayerToLobby(1);

        if (battle.ChampionUserId == 1 && battleController.LobbyCount == 1)
            Assert.Pass();
        else
            Assert.Fail();

    }
}

[TestFixture]
public class ServerTests
{
    ServerController server = ServerController.GetInstance();
    DatabaseController data = DatabaseController.GetInstance();
    int TestUserId;

    [OneTimeSetUp]
    public void Setup()
    {
        data.ResetDatabase();
        data.InitDatabase();
    }

    [Test, Order(1)]
    public void DatabaseAccessTest()
    {
        var db = DatabaseController.GetInstance();

        bool success = db.CheckConnection();

        if (success)
            Assert.Pass();
        else
            Assert.Fail();
    }

    [Test, Order(2)]
    public void NoSessionTest()
    {
        Assert.Throws<UserNotLoggedInException>(() => server.CheckAdmin("notoken"));
    }


    [Test, Order(3)]
    public void CreateUserTest()
    {
        var cred = new UserCredentials("test", "passwort");

        TestUserId = UserService.CreateUser(cred);

        var user = UserService.GetUser(TestUserId);

        if (user is not null && user.UserId == TestUserId)
            Assert.Pass();
        else
            Assert.Fail();
    }

    [Test, Order(4)]
    public void LoginUserTest()
    {
        var cred = new UserCredentials("test", "passwort");
        string token = "testToken";

        var sess = server.CreateSession(cred.Username, token);

        sess.UserID = UserService.AuthenticateUser(cred, token);

        if (sess.UserID > 0 && server.GetSession(cred.Username + "-" + token) is not null)
            Assert.Pass();
    }

    [Test, Order(5)]
    public void AddPackagesTest()
    {
        List<CardModel> cards = new()
        {
            new CardModel("1", "test", 5.0f),
            new CardModel("2", "test", 5.0f),
            new CardModel("3", "test", 5.0f),
            new CardModel("4", "test", 5.0f),
            new CardModel("5", "test", 5.0f)
        };

        CardService.AddPackage(cards.ToArray());

        var card = CardService.GetCard("1");

        if (card.CardId == "1" && card.CardName == "test" && card.CardDamage == 5.0f)
            Assert.Pass();
        else
            Assert.Fail();
    }

    [Test, Order(6)]
    public void AcquirePackage()
    {
        var credits = UserService.GetUser(TestUserId).Credits;

        CardService.AcquirePackage(TestUserId);

        var user = UserService.GetUser(TestUserId);

        if (user is not null && user.Credits != credits - 5)
            Assert.Fail();

        var cards = CardService.GetCardsByUserId(TestUserId);

        if (cards.Count == 5)
            Assert.Pass();
    }

    [Test, Order(7)]
    public void CheckAdminTest()
    {
        var cred = FrameworkEnviroment.Admin;
        var hash = "mtcgToken";
        var token = cred.Username+"-"+hash;

        server.CreateSession(cred.Username, hash);
        
        bool admin = server.CheckAdmin(token);

        if (admin)
            Assert.Pass();
    }

    [Test, Order(8)]
    public void HashPasswordTest()
    {
        string pw = "passwort";

        string hash = UserService.HashPassword(pw);

        if (hash != "33c5ebbb01d608c254b3b12413bdb03e46c12797e591770ccf20f5e2819929b2")
            Assert.Fail();
    }

    [Test, Order(9)]
    public void GenerateTokenTest()
    {
        int length = 16;
        string tok =UserService.GenerateToken(length);

        if (tok.Length != length)
            Assert.Fail();
    }

    [Test, Order(10)]
    public void GetUsernameTest()
    {
        string username = UserService.GetUsername(TestUserId);

        if (username is null)
            Assert.Fail();
        else if (username == "test")
            Assert.Pass();
    }

    [Test, Order(11)]
    public void UpdateCreditsTest()
    {
        int credits = UserService.GetUser(TestUserId).Credits;

        int res = UserService.UpdateUserCredits(TestUserId, 10);

        if (res == -1)
            Assert.Fail();

        int newCredits = UserService.GetUser(TestUserId).Credits;

        if (credits + 10 != newCredits)
            Assert.Fail();
    }

    [Test, Order(12)]
    public void GetCardsByUserIdTest()
    {
        var cards = CardService.GetCardsByUserId(TestUserId);

        if (cards.Count != 5)
            Assert.Fail();
        else
            Assert.Pass();
    }

    [Test, Order(13)]
    public void SetDeckByUserIdFailTest()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            var cards = CardService.GetCardsByUserId(TestUserId);
            List<string> cardIds = new();
            cards.ForEach(card => {
                cardIds.Add(card.CardId);
            });

            CardService.SetDeckByUserId(TestUserId, cardIds);
        });
    }

    [Test, Order(14)]
    public void SetDeckByUserIdTest()
    {
        var cards = CardService.GetCardsByUserId(TestUserId);
        List<string> cardIds = new();
        cards.ForEach(card => {
            if (cardIds.Count == 4)
                return;
            cardIds.Add(card.CardId);
        });

        CardService.SetDeckByUserId(TestUserId, cardIds);
    }

    [Test, Order(15)]
    public void GetDeckByUserIdTest()
    {
        var cards = CardService.GetDeckByUserId(TestUserId);

        if (cards is not null && cards.Count == 4)
            Assert.Pass();
        else
            Assert.Fail();
    }

    [Test, Order(16)]
    public void UserModelConstructorTest()
    {
        var user = new UserModel(0, "testy", false, 20);

        if (user.UserId == 0 && user.Username == "testy" &&
            user.IsAdmin == false && user.Credits == 20)
            Assert.Pass();
        else
            Assert.Fail();
    }

    [Test, Order(17)]
    public void UserCredentialsConstructorTest()
    {
        var cred = new UserCredentials("admin", "admin");

        if (cred is not null && cred.Username == "admin" && cred.Password != "admin")
            Assert.Pass();
        else
            Assert.Fail();
    }

    [Test, Order(18)]
    public void ResetDatabaseTest()
    {
        data.ResetDatabase();

        var user = UserService.GetUser(null, "test");

        if (user is null)
            Assert.Pass();
    }

    [Test, Order(19)]
    public void InitDatabase()
    {
        data.InitDatabase();

        var user = UserService.GetUser(null, "admin");

        if (user is null)
            Assert.Fail();
    }
}

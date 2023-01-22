using Framework.Data.Models;
using Framework.Data.Models.Cards;
using Framework.Models;

namespace Framework.Data.Controller;

public class BattleController
{
    private Dictionary<int,BattleModel> Lobby;
    private static BattleController? Instance;

    public int LobbyCount { get => Lobby.Count; }

    private BattleController()
    {
        Lobby = new Dictionary<int, BattleModel>();
    }

    public static BattleController GetInstance()
    {
        if (Instance is null)
            Instance = new BattleController();

        return Instance;
    }

    public BattleModel AddPlayerToLobby(int userId)
    {
        BattleModel battle;

        var championElo = BattleService.GetEloOfUser(userId);

        if (Lobby.TryGetValue(championElo, out battle))
        {
            // FIGHT!
            battle.ChallengerUserId = userId;
            InitiateBattle(ref battle);
            Lobby.Remove(championElo);
            BattleService.StoreBattle(battle);
        } else
        {
            battle = new BattleModel(userId);
            Lobby.Add(championElo, battle);
        }
        
        return battle;
    }

    public void InitiateBattle(ref BattleModel battle)
    {
        if (battle.ChallengerUserId is null)
            throw new BattleException("Challenger UserID not provided");

        var championCardModels = CardService.GetDeckByUserId(battle.ChampionUserId);
        CardDeck championDeck = new CardDeck(CardFactory.ConvertCards(championCardModels));

        var challengerCardModels = CardService.GetDeckByUserId((int) battle.ChallengerUserId);
        CardDeck challengerDeck = new CardDeck(CardFactory.ConvertCards(challengerCardModels));

        List<BattleRound> battles = new();

        while (championDeck.Count > 0 && challengerDeck.Count > 0)
        {
            if (battles.Count >= 100)
                break;

            var championCard = championDeck.DrawCard();
            var challengerCard = challengerDeck.DrawCard();
            var round = new BattleRound(championCard, challengerCard);

            
            switch (round.ChallengerSucceded)
            {
                case true:
                    // Challenger won
                    challengerDeck.AddCard(championCard);
                    break;
                case false:
                    // Champion won
                    championDeck.AddCard(challengerCard);
                    break;
                case null:
                    // DRAW
                    challengerDeck.AddCard(challengerCard);
                    championDeck.AddCard(championCard);
                    break;
            }

            battles.Add(round);
        }

        battle.BattleRounds = battles;
        battle.ResultsAvailable = true;
    }
}

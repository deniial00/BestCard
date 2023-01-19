using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Framework.Data.Models;
using Framework.Data.Models.Cards;

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
            InitiateBattle(battle);
        } else
        {
            battle = new BattleModel(userId);
            Lobby.Add(championElo, battle);
        }
        
        return battle;
    }

    // TODO: IniateBattle
    // get decks of users
    // Calculate Result 100 times or until deck of user is empty
    // always give card of loser to winner
    // 100 battles => draw

    public void InitiateBattle(BattleModel battle)
    {
        if (battle.ChallengerUserId is null)
            throw new BattleException("Challenger UserID not provided");

        var championCardModels = CardService.GetDeckByUserId(battle.ChampionUserId);
        CardDeck championdeck = new CardDeck(CardFactory.ConvertCards(championCardModels));

        var challengerCardModels = CardService.GetDeckByUserId((int) battle.ChallengerUserId);
        CardDeck challengerdeck = new CardDeck(CardFactory.ConvertCards(championCardModels));




    }
}

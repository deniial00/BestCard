using System;
namespace Framework.Data.Models;

public class BattleModel
{
    public int BattleId;
    public int ChampionUserId;
    public int? ChallengerUserId;

    public int? ChampionEloChange;
    public int? ChallengerEloChange;

    public DateTime BattleTimeStamp;

    public bool ResultsAvailable;

	public BattleModel(int userId)
	{
        ChampionUserId = userId;
        ResultsAvailable = false;
	}
}


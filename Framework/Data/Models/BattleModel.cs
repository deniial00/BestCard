using System;
using Framework.Data.Controller;
using Framework.Models;

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
    public List<BattleRound> BattleRounds;

    public BattleModel(int userId)
	{
        ChampionUserId = userId;
        ResultsAvailable = false;
        BattleRounds = new();
	}
    // TODO: Format json: 'rounds' in array
    // curl prettxy print?
    public string ToJsonString()
    {
        string championUsername = UserService.GetUsername(ChampionUserId);

        if (ChallengerUserId is null)
            throw new ArgumentNullException("Could not retrieve Challenger");

        string challengerUsername = UserService.GetUsername((int) ChallengerUserId);


        string jsonString = "{";
        jsonString += $"\"match\": \"{championUsername.Trim()} vs {challengerUsername.Trim()}\",\"elo_change\": {{ \"champion\": {ChampionEloChange}, \"challenger\": {ChallengerEloChange}}}, \"rounds\": [";
        int roundCount = 1;

        foreach (var round in BattleRounds)
        {
            jsonString += $"{round.SummarizeRoundJsonString()}";
            if (roundCount <= BattleRounds.Count - 1)
                jsonString += ",";
            roundCount++;
        }
        
        jsonString += "]}";

        return jsonString;
    }
}


using System;
using Framework.Battle.Controller;

namespace Framework.Data.Controller;

public static class BattleService
{
	public static int AddToLobby()
	{
		var battle = BattleController.GetInstance();
		return 1;
	}

	// TODO: stats/scoreboard
	// Store battles in db with: champion_user_id (?), challenger_user_id, challenger_succeded, champion_elo_change, challenger_elo_change
	// In scoreboard calculate elo based on changes
}


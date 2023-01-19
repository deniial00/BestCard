using System;
using Framework.Data.Controller;
using Npgsql;

namespace Framework.Data.Controller;

public static class BattleService
{
	public static int baseElo = 1000;

	public static int GetEloOfUser(int userId)
	{
		var db = DatabaseController.GetInstance();

		string getUserEloQuery =
            @"SELECT 
				(
					SELECT
						SUM(champion_elo_change)
					FROM bestcard.battles
					WHERE champion_user_id = @userid
				) + 
					SELECT
						SUM(challenger_elo_change)
					FROM bestcard.battles
					WHERE challenger_user_id = @userid
				)
				as elo_calc 
			FROM bestcard.battles;";
		var getUserEloCmd = new NpgsqlCommand(getUserEloQuery, db.GetConnection());
		getUserEloCmd.Parameters.Add(new NpgsqlParameter("@userid", userId));

		var returnVal = getUserEloCmd.ExecuteScalar();

		if (returnVal is not null)
		{
			return baseElo + (int) returnVal;
		}
		return baseElo;
	}

	// TODO: stats/scoreboard
	// Store battles in db with: champion_user_id (?), challenger_user_id, challenger_succeded, champion_elo_change, challenger_elo_change
	// In scoreboard calculate elo based on changes
}


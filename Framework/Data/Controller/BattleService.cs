using System;
using Framework.Data.Controller;
using Framework.Data.Models;
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
				(
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

		if (returnVal is not null && returnVal is not System.DBNull)
		{
			return baseElo + (int) returnVal;
		}
		return baseElo;
	}

	public static int StoreBattle(BattleModel battle)
	{
		var conn = DatabaseController.GetInstance().GetConnection();

		var tran = conn.BeginTransaction();

		string createBattleQuery =
			@"INSERT INTO bestcard.battles
			(
				champion_user_id,
				challenger_user_id,
				champion_elo_change,
				challenger_elo_change,
				battle_timestamp
			)
			VALUES
			(
				@championId,
				@challengerId,
				@championElo,
				@challengerElo,
				NOW()
			)
			RETURNING battle_id;";

		var createBattleCmd = new NpgsqlCommand(createBattleQuery, conn, tran);
		createBattleCmd.Parameters.Add(new NpgsqlParameter("@championId", battle.ChampionUserId));
        createBattleCmd.Parameters.Add(new NpgsqlParameter("@challengerId", battle.ChallengerUserId));
        createBattleCmd.Parameters.Add(new NpgsqlParameter("@championElo", battle.ChampionEloChange));
        createBattleCmd.Parameters.Add(new NpgsqlParameter("@challengerElo", battle.ChallengerEloChange));

		var battleId = createBattleCmd.ExecuteScalar();

		if (battleId is null || (int)battleId < 1)
		{
			tran.Rollback();
			throw new Exception("Query failed");
		}

		tran.Commit();
		return (int) battleId;
	}
}


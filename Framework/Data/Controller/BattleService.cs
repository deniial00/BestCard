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
						COALESCE(SUM(champion_elo_change), 0)
					FROM bestcard.battles
					WHERE champion_user_id = @userid
				) +
				(
					SELECT
						COALESCE(SUM(challenger_elo_change), 0)
					FROM bestcard.battles
					WHERE challenger_user_id = @userid
				)
				as elo_calc";
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
				battle_timestamp,
				battle_log
			)
			VALUES
			(
				@championId,
				@challengerId,
				@championElo,
				@challengerElo,
				NOW(),
				@battleLog
			)
			RETURNING battle_id;";

		var createBattleCmd = new NpgsqlCommand(createBattleQuery, conn, tran);
		createBattleCmd.Parameters.Add(new NpgsqlParameter("@championId", battle.ChampionUserId));
        createBattleCmd.Parameters.Add(new NpgsqlParameter("@challengerId", battle.ChallengerUserId));
        createBattleCmd.Parameters.Add(new NpgsqlParameter("@championElo", battle.ChampionEloChange));
        createBattleCmd.Parameters.Add(new NpgsqlParameter("@challengerElo", battle.ChallengerEloChange));
		createBattleCmd.Parameters.Add(new NpgsqlParameter("@battleLog", battle.ToJsonString()));

		var battleId = createBattleCmd.ExecuteScalar();

		if (battleId is null || (int)battleId < 1)
		{
			tran.Rollback();
			throw new Exception("Query failed");
		}

		tran.Commit();
		return (int) battleId;
	}

	public static Dictionary<string, string> GetStatistics(int userId)
	{
		var stats = new Dictionary<string, string>();
		var conn = DatabaseController.GetInstance().GetConnection();

		string q_getStats = @"
			SELECT
				COUNT(
					CASE WHEN 
						(champion_elo_change > 0 AND champion_user_id = @userId) OR
						(challenger_elo_change > 0 AND challenger_user_id = @userId)
					THEN 1 END
				) as wins,
				COUNT(*) as games,
				(
					SELECT 
						(
							SELECT
								COALESCE(SUM(champion_elo_change),0)
							FROM bestcard.battles
							WHERE champion_user_id = @userId
						) +
						(
							SELECT
								COALESCE(SUM(challenger_elo_change),0)
							FROM bestcard.battles
							WHERE challenger_user_id = @userId
						)
				) as elo
			FROM bestcard.battles
			WHERE
				champion_user_id = @userId OR
				challenger_user_id = @userId";
		var getStatsCmd = new NpgsqlCommand(q_getStats, conn);
		getStatsCmd.Parameters.Add(new NpgsqlParameter("@userId", userId));


		using (var reader = getStatsCmd.ExecuteReader())
		{
			while (reader.Read())
			{
				stats.Add("gamesWon", reader.GetInt32(0).ToString());
				stats.Add("gamesPlayed", reader.GetInt32(1).ToString());
				stats.Add("elo", reader.GetInt32(2).ToString());
			}

        }

        return stats;
	}
}


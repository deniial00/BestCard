using Npgsql;
using Framework.Data.Models;
using System.Xml.Linq;
using System.Transactions;

namespace Framework.Data.Controller;

public static class CardService
{
    public static CardModel? GetCard(string cardId)
    {
        var conn = DatabaseController.GetInstance().GetConnection();
        CardModel? card = null;

        string query = "SELECT * FROM bestcard.cards WHERE card_id = @card_id";
        var cmd = new NpgsqlCommand(query, conn);
        cmd.Parameters.Add(new NpgsqlParameter("@card_id", cardId));
        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                string id = reader.GetString(reader.GetOrdinal("card_id"));
                string name = reader.GetString(reader.GetOrdinal("card_name"));
                float dmg = reader.GetFloat(reader.GetOrdinal("card_damage"));
                card = new CardModel(id, name, dmg);
            }
        }

        return card;
    }

    public static void AddPackage(CardModel[] cards)
    {
        var connection = DatabaseController.GetInstance().GetConnection();

        string addPackageQuery =
            @"SELECT
                package_id
            FROM bestcard.cards
            ORDER BY package_id DESC
            LIMIT 1";

        var topPackageId = new NpgsqlCommand(addPackageQuery, connection).ExecuteScalar();

        // if no card is in db
        if (topPackageId is null)
            topPackageId = 0;

        int newPackageId = (int) topPackageId + 1;

        // prepare transaction
        var tran = connection.BeginTransaction();
        int affectedRows = 0;

        foreach (var card in cards)
        {
            var paramList = new List<NpgsqlParameter>();
            string addCardQuery =
                @"INSERT INTO bestcard.cards
                (
                    card_id,
                    card_name,
                    card_damage,
                    package_id
                )
                VALUES
                (
                    @CardId,
                    @CardName,
                    @CardDamage,
                    @PackageId
                );";

            paramList.Add(new NpgsqlParameter("@CardId", card.CardId));
            paramList.Add(new NpgsqlParameter("@CardName", card.CardName));
            paramList.Add(new NpgsqlParameter("@CardDamage", card.CardDamage));
            paramList.Add(new NpgsqlParameter("@PackageId", newPackageId));

            var addCardCommand = new NpgsqlCommand(addCardQuery, connection, tran);
            addCardCommand.Parameters.AddRange(paramList.ToArray());

            affectedRows += addCardCommand.ExecuteNonQuery();
        }

        // if 5 cards werent inserted => rollback
        if (affectedRows != 5)
        {
            tran.Rollback();
            throw new ArgumentException("SQL Error");
        }

        tran.Commit();
    }

    public static void AcquirePackage(int receiverId, int? payerId = null)
    {
        UserModel? user = UserService.GetUser(payerId != null ? payerId : receiverId);

        if (user is null)
            throw new BattleException("Could not fetch user");

        if (user.Credits < 5)
            throw new BattleException("Not enough credits");

        var connection = DatabaseController.GetInstance().GetConnection();
        var tran = connection.BeginTransaction();

        string acquirePackageQuery =
            @"UPDATE bestcard.cards
            SET owner_user_id = @userid
            WHERE card_id IN
            (
                SELECT
                    card_id
                FROM bestcard.cards
                WHERE owner_user_id IS NULL
                ORDER BY package_id
                LIMIT 5
            );";

        var acquirePackageCmd = new NpgsqlCommand(acquirePackageQuery, connection, tran);
        acquirePackageCmd.Parameters.Add(new NpgsqlParameter("@userid", receiverId));

        var rowCount = acquirePackageCmd.ExecuteNonQuery();

        if (rowCount < 5)
        {
            tran.Rollback();
            throw new ArgumentException("SQL Error");
        }

        tran.Commit();

        UserService.UpdateUserCredits(user.UserId, -5); 
    }

    public static List<CardModel> GetCardsByUserId(int userId)
    {
        var connection = DatabaseController.GetInstance().GetConnection();
        var cards = new List<CardModel>();

        string getCardsQuery =
            @"SELECT
                *
            FROM bestcard.cards
            WHERE owner_user_id = @userid;";

        var getCardsCmd = new NpgsqlCommand(getCardsQuery, connection);
        getCardsCmd.Parameters.Add(new NpgsqlParameter("@userid", userId));

        using (var reader = getCardsCmd.ExecuteReader())
        {
            while (reader.Read())
            {
                var cardId = reader.GetString(reader.GetOrdinal("card_id"));
                var cardName = reader.GetString(reader.GetOrdinal("card_name"));
                var cardDamage = reader.GetFloat(reader.GetOrdinal("card_damage"));
                var card = new CardModel(cardId, cardName, cardDamage);
                cards.Add(card);
            }
        }

        if (cards.Count <= 0)
            throw new ArgumentException("No cards found");

        return cards;
    }

    public static List<CardModel> GetDeckByUserId(int userId)
    {
        var connection = DatabaseController.GetInstance().GetConnection();
        var cards = new List<CardModel>();

        string getCardsQuery =
            @"SELECT
                *
            FROM bestcard.cards
            WHERE
                owner_user_id = @userid AND
                is_selected = true;";

        var getCardsCmd = new NpgsqlCommand(getCardsQuery, connection);
        getCardsCmd.Parameters.Add(new NpgsqlParameter("@userid", userId));

        using (var reader = getCardsCmd.ExecuteReader())
        {
            while (reader.Read())
            {
                var cardId = reader.GetString(reader.GetOrdinal("card_id"));
                var cardName = reader.GetString(reader.GetOrdinal("card_name"));
                var cardDamage = reader.GetFloat(reader.GetOrdinal("card_damage"));
                var card = new CardModel(cardId, cardName, cardDamage);
                cards.Add(card);
            }
        }

        if (cards.Count <= 0)
            throw new ArgumentException("No cards found");

        return cards;
    }

    public static void SetDeckByUserId(int userid, List<string> cards)
    {
        var connection = DatabaseController.GetInstance().GetConnection();

        if (cards.Count != 4)
            throw new ArgumentException($"{cards.Count} cards were passed. Exactly 4 cards need to be passed");

        var tran = connection.BeginTransaction();

        // unselect all current cards of user
        string updateCardsQuery =
            @"UPDATE bestcard.cards
            SET is_selected = false
            WHERE owner_user_id = @userid";
        var userIdParam = new NpgsqlParameter("@userid", userid);
        var updateCardsCmd = new NpgsqlCommand(updateCardsQuery, connection, tran);

        updateCardsCmd.Parameters.Add(userIdParam);
        updateCardsCmd.ExecuteNonQuery();


        // select all specified cards of user
        updateCardsQuery =
            @"UPDATE bestcard.cards
            SET is_selected = true
            WHERE
                owner_user_id = @userid
                and card_id IN (@card1, @card2, @card3, @card4)";

        var paramList = new List<NpgsqlParameter>
        {
            new NpgsqlParameter("@userid", userid),
            new NpgsqlParameter("@card1", cards[0]),
            new NpgsqlParameter("@card2", cards[1]),
            new NpgsqlParameter("@card3", cards[2]),
            new NpgsqlParameter("@card4", cards[3])
        };

        var updateCardsCmd2 = new NpgsqlCommand(updateCardsQuery, connection, tran);
        updateCardsCmd2.Parameters.AddRange(paramList.ToArray());
        int rowCount = updateCardsCmd2.ExecuteNonQuery();

        if (rowCount != 4)
        {
            tran.Rollback();
            throw new ArgumentException("Invalid Package IDs");
        }

        tran.Commit();
    }
}


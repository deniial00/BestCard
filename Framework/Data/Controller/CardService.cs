using Npgsql;
using Framework.Data.Models;
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;
using System.Transactions;

namespace Framework.Data.Controller;

public class CardService
{
    private static CardService? Instance;

    private CardService()
    {

    }

    public static CardService GetInstance()
    {
        if (Instance == null)
            Instance = new CardService();

        return Instance;
    }

    public static bool AddPackage(CardModel[] cards)
    {
        var db = DatabaseController.GetInstance();
        NpgsqlTransaction tran;

        string addPackageQuery = @"INSERT INTO bestcard.packages
                                (
                                    owner_user_id
                                )
                                VALUES
                                (
                                    NULL
                                )
                                RETURNING package_id";
        var addPackageCmd = new NpgsqlCommand(addPackageQuery, db.GetConnection());

        var packageId = addPackageCmd.ExecuteScalar();

        if (packageId is null)
            throw new NpgsqlException("Package could not be created");

        foreach (var card in cards)
        {
            var paramList = new List<NpgsqlParameter>();
            string addCardQuery = @"INSERT INTO bestcard.cards
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
            paramList.Add(new NpgsqlParameter("@PackageId", packageId));

            (int rowCount, tran) = db.ExecuteNonReadQuery(addCardQuery, paramList);

            if (rowCount != 1)
            {
                tran.Rollback();
                return false;
            }

            tran.Commit();
        }


        return true;
    }

    public static bool AcquirePackage(int userId)
    {

        return false;
    }
}


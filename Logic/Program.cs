using Logic.Controller;
using Logic.Models;
using Logic.Models.Cards;
using Logic.Models.Cards.Monsters;
using Logic.Models.Cards.Spells;


try
{
    var server = new HttpServerController();
    await server.Listen();

}
catch(Exception ex)
{
    Console.WriteLine(ex);
}


//public class Program
//{
//    public static int Main(string[] args)
//    {
//        var dbController = DatabaseController.GetInstance();
//        dbController.Query("Select * FROM Users");

//        return 0;
//        // create first deck
//        CardDeck deck1 = new CardDeck();
//        deck1.AddCard(new DragonMonster(CardElement.Fire, 15));
//        deck1.AddCard(new GoblinMonster(CardElement.Water, 10));
//        deck1.AddCard(new KrakenMonster(CardElement.Water, 8));
//        deck1.AddCard(new BoltSpell(CardElement.Water, 10, CardElement.Fire));


//        // create second deck
//        CardDeck deck2 = new CardDeck();
//        deck2.AddCard(new BoltSpell(CardElement.Fire, 9, CardElement.Normal));
//        deck2.AddCard(new WizardMonster(CardElement.Water, 8));
//        deck2.AddCard(new GoblinMonster(CardElement.Water, 12));
//        deck2.AddCard(new ElfMonster(CardElement.Water, 10));


//        // instantiate CardManager
//        var cardManager = new BattleController();

//        // shuffle decks
//        deck1.ShuffleDeck();
//        deck2.ShuffleDeck();

//        // draw cards from decks
//        var card1 = deck1.DrawCard();
//        var card2 = deck2.DrawCard();

//        cardManager.PrintRound(card1, card2);

//        var roundWinner = cardManager.CalculateResult(card1, card2);

//        if (roundWinner == null)
//        {
//            Console.Write("Draw!");
//        }
//        else
//        {
//            roundWinner.GetInfo();
//        }

//        return 0;
//    }

//}

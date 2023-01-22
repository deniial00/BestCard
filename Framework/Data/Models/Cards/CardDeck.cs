using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Framework.Data.Models.Cards;

namespace Framework.Data.Models;

public class CardDeck
{
    private List<ICard> Cards;

    public int Count { get => Cards.Count; }

    public CardDeck(List<ICard> cards)
    {
        Cards = cards;
    }

    public CardDeck()
    {
        List<ICard> cards = new List<ICard>();
        Cards = cards;
    }

    public void AddCard(ICard card)
    {
        Cards.Add(card);
    }

    public void ShuffleDeck()
    {
        Random rng = new Random();
        ICard temp;
        int n = Cards.Count;

        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            temp = Cards[k];
            Cards[k] = Cards[n];
            Cards[n] = temp;
        }
    }

    public ICard DrawCard()
    {
        ICard card = Cards.First();
        Cards.RemoveAt(Cards.IndexOf(card));
        return card;
    }

    public ICard RemoveCard(ICard card)
    {
        ICard? temp = null;
        foreach (var cardToRemove in Cards)
        {
            if (cardToRemove != card) continue;

            temp = cardToRemove;
            Cards.Remove(cardToRemove);
        }

        if (temp == null)
        {
            throw new ArgumentException($"Card '{card.CardName}' was not found in deck");
        }

        return temp;
    }
}

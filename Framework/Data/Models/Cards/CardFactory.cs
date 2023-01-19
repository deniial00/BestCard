using System;
using System.Diagnostics;
using Framework.Data.Models.Cards.Monsters;
using Framework.Data.Models.Cards.Spells;


namespace Framework.Data.Models.Cards;

public class CardFactory
{
    public static ICard? ConvertCard(CardModel cardModel)
    {
        ICard? card = CardFactory.CreateCard(cardModel.CardName, cardModel.CardDamage);

        return card;
    }

    public static List<ICard> ConvertCards(List<CardModel> cardModels)
    {
        List<ICard> ret = new List<ICard>();

        foreach(var model in cardModels)
        {
            var card = ConvertCard(model);

            if (card is null)
                continue;

            ret.Add(card);
        }

        return ret;
    }

	public static ICard? CreateCard(string name, float damage)
	{
		ICard card;
		CardElement element;
		CardElement effectiveAgainst;

		if (name.Contains("Fire"))
		{
			// FireElement
			element = CardElement.Fire;
			effectiveAgainst = CardElement.Normal;
		}
		else if (name.Contains("Water"))
        {
			// WaterElement
            element = CardElement.Water;
            effectiveAgainst = CardElement.Fire;
        }
		else
		{
			//Normal
			element = CardElement.Normal;
            effectiveAgainst = CardElement.Water;
        }

        // DragonMonster

        if (name.Contains("Dragon"))
        {
			card = new DragonMonster(element, damage, effectiveAgainst);
        }
		else if (name.Contains("Elf"))
		{
			card = new ElfMonster(element, damage, effectiveAgainst);
        }
        else if (name.Contains("Goblin"))
        {
            card = new GoblinMonster(element, damage, effectiveAgainst);
        }
		else if (name.Contains("Knight"))
        {
            card = new KnightMonster(element, damage, effectiveAgainst);
        }
        else if (name.Contains("Kraken"))
        {
            card = new KrakenMonster(element, damage, effectiveAgainst);
        }
        else if (name.Contains("Ork"))
        {
            card = new OrkMonster(element, damage, effectiveAgainst);
        }
        else if (name.Contains("Wizard"))
        {
            card = new WizardMonster(element, damage, effectiveAgainst);
        }
        else if (name.Contains("BoltSpell"))
        {
            card = new BoltSpell(element, damage, effectiveAgainst);
        }
        else
        {
            return null;
        }

        return card;
	}
}


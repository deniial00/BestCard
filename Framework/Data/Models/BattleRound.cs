using System;
using Framework.Data.Models.Cards;

namespace Framework.Models;

public class BattleRound
{
	public ICard ChampionCard;
	public ICard ChallengerCard;

	public bool? ChallengerSucceded;

	public BattleRound(ICard championCard, ICard challengerCard)
	{
		ChampionCard = championCard;
		ChallengerCard = challengerCard;

		CalculateRound();
	}

    private ICard? CalculateRound()
    {
        float card1DamageMultiplier = 1;
        float card2DamageMultiplier = 1;

        if (ChampionCard.CardType == CardType.Spell)
        {
            if (ChampionCard.CardEffectiveAgainst == ChallengerCard.CardElement)
            {
                card1DamageMultiplier = 2;
                card2DamageMultiplier = 0.5f;
            }
        }

        if (ChallengerCard.CardType == CardType.Spell)
        {
            if (ChallengerCard.CardEffectiveAgainst == ChampionCard.CardElement)
            {
                card1DamageMultiplier = 0.5f;
                card2DamageMultiplier = 2;
            }
        }

        float card2Remaining = ChampionCard.Attack(ChallengerCard, card1DamageMultiplier);
        float card1Remaining = ChallengerCard.Attack(ChampionCard, card2DamageMultiplier);

        if (card1Remaining <= 0 && card2Remaining > 0)
        {
            return ChallengerCard;
        }
        else if (card2Remaining <= 0 && card1Remaining > 0)
        {
            return ChampionCard;
        }
        return null;
    }

    public void PrintRound(ICard card1, ICard card2)
    {
        Console.Write($"PlayerA: {card1.CardElement + card1.CardName} ({card1.CardDamage} dmg) vs.{Environment.NewLine}" +
                      $"PlayerB: {card2.CardElement + card2.CardName} ({card2.CardDamage} dmg){Environment.NewLine}");
    }


}


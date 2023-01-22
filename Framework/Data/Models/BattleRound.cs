using System;
using Framework.Data.Models.Cards;

namespace Framework.Models;

public class BattleRound
{
	public ICard ChampionCard;
	public ICard ChallengerCard;

    public string? RoundWinnerName;
    public bool? ChallengerSucceded;

    public float ChampionRemaining;
    public float ChallengerRemaining;

    public BattleRound(ICard championCard, ICard challengerCard)
	{
		ChampionCard = championCard;
		ChallengerCard = challengerCard;

		CalculateRound();
	}

    private void CalculateRound()
    {
        float championDamageMultiplier = 1;
        float challengerDamageMultiplier = 1;

        if (ChampionCard.CardType == CardType.Spell)
        {
            if (ChampionCard.CardEffectiveAgainst == ChallengerCard.CardElement)
            {
                championDamageMultiplier = 2;
                challengerDamageMultiplier = 0.5f;
            }
        }

        if (ChallengerCard.CardType == CardType.Spell)
        {
            if (ChallengerCard.CardEffectiveAgainst == ChampionCard.CardElement)
            {
                championDamageMultiplier = 0.5f;
                challengerDamageMultiplier = 2;
            }
        }

        ChampionRemaining = ChampionCard.Attack(ChallengerCard, championDamageMultiplier);
        ChallengerRemaining = ChallengerCard.Attack(ChampionCard, challengerDamageMultiplier);

        if (ChallengerRemaining == ChampionRemaining)
            return;

        if (ChallengerRemaining <= 0 && ChampionRemaining > 0)
        {
            RoundWinnerName = ChallengerCard.CardName;
            ChallengerSucceded = true;
        }
        else if (ChampionRemaining <= 0 && ChallengerRemaining > 0)
        {
            RoundWinnerName = ChampionCard.CardName;
            ChallengerSucceded = false;
        }
    }

    public string SummarizeRound()
    {
        string ret = $"Champion: {ChampionCard.CardElement + ChampionCard.CardName} ({ChampionCard.CardDamage} dmg) vs Challenger: {ChallengerCard.CardElement + ChallengerCard.CardName} ({ChallengerCard.CardDamage} dmg){Environment.NewLine}" +
                     $"{ChampionCard.CardDamage} vs {ChallengerCard.CardDamage} => {(int)ChampionRemaining} vs {(int)ChallengerRemaining} => {(RoundWinnerName is not null ? RoundWinnerName : "Draw")}";

        return ret;
    }

    public string SummarizeRoundJsonString()
    {
        string ret =    $"{{ " +
                        $"  \"matchup\": \"Champion: {ChampionCard.CardElement + ChampionCard.CardName} ({ChampionCard.CardDamage} dmg) vs Challenger: {ChallengerCard.CardElement + ChallengerCard.CardName} ({ChallengerCard.CardDamage} dmg)\"," +
                        $"  \"result\": \"{ChampionCard.CardDamage} vs {ChallengerCard.CardDamage} => {(int)ChampionRemaining} vs {(int)ChallengerRemaining} => {(RoundWinnerName is not null ? RoundWinnerName : "Draw")}\"" +
                        $"}}";
        return ret;
    }


}


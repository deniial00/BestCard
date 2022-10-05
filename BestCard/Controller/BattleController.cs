using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logic.Models.Cards;

namespace Logic.CardLogic
{
    class BattleController
    {
        public ICard? CalculateResult(ICard card1, ICard card2)
        {
            float card1DamageMultiplier = 1;
            float card2DamageMultiplier = 1;

            if (card1.CardType == CardType.Spell)
            {
                if (card1.CardEffectiveAgainst == card2.CardElement)
                {
                    card1DamageMultiplier = 2;
                    card2DamageMultiplier = 0.5f;
                }
            }

            if (card2.CardType == CardType.Spell)
            {
                if (card2.CardEffectiveAgainst == card1.CardElement)
                {
                    card1DamageMultiplier = 0.5f;
                    card2DamageMultiplier = 2;
                }
            }

            float card2Remaining = card1.Attack(card2, card1DamageMultiplier);
            float card1Remaining = card2.Attack(card1, card2DamageMultiplier);

            if (card1Remaining <= 0 && card2Remaining > 0)
            {
                return card2;
            } else if (card2Remaining <= 0 && card1Remaining > 0)
            {
                return card1;
            }
            return null;
        }

        public void PrintRound(ICard card1, ICard card2)
        {
            Console.Write($"PlayerA: {card1.CardElement + card1.CardName} ({card1.CardAttack} dmg) vs.{Environment.NewLine}" +
                          $"PlayerB: {card2.CardElement + card2.CardName} ({card2.CardAttack} dmg){Environment.NewLine}");
        }
    }
}

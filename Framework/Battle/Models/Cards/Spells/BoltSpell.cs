using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Framework.Battle.Models.Cards;

namespace Framework.Battle.Models.Cards.Spells
{
    internal class BoltSpell : ICard
    {
        public CardType CardType { get; }

        public string CardName { get; }

        public CardElement CardElement { get; }

        public CardElement? CardEffectiveAgainst { get; }

        public int CardAttack { get; }

        public BoltSpell(CardElement cardElement, int cardAttack, CardElement? cardEffectiveAgainst = null)
        {
            CardType = CardType.Spell;
            CardName = "Bolt";
            CardElement = cardElement;
            CardAttack = cardAttack;
            CardEffectiveAgainst = cardEffectiveAgainst;
        }

        public float Attack(ICard enemyCard, float damageMultiplier)
        {
            if (enemyCard.CardName == "Kraken")
            {
                return enemyCard.CardAttack;
            }
            return enemyCard.CardAttack - CardAttack * damageMultiplier;
            ;
        }

        public void GetInfo()
        {
            Console.Write($"Name: {CardElement + " " + CardName + Environment.NewLine}Type: {CardType + Environment.NewLine}Element: {CardElement + Environment.NewLine}Damage: {CardAttack + Environment.NewLine}");
        }
    }
}

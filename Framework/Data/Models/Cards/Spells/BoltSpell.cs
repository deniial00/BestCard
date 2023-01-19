using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Framework.Data.Models.Cards;

namespace Framework.Data.Models.Cards.Spells
{
    internal class BoltSpell : ICard
    {
        public CardType CardType { get; }

        public string CardName { get; }

        public CardElement CardElement { get; }

        public CardElement? CardEffectiveAgainst { get; }

        public float CardDamage { get; }

        public BoltSpell(CardElement cardElement, float CardDamage, CardElement? cardEffectiveAgainst = null)
        {
            CardType = CardType.Spell;
            CardName = "Bolt";
            CardElement = cardElement;
            CardDamage = CardDamage;
            CardEffectiveAgainst = cardEffectiveAgainst;
        }

        public float Attack(ICard enemyCard, float damageMultiplier)
        {
            if (enemyCard.CardName == "Kraken")
            {
                return enemyCard.CardDamage;
            }
            return enemyCard.CardDamage - CardDamage * damageMultiplier;
            ;
        }

        public void GetInfo()
        {
            Console.Write($"Name: {CardElement + " " + CardName + Environment.NewLine}Type: {CardType + Environment.NewLine}Element: {CardElement + Environment.NewLine}Damage: {CardDamage + Environment.NewLine}");
        }
    }
}

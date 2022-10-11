using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Battle.Models.Cards.Monsters
{
    class KnightMonster : ICard
    {
        public CardType CardType { get; }

        public string CardName { get; }

        public CardElement CardElement { get; }
        public CardElement? CardEffectiveAgainst { get; }

        public int CardAttack { get; }

        public KnightMonster(CardElement cardElement, int cardDamage, CardElement? cardEffectiveAgainst = null)
        {
            CardType = CardType.Monster;
            CardName = "Knight";
            CardElement = cardElement;
            CardEffectiveAgainst = cardEffectiveAgainst;
            CardAttack = cardDamage;
        }

        public float Attack(ICard enemyCard, float damageMultiplier)
        {
            if (enemyCard.CardName == "Bolt" && enemyCard.CardElement == CardElement.Water)
            {
                return enemyCard.CardAttack;
            }
            return enemyCard.CardAttack - CardAttack * damageMultiplier;
        }

        public void GetInfo()
        {
            Console.Write($"Name: {CardElement + " " + CardName + Environment.NewLine}Type: {CardType + Environment.NewLine}Element: {CardElement + Environment.NewLine}Damage: {CardAttack + Environment.NewLine}");
        }
    }
}

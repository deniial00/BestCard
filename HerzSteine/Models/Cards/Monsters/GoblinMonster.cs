using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logic.Models.Cards;

namespace Logic.Models.Cards.Monsters
{
    class GoblinMonster : ICard
    {
        public CardType CardType { get; }

        public string CardName { get; }

        public CardElement CardElement { get; }
        public CardElement? CardEffectiveAgainst { get; }

        public int CardAttack { get; }

        public GoblinMonster( CardElement cardElement, int cardDamage, CardElement? cardEffectiveAgainst = null)
        {
            CardType = CardType.Monster;
            CardName = "Goblin";
            CardElement = cardElement;
            CardEffectiveAgainst = cardEffectiveAgainst;
            CardAttack = cardDamage;
        }

        public float Attack(ICard enemyCard, float damageMultiplier)
        {
            if (enemyCard.CardName == "Dragon")
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

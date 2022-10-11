﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Battle.Models.Cards.Monsters
{
    class DragonMonster : ICard
    {
        public CardType CardType { get; }

        public string CardName { get; }

        public CardElement CardElement { get; }

        public CardElement? CardEffectiveAgainst { get; }

        public int CardAttack { get; }

        public DragonMonster(CardElement cardElement, int cardDamage, CardElement? cardEffectiveAgainst = null)
        {
            CardType = CardType.Monster;
            CardName = "Dragon";
            CardElement = cardElement;
            CardEffectiveAgainst = cardEffectiveAgainst;
            CardAttack = cardDamage;
        }

        public float Attack(ICard enemyCard, float damageMultiplier)
        {
            if (enemyCard.CardName == "Elf" && enemyCard.CardElement == CardElement.Fire)
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
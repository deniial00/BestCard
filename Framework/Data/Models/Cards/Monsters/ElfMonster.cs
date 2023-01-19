﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Data.Models.Cards.Monsters;

class ElfMonster : ICard
{
    public CardType CardType { get; }

    public string CardName { get; }

    public CardElement CardElement { get; }
    public CardElement? CardEffectiveAgainst { get; }

    public float CardDamage { get; }

    public ElfMonster(CardElement cardElement, float cardDamage, CardElement? cardEffectiveAgainst = null)
    {
        CardType = CardType.Monster;
        CardName = "Elf";
        CardElement = cardElement;
        CardEffectiveAgainst = cardEffectiveAgainst;
        CardDamage = cardDamage;
    }

    public float Attack(ICard enemyCard, float damageMultiplier)
    {
        return enemyCard.CardDamage - CardDamage * damageMultiplier;
    }

    public void GetInfo()
    {
        Console.Write($"Name: {CardElement + " " + CardName + Environment.NewLine}Type: {CardType + Environment.NewLine}Element: {CardElement + Environment.NewLine}Damage: {CardDamage + Environment.NewLine}");
    }
}

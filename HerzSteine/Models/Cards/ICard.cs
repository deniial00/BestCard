using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Models.Cards
{
    public enum CardElement
    {
        Fire, Water, Normal
    }

    public enum CardType
    {
        Spell,
        Monster
    }

    public interface ICard
    {
        CardType CardType { get; }
        string CardName { get; }
        CardElement CardElement { get; }
        CardElement? CardEffectiveAgainst { get; }
        int CardAttack { get; }
        float Attack(ICard enemyCard, float damageMultiplier);
        void GetInfo();

    }
}

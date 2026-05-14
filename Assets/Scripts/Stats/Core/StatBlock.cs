using System;
using UnityEngine;

namespace ProjectPinch.Stats.Core
{
    [Serializable]
    public class StatBlock
    {
        [Header("Resources")]
        public int maxHealth;
        public int maxMana;

        [Header("Combat")]
        public int physicalAttack;
        public int magicPower;
        public int defense;

        [Header("Movement")]
        public float moveSpeed;

        public static StatBlock operator +(StatBlock a, StatBlock b)
        {
            return new StatBlock
            {
                maxHealth = a.maxHealth + b.maxHealth,
                maxMana = a.maxMana + b.maxMana,
                physicalAttack = a.physicalAttack + b.physicalAttack,
                magicPower = a.magicPower + b.magicPower,
                defense = a.defense + b.defense,
                moveSpeed = a.moveSpeed + b.moveSpeed
            };
        }
    }
}
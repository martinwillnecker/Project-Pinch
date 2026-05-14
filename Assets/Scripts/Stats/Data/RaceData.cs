using UnityEngine;
using ProjectPinch.Stats.Core;

namespace ProjectPinch.Stats.Data
{
    [CreateAssetMenu(fileName = "NewRace", menuName = "Project Pinch/Race")]
    public class RaceData : ScriptableObject
    {
        public string raceName;
        public StatBlock baseStats;
    }
}
using UnityEngine;
using ProjectPinch.Stats.Core;

namespace ProjectPinch.Stats.Data
{
    [CreateAssetMenu(fileName = "NewClass", menuName = "Project Pinch/Class")]
    public class ClassData : ScriptableObject
    {
        public string className;
        public StatBlock baseStats;
    }
}
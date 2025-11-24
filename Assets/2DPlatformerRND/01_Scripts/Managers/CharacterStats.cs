using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PahlBit
{
    public class CharacterStats : MonoBehaviour
    {
        public CharacterRoot CharRoot => GetComponentInParent<CharacterRoot>();

        public StatsBase BasicStat { get; private set; } = new StatsBase();

        public void Init()
        {
        }

        public void UpdateState()
        {
        }

        public void GetHealth(string SkillID)
        {
        }
        public void IsCastable(string skillID)
        {
        }
        public void CastSkill(string skillID)
        {
        }
    }
}
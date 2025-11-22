using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PahlBit
{
    public class PlayerStats : MonoBehaviour
    {
        public PlayerRoot PlayerRoot => GetComponentInParent<PlayerRoot>();

        public PropertyBase BasicStat { get; private set; } = new PropertyBase();

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
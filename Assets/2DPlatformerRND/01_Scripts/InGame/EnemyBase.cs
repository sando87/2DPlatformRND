using PahlBit;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyBase : MonoBehaviour
{
    [SerializeField] Animator animator;

    BaseObject mBase = null;

    private void Awake()
    {
        mBase = GetComponentInParent<BaseObject>();
    }

    public void GetDamaged(float damage)
    {
        mBase.AnimHelper.CrossFadeToState(AnimStateNameHash.Hit);
    }
}

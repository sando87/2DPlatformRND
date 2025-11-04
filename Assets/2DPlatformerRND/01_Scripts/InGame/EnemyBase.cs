using PahlBit;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyBase : MonoBehaviour
{
    BaseObject mBase = null;

    private void Awake()
    {
        mBase = GetComponentInParent<BaseObject>();
    }

    public void GetDamaged(int damage)
    {
        if (damage == 1)
        {
            mBase.Phy.VelocityX = 1;
            mBase.Phy.VelocityY = 5;
        }
        else if (damage == 2)
        {
            mBase.Phy.VelocityX = 3;
            mBase.Phy.VelocityY = 15;
        }
        
        mBase.AnimHelper.CrossFadeToState(AnimStateNameHash.Hit);
    }
}

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
            mBase.Phy.VelocityX = 2;
            mBase.Phy.VelocityY = 8;
            mBase.AnimHelper.CrossFadeToState(AnimStateNameHash.Hit);
        }
        else if (damage == 2)
        {
            mBase.Phy.VelocityX = 5;
            mBase.Phy.VelocityY = 25;
            mBase.AnimHelper.CrossFadeToState(AnimStateNameHash.HitFlying);
        }
        else if (damage == 3)
        {
            mBase.Phy.VelocityX = 7;
            mBase.Phy.VelocityY = 7;
            mBase.AnimHelper.CrossFadeToState(AnimStateNameHash.HitStrong);
        }
        
        
    }
}

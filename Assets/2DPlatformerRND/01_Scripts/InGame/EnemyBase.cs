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

    void Start()
    {
        EnemyDataMono enemyDataMono = mBase.GetComponentInChildren<EnemyDataMono>();
        mBase.Health.InitHealth(enemyDataMono.Data.Stats.Health, 0, 0);

    }

    public void GetDamaged(int damage, Vector2 force)
    {
        ItemObject.Create(mBase.Body.Center, Quaternion.identity);

        Vector3 front = force.x > 0 ? Vector3.back : Vector3.forward;
        mBase.transform.rotation = Quaternion.LookRotation(front, transform.up);

        float dir = force.x > 0 ? 1 : (force.x < 0 ? -1 : mBase.transform.right.x);
        if (damage == 1)
        {
            mBase.Phy.VelocityX = 2 * dir;
            mBase.Phy.VelocityY = 12;
            mBase.AnimHelper.CrossFadeToState(AnimStateNameHash.Hit);
        }
        else if (damage == 2)
        {
            mBase.Phy.VelocityX = 5 * dir;
            mBase.Phy.VelocityY = 30;
            mBase.AnimHelper.CrossFadeToState(AnimStateNameHash.HitFlying);
        }
        else if (damage == 3)
        {
            mBase.Phy.VelocityX = 7 * dir;
            mBase.Phy.VelocityY = 7;
            mBase.AnimHelper.CrossFadeToState(AnimStateNameHash.HitStrong);
        }
    }

    void DropItem()
    {

    }
}

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
        mBase.Health.OnDied.AddListener(OnDeath);

    }

    public void OnDeath()
    {
        DropItem();
    }

    void DropItem()
    {
        ItemObject.Create(mBase.Body.Center, Quaternion.identity);
    }
}
